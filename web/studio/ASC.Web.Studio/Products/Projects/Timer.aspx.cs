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
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASC.Web.Projects.Classes;
using ASC.Projects.Core.Domain;
using System.Globalization;

using ASC.Core.Users;
using ASC.Web.Core.Users;
using ASC.Web.Studio.Utility;
using ASC.Web.Projects.Resources;

namespace ASC.Web.Projects
{
    public partial class Timer : BasePage
    {
        #region Properties

        public int Target { get; set; }

        protected List<Participant> Users { get; set; }

        protected List<Project> UserProjects { get; set; }

        protected IEnumerable<Task> OpenUserTasks { get; set; }

        protected IEnumerable<Task> ClosedUserTasks { get; set; }

        protected string DecimalSeparator
        {
            get { return CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator; }
        }

        public static int TaskID
        {
            get
            {
                int id;
                if (Int32.TryParse(UrlParameters.EntityID, out id))
                {
                    return id;
                }
                return -1;
            }
        }

        #endregion


        #region Events

        protected override void PageLoad()
        {
            Master.Master.DisabledTopStudioPanel = true;
            Master.DisabledSidePanel = true;
            Master.DisabledPrjNavPanel = true;
            Title = HeaderStringHelper.GetPageTitle(ProjectsCommonResource.AutoTimer);

            if (!RequestContext.CanCreateTime()) return;

            RenderContentForTimer();

            Page.RegisterInlineScript(@"jq(document).ready(function() { ASC.Projects.TimeTraking.init();})", true);
        }

        #endregion

        #region Methods

        private void RenderContentForTimer()
        {
            var participantId = Guid.Empty;

            if (!Participant.IsAdmin)
                participantId = Participant.ID;

            UserProjects = Global.EngineFactory.GetProjectEngine().GetOpenProjectsWithTasks(participantId);

            if (UserProjects.Any() && (Project == null || !UserProjects.Contains(Project)))
                Project = UserProjects[0];

            if (Project == null) return;

            var tasks = Global.EngineFactory.GetTaskEngine().GetByProject(Project.ID, null, Participant.IsVisitor ? participantId : Guid.Empty);

            OpenUserTasks = tasks.Where(r => r.Status == TaskStatus.Open).OrderBy(r => r.Title);
            ClosedUserTasks = tasks.Where(r => r.Status == TaskStatus.Closed).OrderBy(r => r.Title);

            Users = Global.EngineFactory.GetProjectEngine().GetTeam(Project.ID).OrderBy(r => DisplayUserSettings.GetFullUserName(r.UserInfo)).Where(r => r.UserInfo.IsVisitor() != true).ToList();

            if (!string.IsNullOrEmpty(Request.QueryString["taskId"]))
            {
                Target = int.Parse(Request.QueryString["taskId"]);
            }
        }

        private void GetTargetTaskId()
        {
            Target = -1;
            int id;
            if (Int32.TryParse(UrlParameters.EntityID, out id))
            {
                Target = id;
            }

            if (Target > 0)
            {
                var t = Global.EngineFactory.GetTaskEngine().GetByID(Target);
                if (t == null || t.Status == TaskStatus.Closed) Target = -1;
            }
        }

        #endregion
    }
}