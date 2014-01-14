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
using ASC.Core.Users;
using AjaxPro;
using ASC.Core;
using ASC.Core.Caching;
using ASC.CRM.Core;
using ASC.CRM.Core.Entities;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Configuration;
using ASC.Web.CRM.Controls.Common;
using ASC.Web.CRM.Controls.Settings;
using ASC.Web.CRM.Resources;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Core.Users;
using ASC.Web.Studio.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#endregion

namespace ASC.Web.CRM.Controls.Contacts
{
    public partial class ListContactView : BaseUserControl
    {
        private static ICache showEmptyScreen = new AspCache();

        #region Properies

        public static string Location
        {
            get { return PathProvider.GetFileStaticRelativePath("Contacts/ListContactView.ascx"); }
        }

        protected string CookieKeyForPagination
        {
            get { return "contactPageNumber"; }
        }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (UrlParameters.Action != "export")
            {
                InitPage();
                Page.RegisterClientScript(typeof(Masters.ClientScripts.ListContactViewData));

                if (showEmptyScreen.Get("crmScreen" + TenantProvider.CurrentTenantID) == null)
                {
                    var hasactivity = Global.DaoFactory.GetContactDao().HasActivity();
                    if (hasactivity)
                    {
                        showEmptyScreen.Insert("crmScreen" + TenantProvider.CurrentTenantID, new object(), TimeSpan.FromMinutes(30));
                    }
                    else
                    {
                        RenderDashboardEmptyScreen();
                    }
                }

                RegisterScript();
            }
            else
            {
                var contacts = GetContactsByFilter();

                if (UrlParameters.View != "editor")
                {
                    Response.Clear();
                    Response.ContentType = "text/csv; charset=utf-8";
                    Response.ContentEncoding = Encoding.UTF8;
                    Response.Charset = Encoding.UTF8.WebName;
                    var fileName = "contacts.csv";

                    Response.AppendHeader("Content-Disposition", String.Format("attachment; filename={0}", fileName)); Response.Write(ExportToCSV.ExportContactsToCSV(contacts, false));
                    Response.End();
                }
                else
                {
                    var fileUrl = ExportToCSV.ExportContactsToCSV(contacts, true);
                    Response.Redirect(fileUrl);
                }
            }
        }

        #endregion

        #region Methods

        private void InitPage()
        {
            Utility.RegisterTypeForAjax(typeof(CommonSettingsView));

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
            public List<string> Tags { get; set; }
            public string ContactListView { get; set; }
            public int ContactStage { get; set; }
            public int ContactType { get; set; }
            public Guid ResponsibleID { get; set; }

            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }

            public String FromDateString { get; set; }
            public String ToDateString { get; set; }

            public FilterObject()
            {
                FromDate = DateTime.MinValue;
                ToDate = DateTime.MinValue;
                ContactStage = -1;
                ContactType = -1;
            }
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
                            case "my":
                            case "responsibleID":
                                result.ResponsibleID = new Guid(filterParam.Value<string>("value"));
                                break;
                            case "tags":
                                result.Tags = new List<string>();
                                result.Tags = filterParam.Value<JArray>("value").ToList().ConvertAll(n => n.ToString());
                                break;
                            case "withopportunity":
                            case "person":
                            case "company":
                                result.ContactListView = filterParam.Value<string>("value");
                                break;
                            case "contactType":
                                result.ContactType = filterParam.Value<int>("value");
                                break;
                            case "contactStage":
                                result.ContactStage = filterParam.Value<int>("value");
                                break;

                            case "lastMonth":
                            case "yesterday":
                            case "today":
                            case "thisMonth":
                                var valueString = filterParam.Value<string>("value");
                                var fromToArray = JsonConvert.DeserializeObject<List<string>>(valueString);
                                if (fromToArray.Count != 2) continue;
                                result.FromDateString = fromToArray[0];
                                result.ToDateString = fromToArray[1];
                                result.FromDate = UrlParameters.ApiDateTimeParse(result.FromDateString);
                                result.ToDate = UrlParameters.ApiDateTimeParse(result.ToDateString);
                                break;
                            case "fromToDate":
                                result.FromDateString = filterParam.Value<string>("from");
                                result.ToDateString = filterParam.Value<string>("to");
                                result.FromDate = UrlParameters.ApiDateTimeParse(result.FromDateString);
                                result.ToDate = UrlParameters.ApiDateTimeParse(result.ToDateString);
                                break;
                        }
                    }
                }
                catch (Exception)
                {
                    result.SortBy = "created";
                    result.SortOrder = "descending";
                }
            }
            else
            {
                result.SortBy = "created";
                result.SortOrder = "descending";
            }

            return result;
        }

        protected List<Contact> GetContactsByFilter()
        {
            var filterObj = GetFilterObjectFromCookie();

            var contactListViewType = ContactListViewType.All;
            if (!String.IsNullOrEmpty(filterObj.ContactListView))
            {
                if (filterObj.ContactListView == ContactListViewType.Company.ToString().ToLower())
                    contactListViewType = ContactListViewType.Company;
                if (filterObj.ContactListView == ContactListViewType.Person.ToString().ToLower())
                    contactListViewType = ContactListViewType.Person;
                if (filterObj.ContactListView == ContactListViewType.WithOpportunity.ToString().ToLower())
                    contactListViewType = ContactListViewType.WithOpportunity;
            }

            ContactSortedByType sortBy;

            if (!Web.CRM.Classes.EnumExtension.TryParse(filterObj.SortBy, true, out sortBy))
            {
                sortBy = ContactSortedByType.Created;
            }

            var isAsc = !String.IsNullOrEmpty(filterObj.SortOrder) && filterObj.SortOrder != "descending";

            return Global.DaoFactory.GetContactDao().GetContacts(
                                                            filterObj.FilterValue,
                                                            filterObj.Tags,
                                                            filterObj.ContactStage,
                                                            filterObj.ContactType,
                                                            contactListViewType,
                                                            filterObj.FromDate,
                                                            filterObj.ToDate,
                                                            0,
                                                            0,
                                                            new OrderBy(sortBy, isAsc),
                                                            filterObj.ResponsibleID);
        }

        protected void RenderDashboardEmptyScreen()
        {
            var dashboardEmptyScreen = (DashboardEmptyScreen)Page.LoadControl(DashboardEmptyScreen.Location);
            _phDashboardEmptyScreen.Controls.Add(dashboardEmptyScreen);
        }

        private void RegisterScript()
        {
            var sb = new StringBuilder();

            sb.AppendFormat(@"ASC.CRM.ListContactView.init({0},{1},{2},'{3}','{4}','{5}','{6}','{7}');",
                Global.VisiblePageCount,
                MailSender.GetQuotas(),
                CRMSecurity.IsAdmin.ToString().ToLower(),
                CookieKeyForPagination,
                CRMSettingResource.ConfigureTheSMTP.HtmlEncode().ReplaceSingleQuote(),
                CRMSettingResource.ConfigureTheSMTPInfo.HtmlEncode().ReplaceSingleQuote(),
                WebImageSupplier.GetAbsoluteWebPath("empty_screen_persons.png", ProductEntryPoint.ID),
                WebImageSupplier.GetAbsoluteWebPath("empty_screen_filter.png")
            );

            Page.RegisterInlineScript(sb.ToString());
        }

        #endregion
    }
}