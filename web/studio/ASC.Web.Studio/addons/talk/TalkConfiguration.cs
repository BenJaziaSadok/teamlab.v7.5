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
using System.Web;
using System.Web.Configuration;
using ASC.Core;
using ASC.Web.Studio.Utility;
using ASC.Web.Talk.Addon;
using ASC.Xmpp.Common;
using log4net;

namespace ASC.Web.Talk
{
    class TalkConfiguration
    {
        public string ServerAddress
        {
            get;
            private set;
        }

        public string UpdateInterval
        {
            get;
            private set;
        }

        public string OverdueInterval
        {
            get;
            private set;
        }

        public string ServerName
        {
            get;
            private set;
        }

        public string ServerPort
        {
            get;
            private set;
        }

        public string BoshUri
        {
            get;
            private set;
        }

        public string UserName
        {
            get;
            private set;
        }

        public string Jid
        {
            get;
            private set;
        }

        public string FileTransportType
        {
            get;
            private set;
        }

        public string RequestTransportType
        {
            get;
            private set;
        }

        public bool EnabledFirebugLite
        {
            get;
            private set;
        }

        public bool EnabledHistory
        {
            get;
            private set;
        }

        public bool EnabledConferences
        {
            get;
            private set;
        }

        public bool EnabledMassend
        {
            get;
            private set;
        }

        public String ValidSymbols
        {
            get;
            private set;
        }

        public String HistoryLength
        {
            get;
            private set;
        }

        public String ResourcePriority
        {
            get;
            private set;
        }

        public String ClientInactivity
        {
            get;
            private set;
        }

        public TalkConfiguration()
        {
            RequestTransportType = WebConfigurationManager.AppSettings["RequestTransportType"] ?? "flash";
            ServerAddress = CoreContext.TenantManager.GetCurrentTenant().TenantDomain;
            ServerName = ServerAddress;
            ServerPort = WebConfigurationManager.AppSettings["JabberPort"] ?? 5222.ToString();
            BoshUri = WebConfigurationManager.AppSettings["BoshPath"] ?? "http://localhost:5280/http-poll/";
            if (RequestTransportType == "handler")
            {
                BoshUri = VirtualPathUtility.ToAbsolute(TalkAddon.BaseVirtualPath + "/http-poll/default.aspx");
            }
            else
            {
                BoshUri = string.Format(BoshUri, ServerAddress);
            }

            try
            {
                UserName = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).UserName.ToLowerInvariant();
            }
            catch
            {
                UserName = string.Empty;
            }
            Jid = string.Format("{0}@{1}", UserName, ServerName).ToLowerInvariant();
            FileTransportType = WebConfigurationManager.AppSettings["FileTransportType"] ?? "flash";
            // in seconds
            UpdateInterval = WebConfigurationManager.AppSettings["UpdateInterval"] ?? "3600";
            OverdueInterval = WebConfigurationManager.AppSettings["OverdueInterval"] ?? "60";

            EnabledHistory = (WebConfigurationManager.AppSettings["History"] ?? "on") == "on";
            EnabledMassend = (WebConfigurationManager.AppSettings["Massend"] ?? "on") == "on";
            EnabledConferences = (WebConfigurationManager.AppSettings["Conferences"] ?? "on") == "on";
            EnabledFirebugLite = (WebConfigurationManager.AppSettings["FirebugLite"] ?? "off") == "on";
            ValidSymbols = WebConfigurationManager.AppSettings["ValidSymbols"] ?? "äöüßña-žа-я";
            HistoryLength = WebConfigurationManager.AppSettings["HistoryLength"] ?? "10";
            ResourcePriority = WebConfigurationManager.AppSettings["ResourcePriority"] ?? "60";
            ClientInactivity = WebConfigurationManager.AppSettings["ClientInactivity"] ?? "90";
        }
    }
}