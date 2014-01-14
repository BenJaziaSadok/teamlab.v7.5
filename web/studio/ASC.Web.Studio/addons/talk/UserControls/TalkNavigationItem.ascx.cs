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
using ASC.Core;
using ASC.Data.Storage;
using ASC.Web.Talk.Addon;

namespace ASC.Web.Talk.UserControls
{
    [AjaxPro.AjaxNamespace("TalkProvider")]
    public partial class TalkNavigationItem : System.Web.UI.UserControl
    {
        private static String EscapeJsString(String s)
        {
            return s.Replace("'", "\\'");
        }

        public static string Location
        {
            get { return TalkAddon.BaseVirtualPath + "/UserControls/TalkNavigationItem.ascx"; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(GetType());
        }

        protected string GetTalkClientURL()
        {
            return TalkAddon.GetTalkClientURL();
        }

        protected string GetMessageStr()
        {
            return TalkAddon.GetMessageStr();
        }

        protected string GetOpenContactHandler()
        {
            return VirtualPathUtility.ToAbsolute("~/addons/talk/opencontact.ashx");
        }

        protected string GetJabberClientPath()
        {
            return TalkAddon.GetClientUrl();
        }

        protected string GetUserName()
        {
            try
            {
                return EscapeJsString(CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).UserName.ToLower());
            }
            catch (Exception)
            {
                return String.Empty;
            }
        }

        protected string GetUpdateInterval()
        {
            return new TalkConfiguration().UpdateInterval;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
            Page.RegisterBodyScripts(ResolveUrl("~/addons/talk/js/talk.navigationitem.js"));
            RegisterScript();
        }

        private void RegisterScript()
        {
            var sb = new StringBuilder();

            sb.AppendFormat(@"
                    ASC.Controls.JabberClient.init('{0}','{1}','{2}');
                    ASC.Controls.TalkNavigationItem.init('{3}');",
                GetUserName(),
                GetJabberClientPath(),
                GetOpenContactHandler(),
                GetUpdateInterval()
            );

            Page.RegisterInlineScript(sb.ToString());
        }
    }
}