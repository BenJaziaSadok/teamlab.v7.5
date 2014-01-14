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

#region Import

using System;
using ASC.CRM.Core;
using ASC.Web.CRM.Controls.Common;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Controls.Tasks;
using ASC.Web.CRM.Resources;
using ASC.Web.Studio.Utility;

#endregion

namespace ASC.Web.CRM
{
    public partial class Tasks : BasePage
    {
        #region Events

        protected override void PageLoad()
        {
            if (String.Compare(UrlParameters.Action, "import", true) == 0)
                ExecImportView();
            else
                ExecListTaskView();
        }

        #endregion

        #region Methods

        protected void ExecImportView()
        {
            var importViewControl = (ImportFromCSVView)LoadControl(ImportFromCSVView.Location);
            importViewControl.EntityType = EntityType.Task;
            CommonContainerHolder.Controls.Add(importViewControl);

            Master.CurrentPageCaption = CRMTaskResource.ImportTasks;
            Title = HeaderStringHelper.GetPageTitle(CRMTaskResource.ImportTasks);
        }

        protected void ExecListTaskView()
        {
            var ctrlListTaskView = (ListTaskView)LoadControl(ListTaskView.Location);
            ctrlListTaskView.CurrentEntityType = EntityType.Contact;
            ctrlListTaskView.EntityID = 0;
            ctrlListTaskView.CurrentContact = null;
            CommonContainerHolder.Controls.Add(ctrlListTaskView);

            Title = HeaderStringHelper.GetPageTitle(Master.CurrentPageCaption ?? CRMTaskResource.Tasks);
        }

        protected void ExecTaskDetailsView(int taskID)
        {
            var task = Global.DaoFactory.GetTaskDao().GetByID(taskID);

            if (!CRMSecurity.CanAccessTo(task))
                Response.Redirect(PathProvider.StartURL());

            Master.CurrentPageCaption = task.Title;

            var closedBy = string.Empty;

            if (task.IsClosed)
                closedBy = string.Format("<div class='crm_taskTitleClosedByPanel'>{0}<div>", CRMTaskResource.ClosedTask);

            Master.CommonContainerHeader = string.Format("{0}{1}", task.Title.HtmlEncode(), closedBy);

            Title = HeaderStringHelper.GetPageTitle(task.Title);
        }

        #endregion
    }
}