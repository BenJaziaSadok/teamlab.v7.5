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
using System.Web;
using ASC.Projects.Engine;

namespace ASC.Web.Projects.Controls.Milestones
{
    public partial class MilestoneAction : BaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            milestoneActionContainer.Options.IsPopup = true;
            Page.RegisterInlineScript("ASC.Projects.MilestoneAction.init();", true);
        }

        public bool IsAdmin()
        {
            return ProjectSecurity.IsAdministrator(Page.Participant.ID);
        }
    }
}
