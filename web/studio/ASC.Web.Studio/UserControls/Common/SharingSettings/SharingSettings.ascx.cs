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
using System.Web.UI;
using ASC.Data.Storage;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.UserControls.Users;
using System.Web.UI.HtmlControls;
using System.Web;
using ASC.Core;
using System.Text;

namespace ASC.Web.Studio.UserControls.Common
{
    public partial class SharingSettings : UserControl
    {
        public bool EnableShareMessage;
        public bool IsPopup = true;

        public static string Location
        {
            get { return "~/UserControls/Common/SharingSettings/SharingSettings.ascx"; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.RegisterBodyScripts(VirtualPathUtility.ToAbsolute("~/usercontrols/common/sharingsettings/js/sharingsettings.js"));
            Page.RegisterStyleControl(VirtualPathUtility.ToAbsolute("~/usercontrols/common/sharingsettings/css/sharingsettings.less"));

            _sharingDialogContainer.Options.IsPopup = IsPopup;
            _sharingDialogContainer.Header.Visible = IsPopup;

            shareUserSelector.IsLinkView = true;
            shareUserSelector.LinkText = Core.Users.CustomNamingPeople.Substitute<Resources.Resource>("AddUsersForSharingButton");

            RegisterScript();
        }

        private void RegisterScript()
        {
            var sb = new StringBuilder();

            sb.AppendFormat(@"
                    jq.tmpl('groupSelectorTemplate', {{selectorID: 'shareGroupSelector', linkText: '{0}'}})
                    .appendTo('#sharingSettingsDialogBody .add-to-sharing-links');                    

                    window.shareGroupSelector = new ASC.Controls.GroupSelector('shareGroupSelector', jq.browser.mobile, true, true);",
                            Core.Users.CustomNamingPeople.Substitute<Resources.Resource>("AddGroupsForSharingButton").HtmlEncode()
                );

            Page.RegisterInlineScript(sb.ToString());
        }
    }
}