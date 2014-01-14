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

#region Import

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web;
using ASC.Core;
using ASC.Core.Users;
using ASC.CRM.Core;
using ASC.CRM.Core.Dao;
using ASC.Data.Storage;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.CRM.Resources;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;
using System.Web.Configuration;
using Newtonsoft.Json.Linq;

#endregion

namespace ASC.Web.CRM.Classes
{
    public class Global
    {
        public static readonly int EntryCountOnPage = 25;
        public static readonly int VisiblePageCount = 10;

        public static readonly int MaxCustomFieldSize = 150;
        public static readonly int MaxCustomFieldRows = 25;
        public static readonly int MaxCustomFieldCols = 150;

        public static readonly int DefaultCustomFieldSize = 40;
        public static readonly int DefaultCustomFieldRows = 2;
        public static readonly int DefaultCustomFieldCols = 40;

        public static DaoFactory DaoFactory
        {
            get { return new DaoFactory(TenantProvider.CurrentTenantID, CRMConstants.DatabaseId); }
        }

        public static CRMSettings TenantSettings
        {
            get { return SettingsManager.Instance.LoadSettings<CRMSettings>(TenantProvider.CurrentTenantID); }
        }

        public static IDataStore GetStore()
        {
            return StorageFactory.GetStorage(PathProvider.BaseVirtualPath + "web.config",
                                             TenantProvider.CurrentTenantID.ToString(), "crm");
        }

        public static bool CanCreateProjects()
        {
            try
            {
                var apiUrl = String.Format("{0}project/securityinfo.json", SetupInfo.WebApiBaseUrl);

                var cacheKey = String.Format("{0}-{1}", ASC.Core.SecurityContext.CurrentAccount.ID, apiUrl);

                bool canCreateProject;

                if (HttpRuntime.Cache[cacheKey] != null)
                    return Convert.ToBoolean(HttpRuntime.Cache[cacheKey]);

                var apiServer = new Api.ApiServer();

                var responseApi = JObject.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(apiServer.GetApiResponse(apiUrl, "GET"))))["response"];

                if (responseApi.HasValues)
                    canCreateProject = Convert.ToBoolean(responseApi["canCreateProject"].Value<String>());
                else
                    canCreateProject = false;

                HttpRuntime.Cache.Insert(cacheKey, canCreateProject, null, System.Web.Caching.Cache.NoAbsoluteExpiration,
                                  TimeSpan.FromMinutes(5));

                return canCreateProject;

            }
            catch 
            {

                return false;
            }

        }


        //Code snippet

        /// <summary>
        /// method for generating a country list, say for populating
        /// a ComboBox, with country options. We return the
        /// values in a Generic List<T>
        /// </summary>
        /// <returns></returns>
        public static List<string> GetCountryList()
        {
            var cultureList = new List<string>();

            var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures);

            foreach (var culture in cultures)
            {
                if (culture.LCID == 127) continue;

                try
                {

                    var region = new RegionInfo(culture.Name);

                    if (String.IsNullOrEmpty(region.EnglishName)) continue;

                    if (!(cultureList.Contains(region.EnglishName)))
                        cultureList.Add(region.EnglishName);
                }
                catch
                {


                }                


            }

            //foreach (var ci in Web.Studio.Core.SetupInfo.EnabledCultures)
            //{
            //    cultureList.Add(ci.DisplayName);
            //}
            return cultureList;
        }

        public static String GetUpButtonHTML(Uri requestUrlReferrer)
        {
            return String.Format(@"<a title='{0}' {1} class='studio-level-up{2}' style='margin-top: 2px;'></a>",
                                  CRMCommonResource.Up,
                                  requestUrlReferrer != null ? "href='" + requestUrlReferrer.OriginalString + "'" : "",
                                  requestUrlReferrer != null ? "" : " disable");
        }

        public static String RenderItemHeaderWithMenu(String title, EntityType entityType, Boolean isPrivate)
        {
            var sbIcon = new StringBuilder();
            var sbPrivateMark = new StringBuilder();

            string titleIconClass;
            switch (entityType)
            {
                case EntityType.Contact:
                    titleIconClass = "group";
                    break;
                case EntityType.Person:
                    titleIconClass = "people";
                    break;
                case EntityType.Company:
                    titleIconClass = "company";
                    break;
                case EntityType.Case:
                    titleIconClass = "cases";
                    break;
                case EntityType.Opportunity:
                    titleIconClass = "opportunities";
                    break;
                default:
                    titleIconClass = string.Empty;
                    break;
            }
            if (!String.IsNullOrEmpty(titleIconClass))
            {
                if (isPrivate)
                {
                    sbPrivateMark.AppendFormat("<div class='privateMark' title='{0}'></div>", CRMCommonResource.Private);
                }
                sbIcon.AppendFormat("<span class='main-title-icon {0}'>{1}</span>", titleIconClass, sbPrivateMark);
            }



            return String.Format(@" <div class='header-with-menu crm-pageHeader'>
                                        {0}<span class='crm-pageHeaderText text-overflow'>{1}</span>
                                        <span class='menu-small'></span>
                                    </div>
                                  ",
                                        sbIcon,
                                        title
                                      );
        }
    }
}