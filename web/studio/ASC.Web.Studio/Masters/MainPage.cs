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
using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.Web.UI;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Core;
using ASC.Web.Core.Utility;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Core.Notify;
using ASC.Web.Studio.Core.SMS;
using ASC.Web.Studio.Core.Statistic;
using ASC.Web.Studio.Core.Users;
using ASC.Web.Studio.Personal;
using ASC.Web.Studio.UserControls.Statistics;
using ASC.Web.Studio.Utility;
using ASC.Web.Studio.UserControls.FirstTime;
using AjaxPro;
using log4net;
using Constants = ASC.Core.Users.Constants;

namespace ASC.Web.Studio
{
    /// <summary>
    /// Base page for all pages in projects
    /// </summary>
    public class MainPage : Page
    {
        protected static ILog Log
        {
            get { return LogManager.GetLogger("ASC.Web"); }
        }

        protected void Page_PreInit(object sender, EventArgs e)
        {
            ProcessSecureFilter();

            var wizardSettings = SettingsManager.Instance.LoadSettings<WizardSettings>(TenantProvider.CurrentTenantID);
            if (Request["first"] == "1" && !string.IsNullOrEmpty(Request["id"]) && wizardSettings.Completed)
            {
                // wizardSettings.Completed - open source, Request["first"] - cloud
                wizardSettings.Completed = false;
                SettingsManager.Instance.SaveSettings(wizardSettings, TenantProvider.CurrentTenantID);
            }

            var authCookie = Request["id"] ?? CookiesManager.GetCookies(CookiesType.AuthKey);

            if (!wizardSettings.Completed && !(this is confirm))
            {

                var successAuth = SecurityContext.IsAuthenticated;
                if (!successAuth)
                {
                    successAuth = AuthByCookies(authCookie);
                    if (successAuth)
                    {
                        CookiesManager.SetCookies(CookiesType.AuthKey, authCookie);
                    }
                    else
                    {
                        try
                        {
                            authCookie = SecurityContext.AuthenticateMe(UserManagerWrapper.AdminID.ToString(), "admin");
                            successAuth = true;
                        }
                        catch (System.Security.Authentication.InvalidCredentialException) { }
                        catch (System.Security.SecurityException) { }
                    }
                }
                if (!successAuth && !(this is Auth))
                {
                    Response.Redirect("~/auth.aspx");
                }
                if (successAuth && !(this is Wizard))
                {
                    Response.Redirect("~/wizard.aspx");
                }
            }
            else if (!SecurityContext.IsAuthenticated && wizardSettings.Completed && !(this is confirm))
            {
                if (this is Auth && Session["refererURL"] == null && !string.IsNullOrEmpty(Request["id"]))
                {
                    if (AuthByCookies(authCookie))
                    {
                        CookiesManager.SetCookies(CookiesType.AuthKey, authCookie);
                        var first = Request["first"] == "1";
                        if (first)
                        {
                            try
                            {
                                var tenant = CoreContext.TenantManager.GetCurrentTenant(false);
                                tenant.Name = Resources.Resource.StudioWelcomeHeader;
                                CoreContext.TenantManager.SaveTenant(tenant);
                            }
                            catch
                            {
                            }
                        }
                        Response.Redirect(VirtualPathUtility.ToAbsolute("~/") + (first ? "?first=1" : ""));
                        return;
                    }
                }

                //for redirect into one of the projects after creating new tenant
                else if (Session["refererURL"] == null && !string.IsNullOrEmpty(Request["id"]))
                {
                    if (AuthByCookies(authCookie))
                    {
                        CookiesManager.SetCookies(CookiesType.AuthKey, authCookie);
                        try
                        {
                            var tenant = CoreContext.TenantManager.GetCurrentTenant(false);
                            tenant.Name = Resources.Resource.StudioWelcomeHeader;
                            CoreContext.TenantManager.SaveTenant(tenant);
                        }
                        catch
                        {
                            Log.Error("Can't set current tenant in MainPage");
                        }
                        var refererURL = GetRefererUrl();
                        if (String.IsNullOrEmpty(refererURL))
                        {
                            Response.Redirect("~/auth.aspx");
                        }
                        else
                        {
                            var currentUser = CoreContext.UserManager.GetUsers(CoreContext.TenantManager.GetCurrentTenant().OwnerId);
                            if (!currentUser.IsOwner())
                            {
                                Response.Redirect("~/auth.aspx");
                            }
                            SecurityContext.AuthenticateMe(CoreContext.Authentication.GetAccountByID(currentUser.ID));

                            FirstTimeTenantSettings.SetDefaultTenantSettings();
                            FirstTimeTenantSettings.SendInstallInfo(currentUser);

                            Response.Redirect(refererURL);
                        }
                    }
                }

                if (!(this is Auth) && !AutoAuthByCookies() && !ExclusivePage())
                {
                    var refererURL = GetRefererUrl();
                    Session["refererURL"] = refererURL;
                    Response.Redirect("~/auth.aspx");
                    return;
                }
            }

            if (TenantStatisticsProvider.IsNotPaid()
                && !(this is Auth || this is Tariffs || this is confirm))
            {
                Response.Redirect(TenantExtra.GetTariffPageLink(), true);
            }
            else if (SecurityContext.IsAuthenticated
                     && StudioSmsNotificationSettings.IsVisibleSettings
                     && StudioSmsNotificationSettings.Enable
                     && !(this is confirm || this is Auth))
            {
                var user = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID);

                if (!CoreContext.UserManager.IsUserInGroup(SecurityContext.CurrentAccount.ID, Constants.GroupAdmin.ID)
                    && (string.IsNullOrEmpty(user.MobilePhone)
                        || user.MobilePhoneActivationStatus == MobilePhoneActivationStatus.NotActivated))
                {
                    Response.Redirect(StudioNotifyService.GenerateConfirmUrl(user.Email, ConfirmType.PhoneActivation));
                }
            }

