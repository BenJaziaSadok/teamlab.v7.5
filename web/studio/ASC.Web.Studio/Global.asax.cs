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
using System.Configuration;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Security;
using ASC.Web.Core.Client;
using AjaxPro.Security;
using ASC.Common.Data;
using ASC.Core;
using ASC.Core.Configuration;
using ASC.Core.Tenants;
using ASC.Data.Storage.S3;
using ASC.Web.Core;
using ASC.Web.Core.Client.Bundling;
using ASC.Web.Core.Helpers;
using ASC.Web.Core.Security;
using ASC.Web.Core.Utility;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Core.Notify;
using ASC.Web.Studio.Core.SearchHandlers;
using ASC.Web.Studio.Utility;
using log4net.Config;
using TMResourceData;

namespace ASC.Web.Studio
{
    public class Global : HttpApplication
    {
        private static readonly object locker = new object();
        private static volatile bool applicationStarted = false;


        protected void Application_Start(object sender, EventArgs e)
        {
            // use Application_StartDelayed instead.
            // in Application_Start HttpContext.Current.Request is not available
            // http://mvolo.com/iis7-integrated-mode-request-is-not-available-in-this-context-exception-in-applicationstart
        }

        public override string GetVaryByCustomString(HttpContext context, string custom)
        {
            if (String.IsNullOrEmpty(custom)) return base.GetVaryByCustomString(context, custom);

            var result = String.Empty;

            foreach (Match match in Regex.Matches(custom.ToLower(), "(?<key>\\w+)(\\[(?<subkey>\\w+)\\])?;?", RegexOptions.Compiled))
            {
                var key = String.Empty;
                var subkey = String.Empty;

                if (match.Groups["key"].Success)
                    key = match.Groups["key"].Value;

                if (match.Groups["subkey"].Success)
                    subkey = match.Groups["subkey"].Value;

                String value = String.Empty;

                switch (key)
                {
                    case "relativeurl":
                        var appUrl = VirtualPathUtility.ToAbsolute("~/");
                        value = Request.GetUrlRewriter().AbsolutePath.Remove(0, appUrl.Length - 1);
                        break;
                    case "url":
                        value = Request.GetUrlRewriter().AbsoluteUri.ToString();
                        break;
                    case "page":
                        value = Request.GetUrlRewriter().AbsolutePath.Substring(Request.Url.AbsolutePath.LastIndexOfAny(new[] { '/', '\\' }) + 1);
                        break;
                    case "module":
                        var module = "common";
                        var matches = Regex.Match(Request.Url.AbsolutePath.ToLower(), "(products|addons)/(\\w+)/?", RegexOptions.Compiled);

                        if (matches.Success && matches.Groups.Count > 2 && matches.Groups[2].Success)
                            module = matches.Groups[2].Value;

                        value = module;

                        break;
                    case "culture":
                        value = Thread.CurrentThread.CurrentUICulture.Name;
                        break;
                    case "theme":
                        value = ColorThemesSettings.GetColorThemesSettings();
                        break;
                }

                if (!(System.String.Compare(value.ToLower(), subkey, System.StringComparison.Ordinal) == 0 ||
                         String.IsNullOrEmpty(subkey))) break;

                result += "|" + value;

            }

            if (!string.IsNullOrEmpty(result)) return String.Join("|", ClientSettings.ResetCacheKey, result);

            return base.GetVaryByCustomString(context, custom);
        }

        private void Application_StartDelayed(object sender, EventArgs e)
        {
            XmlConfigurator.Configure();
            DbRegistry.Configure();
            InitializeDbResources();
            AjaxSecurityChecker.Instance.CheckMethodPermissions += AjaxCheckMethodPermissions;
            try
            {
                AmiPublicDnsSyncService.Synchronize();
            }
            catch { }
            NotifyConfiguration.Configure();
            WebItemManager.Instance.LoadItems();
            SearchHandlerManager.Registry(new StudioSearchHandler());
            (new S3UploadGuard()).DeleteExpiredUploads(TimeSpan.FromDays(1));
            BundleConfig.Configure();
        }

        protected void Session_End(object sender, EventArgs e)
        {
            CommonControlsConfigurer.FCKClearTempStore(Session);
        }


        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (!applicationStarted)
            {
                lock (locker)
                {
                    if (!applicationStarted)
                    {
                        applicationStarted = true;
                        Application_StartDelayed(sender, e);
                    }
                }
            }

