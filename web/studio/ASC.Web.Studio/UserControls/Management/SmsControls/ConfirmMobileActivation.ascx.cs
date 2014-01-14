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

using AjaxPro;
using ASC.Core;
using ASC.Core.Users;
using ASC.Geolocation;
using ASC.Web.Core.Client.Bundling;
using ASC.Web.Core.Security;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Core.SMS;
using System;
using System.Web;
using System.Web.UI;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Studio.UserControls.Management
{
    [AjaxNamespace("AjaxPro.MobileActivationController")]
    public partial class ConfirmMobileActivation : UserControl
    {
        public static string Location
        {
            get { return "~/UserControls/Management/SmsControls/ConfirmMobileActivation.ascx"; }
        }

        public UserInfo User;
        public bool Activation;

        protected string Country = "US";

        protected override void OnPreRender(EventArgs e)
        {
            if (Activation) return;
            if (SecurityContext.IsAuthenticated) Response.Redirect(GetRefererURL());
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (SecurityContext.IsAuthenticated && User.ID != SecurityContext.CurrentAccount.ID)
            {
                Response.Redirect(GetRefererURL());
                return;
            }

            if (!CoreContext.Configuration.YourDocs)
            {
                _communitations.Controls.Add(LoadControl(AuthCommunications.Location));
            }

            AjaxPro.Utility.RegisterTypeForAjax(GetType());

            Page.RegisterBodyScripts(ResolveUrl("~/usercontrols/Management/SmsControls/js/confirmmobile.js"));
            Page.RegisterStyleControl(VirtualPathUtility.ToAbsolute("~/usercontrols/management/SmsControls/css/confirmmobile.less"));

            Context.Session["SmsAuthData"] = User.ID;

            if (string.IsNullOrEmpty(User.MobilePhone))
                Activation = true;

            if (!Activation)
            {
                try
                {
                    SmsManager.PutAuthCode(User, false);
                }
                catch (Exception)
                {
                    Activation = true;
                }
            }

            if (Activation)
            {
                var ipGeolocationInfo = new GeolocationHelper("db").GetIPGeolocationFromHttpContext();
                if (ipGeolocationInfo != null) Country = ipGeolocationInfo.Key;

                var clientScriptReference = new ClientScriptReference();
                clientScriptReference.Includes.Add(typeof(CountriesResources));
                Page.RegisterBodyScripts(clientScriptReference);

                Page.RegisterBodyScripts(ResolveUrl("~/js/asc/plugins/countries.js"));
                Page.RegisterBodyScripts(ResolveUrl("~/js/asc/plugins/phonecontroller.js"));
                Page.RegisterStyleControl(VirtualPathUtility.ToAbsolute("~/skins/default/phonecontroller.css"));
            }
        }

        private UserInfo GetUser()
        {
            return CoreContext.UserManager.GetUsers(
                SecurityContext.IsAuthenticated
                    ? SecurityContext.CurrentAccount.ID
                    : new Guid(Context.Session["SmsAuthData"].ToString()));
        }

        private string GetRefererURL()
        {
            var refererURL = (string)Context.Session["refererURL"];
            if (String.IsNullOrEmpty(refererURL))
                refererURL = CommonLinkUtility.GetDefault();

            Context.Session["refererURL"] = null;
            Context.Session["SmsAuthData"] = null;
            return refererURL;
        }

        #region AjaxMethod

        [SecurityPassthrough]
        [AjaxMethod(HttpSessionStateRequirement.Read)]
        public object SaveMobilePhone(string mobilePhone)
        {
            var user = GetUser();
            mobilePhone = SmsManager.SaveMobilePhone(user, mobilePhone);

            var mustConfirm = StudioSmsNotificationSettings.Enable;

            return
                new
                    {
                        phoneNoise = SmsManager.BuildPhoneNoise(mobilePhone),
                        confirm = mustConfirm,
                        RefererURL = mustConfirm ? string.Empty : GetRefererURL()
                    };
        }

        [SecurityPassthrough]
        [AjaxMethod(HttpSessionStateRequirement.Read)]
        public object SendSmsCodeAgain()
        {
            var user = GetUser();
            SmsManager.PutAuthCode(user, true);

            return
                new
                    {
                        phoneNoise = SmsManager.BuildPhoneNoise(user.MobilePhone),
                        confirm = true,
                    };
        }

        [SecurityPassthrough]
        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public object ValidateSmsCode(string code)
        {
            var user = GetUser();

            SmsManager.ValidateSmsCode(user, code);

            return new { RefererURL = GetRefererURL() };
        }

        #endregion
    }
}