            //check disable and public 
            var webitem = CommonLinkUtility.GetWebItemByUrl(Request.Url.ToString());
            var parentIsDisabled = false;
            if (webitem != null && webitem.IsSubItem())
            {
                var parentItemID = WebItemManager.Instance.GetParentItemID(webitem.ID);
                parentIsDisabled = WebItemManager.Instance[parentItemID].IsDisabled();
            }

            if (webitem != null && (webitem.IsDisabled() || parentIsDisabled) && !ExclusivePage())
            {
                if (webitem.ID == new Guid("{F4D98AFD-D336-4332-8778-3C6945C81EA0}")
                    && string.Equals(GetType().BaseType.FullName, "ASC.Web.People.Profile"))
                {
                    Response.Redirect("~/my.aspx");
                    return;
                }

                Response.Redirect("~/");
                return;
            }

            if (SecurityContext.IsAuthenticated)
            {
                try
                {
                    StatisticManager.SaveUserVisit(TenantProvider.CurrentTenantID, SecurityContext.CurrentAccount.ID, CommonLinkUtility.GetProductID());
                }
                catch (Exception exc)
                {
                    Log.Error("failed save user visit", exc);
                }
            }

            PersonalHelper.TransferRequest(this);
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            InitInlineScript();
        }

        private void ProcessSecureFilter()
        {
            var filter = SetupInfo.SecureFilter;
            if (HttpContext.Current == null) return;

            //ssl enable only on subdomain of basedomain
            if (HttpContext.Current.Request.GetUrlRewriter().Host.EndsWith("." + SetupInfo.BaseDomain))
                SecureFilter.GetInstance(filter).ProcessRequest(Request.GetUrlRewriter(), SetupInfo.SslPort, SetupInfo.HttpPort);
        }

        private static bool AutoAuthByCookies()
        {
            return AuthByCookies(CookiesManager.GetCookies(CookiesType.AuthKey));
        }

        private static bool AuthByCookies(string cookiesKey)
        {
            if (string.IsNullOrEmpty(cookiesKey)) return false;

            try
            {
                if (SecurityContext.AuthenticateMe(cookiesKey)) return true;
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("AutoAuthByCookies Error {0}", ex);
            }

            return false;
        }

        private string GetRefererUrl()
        {
            var refererURL = Request.Url.AbsoluteUri;
            if (String.IsNullOrEmpty(refererURL)
                || refererURL.IndexOf("Subgurim_FileUploader", StringComparison.InvariantCultureIgnoreCase) != -1
                || (this is _Default)
                || (this is ServerError)
                )
                refererURL = (string)Session["refererURL"];

            return refererURL;
        }

        private bool ExclusivePage()
        {
            var baseType = GetType().BaseType;
            if (baseType == null) return false;
            var typeName = baseType.FullName;
            if (string.Equals(typeName, "ASC.Web.Files.DocEditor"))
                return !string.IsNullOrEmpty(Request[CommonLinkUtility.DocShareKey]) || CoreContext.Configuration.YourDocsDemo;

            return false;
        }

        private void InitInlineScript()
        {
            var scripts = HttpContext.Current.Items[Constant.AjaxID + ".pagescripts"] as ListDictionary;

            if (scripts == null) return;

            var sb = new StringBuilder();

            foreach (var key in scripts.Keys)
            {
                sb.Append(scripts[key]);
            }

            this.RegisterInlineScript(sb.ToString(), onReady: false);
        }
    }
}