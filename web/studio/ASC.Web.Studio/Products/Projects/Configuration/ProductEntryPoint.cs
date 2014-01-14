/* 
 * 
 * (c) Copyright Ascensio System Limited 2010-2014
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as
 * published by the Free Software Foundation, either version 3 of the
 * License, or (at your option) any later version.
 * 
 * http://www.gnu.org/licenses/agpl.html 
 * 
 */

using System;
using System.IO;
using System.Linq;
using System.Web;
using ASC.Common.Web;
using ASC.Notify;
using ASC.Notify.Engine;
using ASC.Projects.Core.Services.NotifyService;
using ASC.Projects.Engine;
using ASC.Web.Core;
using ASC.Web.Core.Utility;
using ASC.Web.Files.Api;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Resources;
using ASC.Web.Studio.Utility;
using log4net;

namespace ASC.Web.Projects.Configuration
{
    public class ProductEntryPoint : Product
    {
        private static readonly object Locker = new object();
        private static bool registered;

        private ProductContext context;

        public static readonly Guid ID = EngineFactory.ProductId;
        public static readonly Guid MilestoneModuleID = new Guid("{AF4AFD50-5553-47f3-8F91-651057BC930B}");
        public static readonly Guid TasksModuleID = new Guid("{04339423-70E6-4b81-A2DF-3C31C723BD90}");
        public static readonly Guid GanttChartModuleID = new Guid("{23CD2123-3C4C-4868-B927-A26BB49CA458}");
        public static readonly Guid MessagesModuleID = new Guid("{9FF0FADE-6CFA-44ee-901F-6185593E4594}");
        public static readonly Guid DocumentsModuleID = new Guid("{81402440-557D-401d-9EE1-D570748F426D}");
        public static readonly Guid TimeTrackingModuleID = new Guid("{57E87DA0-D59B-443d-99D1-D9ABCAB31084}");
        public static readonly Guid ProjectTeamModuleID = new Guid("{C42F993E-5D22-497e-AC26-1E9592515898}");
        public static readonly Guid ContactsModuleID = new Guid("{ec12f0ba-14cb-413c-b5e5-65f6ddd5fc19}");

        public override Guid ProductID
        {
            get { return ID; }
        }

        public override string Name
        {
            get { return ProjectsCommonResource.ProductName; }
        }

        public override string ExtendedDescription
        {
            get { return string.Format(ProjectsCommonResource.ProductDescriptionEx, "<span style='display:none'>", "</span>"); }
        }

        public override string Description
        {
            get { return ProjectsCommonResource.ProductDescription; }

        }

        public override string StartURL
        {
            get { return PathProvider.BaseVirtualPath; }
        }

        public override string ProductClassName
        {
            get { return "projects"; }
        }

        public override ProductContext Context
        {
            get { return context; }
        }

        public override void Init()
        {
            context = new ProductContext
                          {
                              MasterPageFile = String.Concat(PathProvider.BaseVirtualPath, "Masters/BasicTemplate.Master"),
                              DisabledIconFileName = "product_disabled_logo.png",
                              IconFileName = "product_logo.png",
                              LargeIconFileName = "product_logolarge.png",
                              SubscriptionManager = new ProductSubscriptionManager(),
                              DefaultSortOrder = 20,
                              SpaceUsageStatManager = new ProjectsSpaceUsageStatManager(),
                              AdminOpportunities = () => ProjectsCommonResource.ProductAdminOpportunities.Split('|').ToList(),
                              UserOpportunities = () => ProjectsCommonResource.ProductUserOpportunities.Split('|').ToList(),
                              HasComplexHierarchyOfAccessRights = true,
                          };

            FilesIntegration.RegisterFileSecurityProvider("projects", "project", new SecurityAdapterProvider());
            SearchHandlerManager.Registry(new SearchHandler());

            var securityInterceptor = new SendInterceptorSkeleton(
                "ProjectInterceptorSecurity",
                InterceptorPlace.DirectSend,
                InterceptorLifetime.Global,
                (r, p) =>
                    {
                        HttpContext.Current = null;
                        try
                        {
                            HttpContext.Current = new HttpContext(
                                       new HttpRequest("hack", CommonLinkUtility.GetFullAbsolutePath("/"), string.Empty),
                                       new HttpResponse(new StringWriter()));

                            var data = r.ObjectID.Split('_');
                            var entityType = data[0];
                            var entityId = Convert.ToInt32(data[1]);

                            var projectId = 0;
                            
                            if(data.Length == 3)
                                projectId = Convert.ToInt32(r.ObjectID.Split('_')[2]);

                            switch (entityType)
                            {
                                case "Task":
                                    var task = Global.EngineFactory.GetTaskEngine().GetByID(entityId, false);

                                    if (task == null && projectId != 0)
                                    {
                                        var project = Global.EngineFactory.GetProjectEngine().GetByID(projectId, false);
                                        return !ProjectSecurity.CanRead(project, new Guid(r.Recipient.ID));
                                    }

                                    return !ProjectSecurity.CanRead(task, new Guid(r.Recipient.ID));
                                case "Message":
                                    var discussion = Global.EngineFactory.GetMessageEngine().GetByID(entityId, false);

                                    if (discussion == null && projectId != 0)
                                    {
                                        var project = Global.EngineFactory.GetProjectEngine().GetByID(projectId, false);
                                        return !ProjectSecurity.CanRead(project, new Guid(r.Recipient.ID));
                                    }

                                    return !ProjectSecurity.CanRead(discussion, new Guid(r.Recipient.ID));
                                case "Milestone":
                                    var milestone = Global.EngineFactory.GetMilestoneEngine().GetByID(entityId, false);

                                    if (milestone == null && projectId != 0)
                                    {
                                        var project = Global.EngineFactory.GetProjectEngine().GetByID(projectId, false);
                                        return !ProjectSecurity.CanRead(project, new Guid(r.Recipient.ID));
                                    }

                                    return !ProjectSecurity.CanRead(milestone, new Guid(r.Recipient.ID));
                            }
                        }
                        catch (Exception ex)
                        {
                            LogManager.GetLogger("ASC.Projects.Tasks").Error("Send", ex);
                        }
                        finally
                        {
                            if (HttpContext.Current != null)
                            {
                                new DisposableHttpContext(HttpContext.Current).Dispose();
                                HttpContext.Current = null;
                            }
                        }
                        return false;
                    });

            NotifyClient.Instance.Client.AddInterceptor(securityInterceptor);
        }

        public override void Shutdown()
        {
            if (registered)
            {
                NotifyClient.Instance.Client.UnregisterSendMethod(NotifyHelper.SendMsgMilestoneDeadline);
                NotifyClient.Instance.Client.UnregisterSendMethod(NotifyHelper.SendAutoReports);
                NotifyClient.Instance.Client.UnregisterSendMethod(NotifyHelper.SendAutoReminderAboutTask);

                NotifyClient.Instance.Client.RemoveInterceptor("ProjectInterceptorSecurity");
            }
        }

        public static void RegisterSendMethods()
        {
            lock (Locker)
            {
                if (!registered)
                {
                    registered = true;
                    NotifyClient.Instance.Client.RegisterSendMethod(NotifyHelper.SendMsgMilestoneDeadline, "0 0 7 ? * *");
                    NotifyClient.Instance.Client.RegisterSendMethod(NotifyHelper.SendAutoReports, "0 0 * ? * *");
                    NotifyClient.Instance.Client.RegisterSendMethod(NotifyHelper.SendAutoReminderAboutTask, "0 0 * ? * *");
                }
            }
        }
    }
}