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
using System.ServiceModel;
using ASC.Common.Module;
using ASC.Core;
using ASC.FullTextIndex.Service.Config;
using log4net;

namespace ASC.FullTextIndex
{
    public static class FullTextSearch
    {
        public static readonly string BlogsModule = "Blogs";
        public static readonly string NewsModule = "News";
        public static readonly string PhotosModule = "Photos";
        public static readonly string BookmarksModule = "Bookmarks";
        public static readonly string WikiModule = "Wiki";
        public static readonly string ForumModule = "Forum";
        public static readonly string ProjectsModule = "Projects";
        public static readonly string UserEmailsModule = "UserEmails";
        public static readonly string FileModule = "Files";
        public static readonly string MailModule = "Mail";
        public static readonly string MailContactsModule = "Mail.Contacts";
        public static readonly string MailFromTextModule = "Mail.FromText";
        public static readonly string CRMTasksModule = "CRM.Tasks";
        public static readonly string CRMDealsModule = "CRM.Deals";
        public static readonly string CRMContactsModule = "CRM.Contacts";
        public static readonly string CRMCasesModule = "CRM.Cases";
        public static readonly string CRMEmailsModule = "CRM.Emails";
        public static readonly string CRMEventsModule = "CRM.Events";


        public static readonly string[] AllModules = new[] 
        {
            BlogsModule, 
            NewsModule, 
            PhotosModule, 
            BookmarksModule, 
            ForumModule, 
            ProjectsModule,
            UserEmailsModule,
            WikiModule,
            FileModule,
            MailModule,
            MailContactsModule,
            MailFromTextModule,
            CRMTasksModule,
            CRMDealsModule,
            CRMContactsModule,
            CRMCasesModule,
            CRMEmailsModule,
            CRMEventsModule
        };

        private static readonly ILog log = LogManager.GetLogger(typeof(FullTextSearch));

        private static readonly TimeSpan timeout = TimeSpan.FromMinutes(1);

        private static DateTime lastErrorTime = default(DateTime);

        private static bool IsServiceProbablyNotAvailable()
        {
            var disabled = ConfigurationManager.AppSettings["fullTextSearch"] == "false";
            return disabled || (lastErrorTime != default(DateTime) && lastErrorTime + timeout > DateTime.Now);
        }


        public static bool SupportModule(string module)
        {
            var result = false;
            if (IsServiceProbablyNotAvailable()) return result;

            using (var service = new TextIndexServiceClient())
            {
                try
                {
                    result = service.SupportModule(module);
                }
                catch (FaultException fe)
                {
                    LogError(fe);
                }
                catch (CommunicationException ce)
                {
                    LogError(ce);
                    lastErrorTime = DateTime.Now;
                }
                catch (TimeoutException te)
                {
                    LogError(te);
                    lastErrorTime = DateTime.Now;
                }
            }
            return result;
        }

        public static TextSearchResult Search(string query, string module)
        {
            return Search(query, module, CoreContext.TenantManager.GetCurrentTenant().TenantId);
        }

        public static TextSearchResult Search(string query, string module, int tenantId)
        {
            var result = new TextSearchResult(module);

            if (IsServiceProbablyNotAvailable() || string.IsNullOrEmpty(query))
            {
                return result;
            }
            if (TextIndexCfg.MaxQueryLength < query.Length)
            {
                query = query.Substring(0, TextIndexCfg.MaxQueryLength);
            }

            using (var service = new TextIndexServiceClient())
            {
                try
                {
                    result = service.Search(tenantId, query, module);
                }
                catch (FaultException fe)
                {
                    LogError(fe);
                }
                catch (CommunicationException ce)
                {
                    LogError(ce);
                    lastErrorTime = DateTime.Now;
                }
                catch (TimeoutException te)
                {
                    LogError(te);
                    lastErrorTime = DateTime.Now;
                }
            }
            return result;
        }

        private static void LogError(Exception error)
        {
            log.Error(error);
        }


        private class TextIndexServiceClient : BaseWcfClient<ITextIndexService>, ITextIndexService
        {
            public bool SupportModule(string module)
            {
                return Channel.SupportModule(module);
            }

            public TextSearchResult Search(int tenant, string query, string module)
            {
                return Channel.Search(tenant, query, module);
            }
        }
    }
}
