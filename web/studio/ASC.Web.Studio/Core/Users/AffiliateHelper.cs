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
using System.IO;
using System.Net;
using System.Web.Configuration;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Studio.Core.Users
{
    public class AffiliateHelper
    {
        public static string JoinAffilliateLink
        {
            get { return WebConfigurationManager.AppSettings["web.affiliates.link"]; }

        }

        private static bool Available(UserInfo user)
        {
            return !String.IsNullOrEmpty(JoinAffilliateLink) &&
                   user.ActivationStatus == EmployeeActivationStatus.Activated &&
                   user.Status == EmployeeStatus.Active;
        }

        public static bool BannerAvailable
        {
            get
            {   
                var user = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID);
                return Available(user) && (TenantExtra.GetTenantQuota().NonProfit || user.IsVisitor());
            }
        }

        public static bool ButtonAvailable(UserInfo user)
        {
            return Available(user) && user.IsMe();
        }

        public static string Join()
        {
            if (!string.IsNullOrEmpty(JoinAffilliateLink))
            {
                var request = WebRequest.Create(string.Format("{2}/Account/Register?uid={1}&tenantAlias={0}",
                                                    CoreContext.TenantManager.GetCurrentTenant().TenantAlias, SecurityContext.CurrentAccount.ID,
                                                    JoinAffilliateLink));
                request.Method = "PUT";
                request.ContentLength = 0;
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        var origin = streamReader.ReadToEnd();
                        if (response.StatusCode != HttpStatusCode.BadRequest)
                        {
                            return string.Format("{0}/Account/SignIn?ticketKey={1}", JoinAffilliateLink, origin);
                        }
                    }
                }
            }

            return "";
        }
    }
}
