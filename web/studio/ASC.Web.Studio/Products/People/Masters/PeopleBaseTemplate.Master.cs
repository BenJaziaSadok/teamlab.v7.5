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
using System.Text;
using System.Web;
using System.Web.UI;
using ASC.Web.People.UserControls;
using ASC.Web.Studio.UserControls.Users;

namespace ASC.Web.People.Masters
{
    public partial class PeopleBaseTemplate : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            InitScripts();

            _sidepanelHolder.Controls.Add(LoadControl(SideNavigationPanel.Location));

            //UserMaker.AddOnlyOne(Page, ControlHolder);
            ControlHolder.Controls.Add(LoadControl(DepartmentAdd.Location));
            ControlHolder.Controls.Add(new ImportUsersWebControl());
            ControlHolder.Controls.Add(LoadControl(ResendInvitesControl.Location));

            Page.RegisterClientLocalizationScript(typeof(ClientScripts.ClientTemplateResources));
            Page.RegisterClientLocalizationScript(typeof(ClientScripts.ClientLocalizationResources));
        }

        private void InitScripts()
        {
            Page.RegisterStyleControl(LoadControl(VirtualPathUtility.ToAbsolute("~/products/people/masters/Styles.ascx")));
            Page.RegisterBodyScripts(LoadControl(VirtualPathUtility.ToAbsolute("~/products/people/masters/CommonBodyScripts.ascx")));

            var script = new StringBuilder();

            script.Append("jQuery.profile = {};");
            script.Append("jQuery.extend(jQuery.profile, { isAdmin: Teamlab.profile.isAdmin });");
            script.Append("jQuery(document.body).children('form').bind('submit', function() { return false; });");
            Page.RegisterInlineScript(script.ToString());
        }
    }
}