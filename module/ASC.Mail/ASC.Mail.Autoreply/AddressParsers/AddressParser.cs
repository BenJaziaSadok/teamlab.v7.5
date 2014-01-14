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

using System.Collections.Generic;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Linq;
using ASC.Core;
using ASC.Core.Tenants;

namespace ASC.Mail.Autoreply.AddressParsers
{
    internal abstract class AddressParser : IAddressParser
    {
        private Regex _routeRegex;
        private Regex RouteRegex
        {
            get { return _routeRegex ?? (_routeRegex = GetRouteRegex()); }
        }

        protected abstract Regex GetRouteRegex();
        protected abstract ApiRequest ParseRequestInfo(IDictionary<string, string> groups, Tenant t);

        protected bool IsLastVersion(Tenant t)
        {
            return t.Version > 1;
        }

        public ApiRequest ParseRequestInfo(string address)
        {
            try
            {
                var mailAddress = new MailAddress(address);

                var match = RouteRegex.Match(mailAddress.User);

                if (!match.Success)
                    return null;

                var tenant = CoreContext.TenantManager.GetTenant(mailAddress.Host);

                if (tenant == null)
                    return null;

                var groups = RouteRegex.GetGroupNames().ToDictionary(groupName => groupName, groupName => match.Groups[groupName].Value);
                var requestInfo = ParseRequestInfo(groups, tenant);

                requestInfo.Method = "POST";
                requestInfo.Tenant = tenant;

                if (!string.IsNullOrEmpty(requestInfo.Url))
                    requestInfo.Url = string.Format("api/2.0/{0}.json", requestInfo.Url);

                return requestInfo;
            }
            catch
            {
                return null;
            }
        }
    }
}
