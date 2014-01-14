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
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using ASC.Thrdparty;
using ASC.Thrdparty.Configuration;
using AjaxPro;
using ASC.Core;
using ASC.FederatedLogin;
using ASC.FederatedLogin.Profile;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Studio.UserControls.Users.UserProfile
{
    [AjaxNamespace("AccountLinkControl")]
    public partial class AccountLinkControl : UserControl
    {
        public static string Location
        {
            get { return "~/UserControls/Users/UserProfile/AccountLinkControl.ascx"; }
        }

        public static bool IsNotEmpty
        {
            get
            {
                return
                    !string.IsNullOrEmpty(KeyStorage.Get("googleConsumerKey"))
                    || !string.IsNullOrEmpty(KeyStorage.Get("facebookAppID"))
                    || !string.IsNullOrEmpty(KeyStorage.Get("twitterKey"))
                    || !string.IsNullOrEmpty(KeyStorage.Get("linkedInKey"));
            }
        }

        public bool SettingsView { get; set; }
        public bool InviteView { get; set; }

        protected ICollection<AccountInfo> Infos = new List<AccountInfo>();

        public AccountLinkControl()
        {
            ClientCallback = "loginCallback";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(GetType());
            Page.RegisterStyleControl(VirtualPathUtility.ToAbsolute("~/usercontrols/users/userprofile/css/accountlink_style.less"));
            Page.RegisterBodyScripts(ResolveUrl("~/usercontrols/users/userprofile/js/accountlinker.js"));
            InitProviders();
        }

        public string ClientCallback { get; set; }

        private void InitProviders()
        {
            IEnumerable<LoginProfile> linkedAccounts = new List<LoginProfile>();
            var realmUrl = string.Empty;

            if (SecurityContext.IsAuthenticated)
            {
                linkedAccounts = GetLinker().GetLinkedProfiles(SecurityContext.CurrentAccount.ID.ToString());
            }

            var haveOpenIdAccount = linkedAccounts.Any(x => x.Provider == ProviderConstants.OpenId);

            if (SettingsView && !haveOpenIdAccount)
            {
                realmUrl = ConfigurationManager.AppSettings["openid-realm-url"] ?? string.Empty;
            }
            else if (InviteView && haveOpenIdAccount)
            {
                var accountWithRealm = linkedAccounts.FirstOrDefault(x => x.Provider == ProviderConstants.OpenId && !String.IsNullOrEmpty(x.RealmUrl));

                if (accountWithRealm != default(LoginProfile))
                    realmUrl = accountWithRealm.RealmUrl;
            }

            if (!string.IsNullOrEmpty(KeyStorage.Get("googleConsumerKey")))
                AddProvider(ProviderConstants.OpenId, "https://www.google.com/accounts/o8/id", realmUrl, linkedAccounts);

            if (!string.IsNullOrEmpty(KeyStorage.Get("facebookAppID")))
                AddProvider(ProviderConstants.Facebook, linkedAccounts);

            if (!string.IsNullOrEmpty(KeyStorage.Get("twitterKey")))
                AddProvider(ProviderConstants.Twitter, linkedAccounts);

            if (!string.IsNullOrEmpty(KeyStorage.Get("linkedInKey")))
                AddProvider(ProviderConstants.LinkedIn, linkedAccounts);
        }

        private void AddProvider(string provider, IEnumerable<LoginProfile> linkedAccounts)
        {
            AddProvider(provider, null, null, linkedAccounts);
        }

        private void AddProvider(string provider, string oid, string realmUrl, IEnumerable<LoginProfile> linkedAccounts)
        {
            Infos.Add(new AccountInfo
                {
                    Linked = linkedAccounts.Any(x => x.Provider == provider),
                    Provider = provider,
                    Url = VirtualPathUtility.ToAbsolute("~/login.ashx")
                          + string.Format("?auth={0}&mode=popup&callback={1}"
                                          + (string.IsNullOrEmpty(oid) ? "" : ("&oid=" + HttpUtility.UrlEncode(oid)))
                                          + (string.IsNullOrEmpty(realmUrl) ? "" : ("&realmUrl=" + HttpUtility.UrlEncode(realmUrl))),
                                          provider, ClientCallback)
                });
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse LinkAccount(string serializedProfile)
        {
            //Link it
            var profile = new LoginProfile(serializedProfile);
            GetLinker().AddLink(SecurityContext.CurrentAccount.ID.ToString(), profile);
            return RenderControlHtml();
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse UnlinkAccount(string provider)
        {
            //Link it
            GetLinker().RemoveProvider(SecurityContext.CurrentAccount.ID.ToString(), provider);
            return RenderControlHtml();
        }

        private AjaxResponse RenderControlHtml()
        {
            using (var stringWriter = new StringWriter())
            using (var writer = new HtmlTextWriter(stringWriter))
            {
                var ctrl = (AccountLinkControl) LoadControl(Location);
                ctrl.SettingsView = true;
                ctrl.InitProviders();
                ctrl.RenderControl(writer);
                return new AjaxResponse {rs1 = stringWriter.GetStringBuilder().ToString()};
            }
        }

        public AccountLinker GetLinker()
        {
            return new AccountLinker("webstudio");
        }

        public IEnumerable<AccountInfo> GetLinkableProviders()
        {
            return Infos.Where(x => !(x.Provider.ToLower() == "twitter" || x.Provider.ToLower() == "linkedin"));
        }
    }

    public class AccountInfo
    {
        public string Provider { get; set; }
        public string Url { get; set; }
        public bool Linked { get; set; }
        public string Class { get; set; }
    }
}