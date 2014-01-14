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
using System.Linq;
using System.Web;

using ASC.Projects.Engine;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Configuration;
using ASC.Web.Projects.Resources;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.Utility;
using ASC.Web.Core.Utility.Skins;

namespace ASC.Web.Projects
{
    public partial class Milestones : BasePage
    {
        protected override string CookieKeyForPagination
        {
            get
            {
                return "milestonesKeyForPagination";
            }
        }

        protected int AllMilestonesCount { get; set; }

        protected bool IsAdmin { get; set; }

        protected override void PageLoad()
        {
            IsAdmin = ProjectSecurity.IsAdministrator(Participant.ID);

            Bootstrap();

            LoadControls();

            Page.RegisterInlineScript(@"
                    if (location.href.indexOf('milestones.aspx') > 0) {
                        ASC.Projects.AllMilestones.init(" + EntryCountOnPage + ",'" + CookieKeyForPagination + "', " + Global.VisiblePageCount + ");}", true);
        }

        private void Bootstrap()
        {
            _hintPopupTasks.Options.IsPopup = true;
            _hintPopupTaskRemove.Options.IsPopup = true;

            Title = HeaderStringHelper.GetPageTitle(MilestoneResource.Milestones);
        }

        private void LoadControls()
        {
            AllMilestonesCount = RequestContext.IsInConcreteProject
                ? Global.EngineFactory.GetMilestoneEngine().GetByProject(RequestContext.GetCurrentProjectId()).Count()
                : Global.EngineFactory.GetMilestoneEngine().GetAll().Count();

            RenderEmptyScreens();
        }

        private void RenderEmptyScreens()
        {
            var emptyScreenControl = new EmptyScreenControl
            {
                ImgSrc = WebImageSupplier.GetAbsoluteWebPath("empty_screen_milestones.png", ProductEntryPoint.ID),
                Header = MilestoneResource.MilestoneNotFound_Header,
                Describe = String.Format(MilestoneResource.MilestonesMarkMajorTimestamps),
                ID = "emptyListMilestone" 
            };

            if (RequestContext.CanCreateMilestone(true))
                emptyScreenControl.ButtonHTML = String.Format("<a class='baseLinkAction addFirstElement'>{0}</a>", MilestoneResource.PlanFirstMilestone);

            _emptyScreenPlaceHolder.Controls.Add(emptyScreenControl);

            _emptyScreenPlaceHolder.Controls.Add(Masters.BasicTemplate.RenderEmptyScreenForFilter(MilestoneResource.FilterNoMilestones, MilestoneResource.DescrEmptyListMilFilter));
        }
    }
}
