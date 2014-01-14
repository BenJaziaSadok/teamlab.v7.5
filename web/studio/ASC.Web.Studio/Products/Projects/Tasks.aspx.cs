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
using System.Globalization;
using System.Web;

using ASC.Web.Core.Utility.Skins;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Configuration;
using ASC.Web.Projects.Controls;
using ASC.Web.Projects.Controls.Tasks;
using ASC.Web.Projects.Resources;
using ASC.Web.Studio.Utility;

using ASC.Projects.Core.Domain;

namespace ASC.Web.Projects
{
    public partial class Tasks : BasePage
    {
        protected override string CookieKeyForPagination
        {
            get
            {
                return "tasksKeyForPagination";
            }
        }

        protected override void PageLoad()
        {
            int taskID;

            if (Int32.TryParse(UrlParameters.EntityID, out taskID))
            {
                if (Project == null) return;

                var task = Global.EngineFactory.GetTaskEngine().GetByID(taskID);

                if (task == null)
                {
                    TaskNotFoundControlView(Project.ID);
                    Title = HeaderStringHelper.GetPageTitle(TaskResource.TaskNotFound_Header);
                }
                else
                {
                    InitTaskPage(task);
                }
            }
            else
            {
                InitTaskList();
            }

            Page.RegisterInlineScript(@"
                    if (jq.getURLParam('id')) {
                        ASC.Projects.TaskDescroptionPage.init();
                    } else {
                        ASC.Projects.TasksManager.init(" + EntryCountOnPage + ",'" + CookieKeyForPagination + "', " + Global.VisiblePageCount + ");}", true);
        }

        protected void TaskNotFoundControlView(int projectId)
        {
            _content.Controls.Add(new ElementNotFoundControl
                {
                    Header = TaskResource.TaskNotFound_Header,
                    Body = TaskResource.TaskNotFound_Body,
                    RedirectURL = String.Format("tasks.aspx?prjID=" + projectId),
                    RedirectTitle = TaskResource.TaskNotFound_RedirectTitle
                });
        }

        public string GetProjectId()
        {
            return RequestContext.GetCurrentProjectId().ToString(CultureInfo.InvariantCulture);
        }

        private void InitTaskPage(Task task)
        {
            var taskDescriptionView = (TaskDescriptionView)LoadControl(PathProvider.GetControlVirtualPath("TaskDescriptionView.ascx"));

            taskDescriptionView.Task = task;

            _content.Controls.Add(taskDescriptionView);

            Master.EssenceTitle = task.Title;
            Master.IsSubcribed = Global.EngineFactory.GetTaskEngine().IsSubscribed(task);

            if ((int)task.Status == 2)
            {
                Master.EssenceStatus = TaskResource.Closed.ToLower();
            }

            Title = HeaderStringHelper.GetPageTitle(task.Title);
        }

        private void InitTaskList()
        {
            var taskList = (TaskList)LoadControl(PathProvider.GetControlVirtualPath("TaskList.ascx"));
            _content.Controls.Add(taskList);

            _content.Controls.Add(Masters.BasicTemplate.RenderEmptyScreenForFilter(TaskResource.NoTasks, TaskResource.DescrEmptyListTaskFilter));

            var emptyScreenControl = new Studio.Controls.Common.EmptyScreenControl
                {
                    ImgSrc = WebImageSupplier.GetAbsoluteWebPath("empty_screen_tasks.png", ProductEntryPoint.ID),
                    Header = TaskResource.NoTasksCreated,
                    Describe = String.Format(TaskResource.TasksHelpTheManage, TaskResource.DescrEmptyListTaskFilter),
                    ID = "emptyTaskListScreen"
                };

            if (RequestContext.CanCreateTask(true))
                emptyScreenControl.ButtonHTML = String.Format("<span class='baseLinkAction addFirstElement'>{0}</span>", TaskResource.AddFirstTask);

            _content.Controls.Add(emptyScreenControl);

            Title = HeaderStringHelper.GetPageTitle(TaskResource.Tasks);
        }
    }
}