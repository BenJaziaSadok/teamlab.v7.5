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
using System.Globalization;
using AjaxPro;
using ASC.Core;
using ASC.Web.Studio.Core;

namespace ASC.Web.Studio.UserControls.Users
{
    [AjaxNamespace("AjaxPro.UserLangController")]
    public partial class UserLanguage : System.Web.UI.UserControl
    {
        public static string Location { get { return "~/UserControls/Users/UserProfile/UserLanguage.ascx"; } }
        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(this.GetType());
        }

        protected LangData GetCurrentLanguage()
        {
            foreach (var cul in SetupInfo.EnabledCultures)
            {
                if (String.Equals(CultureInfo.CurrentCulture.Name, cul.Name))
                    return new LangData { Name = cul.Name, DisplayName = cul.DisplayName };
            }
            return new LangData { Name = CultureInfo.CurrentCulture.Name, DisplayName = CultureInfo.CurrentCulture.DisplayName };
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public object SaveUserLanguageSettings(string lng)
        {
            try
            {
                var user = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID);

                var changelng = false;
                if (SetupInfo.EnabledCultures.Find(c => String.Equals(c.Name, lng, StringComparison.InvariantCultureIgnoreCase)) != null)
                {
                    if (user.CultureName != lng)
                    {
                        user.CultureName = lng;
                        changelng = true;
                    }
                }
                CoreContext.UserManager.SaveUserInfo(user);

                return new {Status = changelng ? 1 : 2, Message = Resources.Resource.SuccessfullySaveSettingsMessage};
            }
            catch (Exception e)
            {
                return new { Status = 0, Message = e.Message.HtmlEncode() };
            }
        }

        protected class LangData
        {
            public string Name { get; set; }
            public string DisplayName { get; set; }
        }
    }
}