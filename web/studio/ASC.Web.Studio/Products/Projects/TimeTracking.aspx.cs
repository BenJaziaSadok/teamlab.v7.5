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

#region Usings

using System;
using System.Globalization;
using System.Web;
using ASC.Projects.Core.Domain;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Configuration;
using ASC.Web.Projects.Resources;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;
using ASC.Projects.Engine;
using ASC.Core;

#endregion

namespace ASC.Web.Projects
{
    public partial class TimeTracking : BasePage
    {
        #region Properties

        public bool ShowMultiActionPanel { get; private set; }

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

        protected bool CanCreate { get; set; }

        #endregion

        #region Events

        protected override void PageLoad()
        {
            _popupTimerRemove.Options.IsPopup = true;

            ShowMultiActionPanel = Participant.IsAdmin;

            if (Project != null && !ShowMultiActionPanel)
            {
                ShowMultiActionPanel = Guid.Equals(SecurityContext.CurrentAccount.ID, Project.Responsible);
            }

            if (Participant.IsVisitor)
            {
                Response.Redirect(PathProvider.BaseVirtualPath, true);
            }

            CanCreate = RequestContext.CanCreateTime();

            if (TaskID <= 0)
            {
                _emptyScreens.Controls.Add(Masters.BasicTemplate.RenderEmptyScreenForFilter(TimeTrackingResource.NoTimersFilter, TimeTrackingResource.DescrEmptyListTimersFilter));
            }

            var emptyScreenControl = new Studio.Controls.Common.EmptyScreenControl
                {
                    ImgSrc = WebImageSupplier.GetAbsoluteWebPath("empty_screen_time_tracking.png", ProductEntryPoint.ID),
                    Header = TimeTrackingResource.NoTtimers,
                    Describe = String.Format(TimeTrackingResource.NoTimersNote),
                    ID = "emptyScreenForTimers"
                };

            if (CanCreate)
                emptyScreenControl.ButtonHTML = String.Format("<span class='baseLinkAction addFirstElement'>{0}</span>", TimeTrackingResource.StartTimer);

            _emptyScreens.Controls.Add(emptyScreenControl);

            Page.RegisterInlineScript(@"
                                       ASC.Projects.TimeTrakingEdit.init();" +
                                      "ASC.Projects.TimeSpendActionPage.init();", true);

            Title = HeaderStringHelper.GetPageTitle(ProjectsCommonResource.TimeTracking);
        }

        #endregion

    }
}