            var currentTenant = CoreContext.TenantManager.GetCurrentTenant(false);
            if (currentTenant == null)
            {
                var redirectUrl = String.Format("{0}?url={1}", SetupInfo.NoTenantRedirectURL, Request.Url.Host);
                Response.Redirect(redirectUrl, true);
            }
            else if (currentTenant.Status != TenantStatus.Active)
            {
                var ind = Request.Url.AbsoluteUri.IndexOf(VirtualPathUtility.ToAbsolute("~/confirm.aspx"), StringComparison.InvariantCultureIgnoreCase);
                if (currentTenant.Status == TenantStatus.Transfering)
                {
                    var errorRegexPattern = ConfigurationManager.AppSettings["web.transfering.errorstatus-regex"];
                    if (!string.IsNullOrEmpty(errorRegexPattern) && Regex.IsMatch(Request.Url.AbsoluteUri, errorRegexPattern))
                    {
                        Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                        Response.End();
                    }
                    else
                    {
                        Response.Redirect(SetupInfo.TenantTransferingRedirectURL, true);
                    }
                }
                else if (currentTenant.Status == TenantStatus.RemovePending || !(ind >= 0 && currentTenant.Status == TenantStatus.Suspended))
                {
                    var redirectUrl = String.Format("{0}?url={1}", SetupInfo.NoTenantRedirectURL, Request.Url.Host);
                    Response.Redirect(redirectUrl, true);
                }
            }

            if (!CheckBasicAuth(((HttpApplication)sender).Context))
            {
                WebStudioCommonModule.Authenticate();
            }
            WebStudioCommonModule.ResolveUserCulture();
            FixFlashPlayerCookieBug();
        }

        private void InitializeDbResources()
        {
            if (ConfigurationManager.AppSettings["resources.from-db"] == "true")
            {
                AssemblyWork.UploadResourceData(AppDomain.CurrentDomain.GetAssemblies());
                AppDomain.CurrentDomain.AssemblyLoad += (sender, args) => AssemblyWork.UploadResourceData(AppDomain.CurrentDomain.GetAssemblies());
            }
        }

        /// <summary>
        /// Fix for the Flash Player Cookie bug in Non-IE browsers.
        /// Since Flash Player always sends the IE cookies even in FireFox we have to bypass the cookies by sending
        /// the values as part of the POST or GET and overwrite the cookies with the passed in values.
        /// The theory is that at this point (BeginRequest) the cookies have not been read by
        /// the Session and Authentication logic and if we update the cookies here we'll get our
        /// Session and Authentication restored correctly.
        /// </summary>
        private void FixFlashPlayerCookieBug()
        {
            try
            {
                if (HttpContext.Current.Request.Form["ASPSESSID"] != null)
                {
                    UpdateCookie("ASP.NET_SESSIONID", HttpContext.Current.Request.Form["ASPSESSID"]);
                }
                else if (HttpContext.Current.Request.QueryString["ASPSESSID"] != null)
                {
                    UpdateCookie("ASP.NET_SESSIONID", HttpContext.Current.Request.QueryString["ASPSESSID"]);
                }
            }
            catch (Exception)
            {
                Response.StatusCode = 500;
                Response.Write("Error Initializing Session");
            }
            try
            {
                if (HttpContext.Current.Request.Form["AUTHID"] != null)
                {
                    UpdateCookie(FormsAuthentication.FormsCookieName, HttpContext.Current.Request.Form["AUTHID"]);
                }
                else if (HttpContext.Current.Request.QueryString["AUTHID"] != null)
                {
                    UpdateCookie(FormsAuthentication.FormsCookieName, HttpContext.Current.Request.QueryString["AUTHID"]);
                }

            }
            catch (Exception)
            {
                Response.StatusCode = 500;
                Response.Write("Error Initializing Forms Authentication");
            }
        }

        private bool CheckBasicAuth(HttpContext context)
        {
            string authCookie;
            if (AuthorizationHelper.ProcessBasicAuthorization(context, out authCookie))
            {
                CookiesManager.SetCookies(CookiesType.AuthKey, authCookie);
                return true;
            }
            return false;
        }

        private bool AjaxCheckMethodPermissions(MethodInfo method)
        {
            var authorized = SecurityContext.IsAuthenticated;
            if (!authorized && HttpContext.Current != null)
            {
                authorized = method.GetCustomAttributes(typeof(SecurityAttribute), true)
                    .Cast<SecurityAttribute>()
                    .Any(a => a.CheckAuthorization(HttpContext.Current));
                if (!authorized)
                {
                    authorized = WebStudioCommonModule.Authenticate();
                }
            }
            return authorized;
        }

        private void UpdateCookie(string name, string value)
        {
            var cookie = HttpContext.Current.Request.Cookies.Get(name);
            if (cookie == null)
            {
                cookie = new HttpCookie(name);
                HttpContext.Current.Request.Cookies.Add(cookie);
            }
            cookie.Value = value;
            HttpContext.Current.Request.Cookies.Set(cookie);
        }
    }
}