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

using System.Text;
using System.Web;
using ASC.Web.Projects.Classes;
using ASC.Web.Studio.Utility;
using ASC.Web.Projects.Resources;
using ASC.Projects.Engine;

namespace ASC.Web.Projects
{
    public partial class GanttChart : BasePage
    {
        protected override void PageLoad()
        {
            if (string.IsNullOrEmpty(Request["prjID"]) || !ProjectSecurity.CanRead(Project))
            {
                Response.Redirect(PathProvider.BaseVirtualPath, true);
            }

            if (!ProjectSecurity.CanReadGantt(Project) || Request.Browser.IsMobileDevice)
            {
                Response.Redirect("tasks.aspx?prjID=" + Project.ID);
            }

            Master.Master.DisabledHelpTour = true;
            Master.DisabledSidePanel = true;
            Master.DisabledPrjNavPanel = true;

            _hintPopupTaskRemove.Options.IsPopup = true;
            _hintPopupMilestoneRemove.Options.IsPopup = true;
            _hintPopupMilestoneTasks.Options.IsPopup = true;
            _hintPopupTaskWithSubtasks.Options.IsPopup = true;
            _newLinkError.Options.IsPopup = true;
            _moveTaskOutMilestone.Options.IsPopup = true;
            _addNewLinkPopup.Options.IsPopup = true;

            Title = HeaderStringHelper.GetPageTitle(ProjectResource.GanttGart);
        }

        protected string RenderStyles()
        {
            var sb = new StringBuilder();

            sb.AppendFormat("<link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\"/>",
                            VirtualPathUtility.ToAbsolute("~/products/projects/app_themes/default/css/ganttchart.css"));
            return sb.ToString();
        }
    }
}