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
using System.Web;
using ASC.Core.Tenants;
using log4net.Core;
using log4net.Layout;
using log4net.Util;

namespace ASC.Core.Common.Logging
{
    public class AdminLogLayout : LayoutSkeleton
    {
        public string Property
        {
            get;
            set;
        }

        public override void ActivateOptions()
        {
        }

        public override void Format(TextWriter writer, LoggingEvent loggingEvent)
        {
            var value = string.Empty;
            try
            {
                if (Property == "tenant")
                {
                    var tenant = CoreContext.TenantManager.GetCurrentTenant(false);
                    value = (tenant != null ? tenant.TenantId : Tenant.DEFAULT_TENANT).ToString();
                }
                else if (Property == "ip" && HttpContext.Current != null)
                {
                    value = HttpContext.Current.Request.UserHostAddress;
                }
                else if (Property == "user")
                {
                    value = SecurityContext.CurrentAccount.ID.ToString();
                }
                else if (Property == "email")
                {
                    value = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).Email;
                }
            }
            catch (Exception error)
            {
                if (IgnoresException)
                {
                    LogLog.Error("Can not format property " + Property, error);
                }
                else
                {
                    throw;
                }
            }
            writer.Write(value);
        }
    }
}
