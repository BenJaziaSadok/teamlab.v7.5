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
using System.Linq;
using System.Text;
using System.Web;
using ASC.Core;
using ASC.Web.CRM.Controls.Common;
using ASC.Web.CRM.Configuration;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Core.Users;
using ASC.CRM.Core.Entities;
using ASC.Web.CRM.Resources;
using ASC.Web.CRM.Classes;
using ASC.CRM.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#endregion

namespace ASC.Web.CRM.Controls.Cases
{
    public partial class ListCasesView : BaseUserControl
    {
        #region Properies

        public static string Location
        {
            get { return PathProvider.GetFileStaticRelativePath("Cases/ListCasesView.ascx"); }
        }

        protected bool MobileVer = false;

        protected string CookieKeyForPagination
        {
            get { return "casesPageNumber"; }
        }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            MobileVer = ASC.Web.Core.Mobile.MobileDetector.IsRequestMatchesMobile(this.Context);

            if (UrlParameters.Action != "export")
            {
                InitPanels();
                Page.RegisterClientScript(typeof(Masters.ClientScripts.ListCasesViewData));
                RegisterScript();
            }
            else
            {
                var cases = GetCasesByFilter();

                if (UrlParameters.View != "editor")
                {
                    Response.Clear();
                    Response.ContentType = "text/csv; charset=utf-8";
                    Response.ContentEncoding = Encoding.UTF8;
                    Response.Charset = Encoding.UTF8.WebName;

                    var fileName = "cases.csv";

                    Response.AppendHeader("Content-Disposition", String.Format("attachment; filename={0}", fileName));
                   
                    
                    Response.Write(ExportToCSV.ExportCasesToCSV(cases, false));
                    Response.End();
                }
                else
                {
                    var fileUrl = ExportToCSV.ExportCasesToCSV(cases, true);
                    Response.Redirect(fileUrl);
                }
            }
        }

        #endregion

        #region Methods

        private void InitPanels()
        {
            //init PrivatePanel
            var privatePanel = (PrivatePanel)LoadControl(PrivatePanel.Location);
            var usersWhoHasAccess = new List<string> { CustomNamingPeople.Substitute<CRMCommonResource>("CurrentUser").HtmlEncode() };
            privatePanel.UsersWhoHasAccess = usersWhoHasAccess;
            privatePanel.DisabledUsers = new List<Guid> { SecurityContext.CurrentAccount.ID };
            privatePanel.HideNotifyPanel = true;
            _phPrivatePanel.Controls.Add(privatePanel);
        }

        private class FilterObject
        {
            public string SortBy { get; set; }
            public string SortOrder { get; set; }
            public string FilterValue { get; set; }
            public bool? IsClosed { get; set; }
            public List<string> Tags { get; set; }

            public FilterObject()
            { }
        };

        private FilterObject GetFilterObjectFromCookie()
        {
            var result = new FilterObject();

            var cookieKey = Request.Url.AbsoluteUri.Substring(0, Request.Url.AbsoluteUri.Length - Request.Url.Query.Length);
            var cookie = Request.Cookies[System.Web.HttpUtility.UrlEncode(cookieKey)];

            if (cookie != null && !String.IsNullOrEmpty(cookie.Value))
            {
                var anchor = cookie.Value;

                try
                {
                    var cookieJson = Encoding.UTF8.GetString(Convert.FromBase64String(HttpUtility.UrlDecode(anchor)));
                    
                    var jsonArray = cookieJson.Split(';');

                    foreach (var filterItem in jsonArray)
                    {
                        var filterObj = JObject.Parse(filterItem);

                        var filterParam = JObject.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(filterObj.Value<string>("params"))));

                        switch (filterObj.Value<string>("id"))
                        {
                            case "sorter":
                                result.SortBy = filterParam.Value<string>("id");
                                result.SortOrder = filterParam.Value<string>("sortOrder");
                                break;
                            case "text":
                                result.FilterValue = filterParam.Value<string>("value");
                                break;
                            case "closed":
                            case "opened":
                                result.IsClosed = filterParam.Value<bool>("value");
                                break;
                            case "tags":
                                result.Tags = new List<string>();
                                result.Tags = filterParam.Value<JArray>("value").ToList().ConvertAll(
                                        n => n.ToString());
                                break;
                        }
                    }
                }
                catch (Exception)
                {
                    result.SortBy = "title";
                    result.SortOrder = "ascending";
                }
            }
            else
            {
                result.SortBy = "title";
                result.SortOrder = "ascending";
            }
            return result;
        }

        protected List<ASC.CRM.Core.Entities.Cases> GetCasesByFilter()
        {
            var filterObj = GetFilterObjectFromCookie();

            SortedByType sortBy;
            if (!Web.CRM.Classes.EnumExtension.TryParse(filterObj.SortBy, true, out sortBy))
            {
                sortBy = SortedByType.Title;
            }

            var isAsc = !String.IsNullOrEmpty(filterObj.SortOrder) && filterObj.SortOrder != "descending";

            return Global.DaoFactory.GetCasesDao().GetCases(filterObj.FilterValue,
                                                            0,
                                                            filterObj.IsClosed,
                                                            filterObj.Tags,
                                                            0, 0,
                                                            new OrderBy(sortBy, isAsc));
        }

        private void RegisterScript()
        {
            var sb = new StringBuilder();

            sb.AppendFormat(@"ASC.CRM.ListCasesView.init({0},""{1}"",{2},""{3}"",""{4}"");",
                Global.VisiblePageCount,
                CookieKeyForPagination,
                CRMSecurity.IsAdmin.ToString().ToLower(),
                WebImageSupplier.GetAbsoluteWebPath("empty_screen_cases.png", ProductEntryPoint.ID),
                WebImageSupplier.GetAbsoluteWebPath("empty_screen_filter.png")
            );

            Page.RegisterInlineScript(sb.ToString());
        }

        #endregion

    }
}