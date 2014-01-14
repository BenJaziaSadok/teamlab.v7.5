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
using System.Threading;
using System.Web;
using ASC.Core;
using ASC.Web.Core;

namespace ASC.Web.Studio.Core
{
    public class WebStudioCommonModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.AcquireRequestState += AcquireRequestState;
            context.BeginRequest += BeginRequest;
            context.EndRequest += EndRequest;
        }

        public void Dispose()
        {
        }


        private void AcquireRequestState(object sender, EventArgs e)
        {
            Authenticate();
            ResolveUserCulture();
        }

        private void BeginRequest(object sender, EventArgs e)
        {
            if (HttpContext.Current != null && !RestrictRewriter(HttpContext.Current.Request.Url))
            {
                HttpContext.Current.PushRewritenUri();
            }
        }

        private void EndRequest(object sender, EventArgs e)
        {
            if (HttpContext.Current != null && !RestrictRewriter(HttpContext.Current.Request.Url))
            {
                HttpContext.Current.PopRewritenUri();
            }
        }


        public static bool Authenticate()
        {
            var tenant = CoreContext.TenantManager.GetCurrentTenant(false);
            if (tenant != null && !SecurityContext.IsAuthenticated)
            {
                var cookie = CookiesManager.GetCookies(CookiesType.AuthKey);
                if (!string.IsNullOrEmpty(cookie))
                {
                    return SecurityContext.AuthenticateMe(cookie);
                }
            }
            return false;
        }

        public static void ResolveUserCulture()
        {
            CultureInfo culture = null;
            
            var tenant = CoreContext.TenantManager.GetCurrentTenant(false);
            if (tenant != null)
            {
                culture = tenant.GetCulture();
            }
            
            var user = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID);
            if (!string.IsNullOrEmpty(user.CultureName))
            {
                culture = CultureInfo.GetCultureInfo(user.CultureName);
            }

            if (culture != null && Thread.CurrentThread.CurrentCulture != culture)
            {
                Thread.CurrentThread.CurrentCulture = culture;
            }
            if (culture != null && Thread.CurrentThread.CurrentUICulture != culture)
            {
                Thread.CurrentThread.CurrentUICulture = culture;
            }
        }


        private bool RestrictRewriter(Uri uri)
        {
            return uri.AbsolutePath.IndexOf(".svc", StringComparison.OrdinalIgnoreCase) != -1;
        }
    }
}