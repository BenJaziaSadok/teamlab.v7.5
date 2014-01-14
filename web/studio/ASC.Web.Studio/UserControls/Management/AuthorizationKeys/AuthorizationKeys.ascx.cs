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
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.UI;
using ASC.Core;
using ASC.Data.Storage;
using ASC.Thrdparty.Configuration;
using ASC.Web.Studio.Core;

namespace ASC.Web.Studio.UserControls.Management
{
    [AjaxPro.AjaxNamespace("AuthorizationKeys")]
    public partial class AuthorizationKeys : UserControl
    {
        public static string Location
        {
            get { return "~/UserControls/Management/AuthorizationKeys/AuthorizationKeys.ascx"; }
        }

        private List<AuthService> _authServiceList;
        protected List<AuthService> AuthServiceList
        {
            get { return _authServiceList ?? (_authServiceList = GetAuthServices().ToList()); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(GetType(), Page);
            Page.RegisterBodyScripts(ResolveUrl("~/usercontrols/management/AuthorizationKeys/js/authorizationkeys.js"));
            Page.ClientScript.RegisterClientScriptBlock(GetType(), "authorizationkeys_style", "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + WebPath.GetPath("usercontrols/management/authorizationkeys/css/authorizationkeys.css") + "\">", false);
        }

        private static IEnumerable<AuthService> GetAuthServices()
        {
            return from KeyElement keyElement in ConsumerConfigurationSection.GetSection().Keys
                   where keyElement.Type != KeyElement.KeyType.Default && !string.IsNullOrEmpty(keyElement.ConsumerName)
                   group keyElement by keyElement.ConsumerName
                   into keyGroup
                   let consumerKey = keyGroup.FirstOrDefault(key => key.Type == KeyElement.KeyType.Key)
                   let consumerSecret = keyGroup.FirstOrDefault(key => key.Type == KeyElement.KeyType.Secret)
                   select ToAuthService(keyGroup.Key, consumerKey, consumerSecret);
        }

        private static AuthService ToAuthService(string consumerName, KeyElement consumerKey, KeyElement consumerSecret)
        {
            var authService = new AuthService(consumerName);
            if (consumerKey != null) 
                authService.WithId(consumerKey.Name, consumerKey.Value);
            if (consumerSecret != null) 
                authService.WithKey(consumerSecret.Name, consumerSecret.Value);
            return authService;
        }

        #region Ajax

        [AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.ReadWrite)]
        public void SaveAuthKeys(List<AuthKey> authKeys)
        {
            SecurityContext.DemandPermissions(SecutiryConstants.EditPortalSettings);
            var config = WebConfigurationManager.OpenWebConfiguration(HostingEnvironment.ApplicationVirtualPath);
            var consumersSection = (ConsumerConfigurationSection)config.GetSection(ConsumerConfigurationSection.SectionName);

            //todo: keys to db
            foreach (var authKey in authKeys)
            {
                KeyElement consumersKey = consumersSection.Keys.GetKey(authKey.Name);
                if (consumersKey != null && consumersKey.Value != authKey.Value)
                {
                    consumersKey.Value = authKey.Value;
                }
            }

            config.Save(ConfigurationSaveMode.Modified);
        }

        #endregion
    }
}