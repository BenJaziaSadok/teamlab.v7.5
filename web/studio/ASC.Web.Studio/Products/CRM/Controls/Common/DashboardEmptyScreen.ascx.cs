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

namespace ASC.Web.CRM.Controls.Common
{
    public partial class DashboardEmptyScreen : System.Web.UI.UserControl
    {
        public static String Location
        {
            get
            {
                return PathProvider.GetFileStaticRelativePath("Common/DashboardEmptyScreen.ascx");
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.RegisterBodyScripts(VirtualPathUtility.ToAbsolute("~/js/asc/core/decoder.js"));

            Page.RegisterInlineScript(@"jq(function() {
                jq('#content .close').on('click', function () {
                    jq('[blank-page]').remove();
                });      
            });");
        }
    }
}