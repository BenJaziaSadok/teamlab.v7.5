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

namespace ASC.Web.Studio.UserControls.Management
{
    public partial class NamingPeopleSettings : System.Web.UI.UserControl
    {
        public static string Location
        {
            get { return "~/UserControls/Management/NamingPeopleSettings/NamingPeopleSettings.ascx"; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {            
            content.Controls.Add(LoadControl(NamingPeopleSettingsContent.Location));
            Page.RegisterBodyScripts(ResolveUrl("~/usercontrols/Management/NamingPeopleSettings/js/namingpeople.js"));

        }
    }
}