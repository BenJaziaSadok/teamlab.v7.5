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

using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using ASC.Common.Module;

namespace ASC.Core.Configuration
{
    public class AmiPublicDnsSyncService : IServiceController
    {
        public void Start()
        {
            Synchronize();
        }

        public void Stop()
        {

        }

        public static void Synchronize()
        {
            if (CoreContext.Configuration.Standalone)
            {
                var tenants = CoreContext.TenantManager.GetTenants().Where(t => MappedDomainNotSettedByUser(t.MappedDomain));
                if (tenants.Any())
                {
                    var dnsname = GetAmiPublicDnsName();
                    foreach (var tenant in tenants.Where(t => !string.IsNullOrEmpty(dnsname) && t.MappedDomain != dnsname))
                    {
                        tenant.MappedDomain = dnsname;
                        CoreContext.TenantManager.SaveTenant(tenant);
                    }
                }
            }
        }

        private static bool MappedDomainNotSettedByUser(string domain)
        {
            return string.IsNullOrEmpty(domain) || Regex.IsMatch(domain, "^ec2.+\\.compute\\.amazonaws\\.com$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        }

        private static string GetAmiPublicDnsName()
        {
            try
            {
                var request = WebRequest.Create("http://169.254.169.254/latest/meta-data/public-hostname");
                request.Timeout = 5000;
                using (var responce = request.GetResponse())
                using (var stream = responce.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    throw;
                }
            }
            return null;
        }
    }
}
