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
using ASC.Web.CRM.Configuration;
using ASC.Web.Core.Utility.Skins;
using ASC.CRM.Core.Entities;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Resources;
using ASC.CRM.Core;
using ASC.Web.Studio.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using log4net;

#endregion

namespace ASC.Web.CRM.Controls.Tasks
{
    public partial class ListTaskView : BaseUserControl
    {
        #region Properies

        public static string Location
        {
            get { return PathProvider.GetFileStaticRelativePath("Tasks/ListTaskView.ascx"); }
        }

        public int EntityID { get; set; }
        public Contact CurrentContact { get; set; }
        public EntityType CurrentEntityType { get; set; }

        protected bool MobileVer = false;

        protected string CookieKeyForPagination
        {
            get { return "taskPageNumber"; }
        }

        private const string ExportErrorCookieKey = "export_tasks_error";

        protected readonly ILog _log = LogManager.GetLogger("ASC.CRM");

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            MobileVer = ASC.Web.Core.Mobile.MobileDetector.IsRequestMatchesMobile(this.Context);

            if (UrlParameters.Action != "export")
            {
                PageLoadRegular();
            }
            else // export to csv
            {
                var tasks = GetTasksByFilter();

                if (tasks.Count != 0)
                {
                    if (UrlParameters.View != "editor")
                    {
                        Response.Clear();
                        Response.ContentType = "text/csv; charset=utf-8";
                        Response.ContentEncoding = Encoding.UTF8;
                        Response.Charset = Encoding.UTF8.WebName;

                        var fileName = "tasks.csv";

                        Response.AppendHeader("Content-Disposition", String.Format("attachment; filename={0}", fileName));
                        Response.Write(ExportToCSV.ExportTasksToCSV(tasks, false));
                        Response.End();
                    }
                    else
                    {
                        var fileUrl = ExportToCSV.ExportTasksToCSV(tasks, true);
                        Response.Redirect(fileUrl);
                    }
                }
                else
                {
                    var cookie = HttpContext.Current.Request.Cookies.Get(ExportErrorCookieKey);
                    if (cookie == null)
                    {
                        cookie = new HttpCookie(ExportErrorCookieKey);
                        cookie.Value = CRMTaskResource.ExportTaskListEmptyError;
                        HttpContext.Current.Response.Cookies.Add(cookie);
                    }
                    Response.Redirect(PathProvider.StartURL() + "tasks.aspx");
                }
            }
        }

        #endregion

        #region Methods

        private void PageLoadRegular() {
            Page.RegisterClientScript(typeof(Masters.ClientScripts.ListTaskViewData));
            RegisterScript();
        }

        private class FilterObject
        {
            public string SortBy { get; set; }
            public string SortOrder { get; set; }
            public string FilterValue { get; set; }

            public Guid ResponsibleID { get; set; }

            public bool? IsClosed { get; set; }
            
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }

            public int CategoryID { get; set; }

            public int ContactID { get; set; }
           
            public FilterObject()
            {
                IsClosed = null;
                FromDate = DateTime.MinValue;
                ToDate = DateTime.MinValue;
            }
        };

        private FilterObject GetFilterObjectFromCookie()
        {
            var result = new FilterObject();

            var cookieKey = Request.Url.AbsoluteUri.Substring(0, Request.Url.AbsoluteUri.Length - Request.Url.Query.Length);

            var cookie = Request.Cookies[System.Web.HttpUtility.UrlEncode(cookieKey)];
            _log.Debug(String.Format("GetFilterObjectFromCookie. cookieKey={0}", cookieKey));

            if (cookie != null && !String.IsNullOrEmpty(cookie.Value))
            {
                var anchor = cookie.Value;
                _log.Debug(String.Format("GetFilterObjectFromCookie. cookie.Value={0}", anchor));
                try
                {
                    var cookieJson = Encoding.UTF8.GetString(Convert.FromBase64String(HttpUtility.UrlDecode(anchor)));
                    _log.Debug(String.Format("GetFilterObjectFromCookie. cookieJson={0}", cookieJson));
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

                            case "overdue":
                            case "today":
                            case "theNext":
                                var valueString = filterParam.Value<string>("value");
                                var fromToArray = JsonConvert.DeserializeObject<List<string>>(valueString);
                                if (fromToArray.Count != 2) continue;
                                result.FromDate = !String.IsNullOrEmpty(fromToArray[0]) ? UrlParameters.ApiDateTimeParse(fromToArray[0]) : DateTime.MinValue;
                                result.ToDate = !String.IsNullOrEmpty(fromToArray[1]) ? UrlParameters.ApiDateTimeParse(fromToArray[1]) : DateTime.MinValue;
                                break;
                            case "fromToDate":
                                result.FromDate =filterParam.Value<DateTime>("from");
                                result.ToDate = filterParam.Value<DateTime>("to");
                                break;
                            case "categoryID":
                                result.CategoryID = filterParam.Value<int>("value");
                                break;
                            case "openTask":
                            case "closedTask":
                                result.IsClosed = filterParam.Value<bool>("value");
                                break;
                            case "contactID":
                                result.ContactID = filterParam.Value<int>("id");
                                break;
                        }
                    }
                }
                catch (Exception)
                {
                    _log.Info("GetFilterObjectFromCookie. Exception! line 206");
                    result.SortBy = "deadline";
                    result.SortOrder = "ascending";
                }
            }
            else
            {
                _log.Info("GetFilterObjectFromCookie. Cookie is null, default filters should be used");
                result.SortBy = "deadline";
                result.SortOrder = "ascending";
            }

            return result;
        }

        protected List<Task> GetTasksByFilter()
        {
            var filterObj = GetFilterObjectFromCookie();

            TaskSortedByType sortBy;
            if (!Web.CRM.Classes.EnumExtension.TryParse(filterObj.SortBy, true, out sortBy))
            {
                sortBy = TaskSortedByType.DeadLine;
            }

            var isAsc = !String.IsNullOrEmpty(filterObj.SortOrder) && filterObj.SortOrder != "descending";

            return Global.DaoFactory.GetTaskDao().GetTasks(filterObj.FilterValue,
                                                           filterObj.ResponsibleID,
                                                           filterObj.CategoryID,
                                                           filterObj.IsClosed,
                                                           filterObj.FromDate,
                                                           filterObj.ToDate,
                                                           filterObj.ContactID > 0? EntityType.Contact: EntityType.Any,
                                                           filterObj.ContactID,
                                                           0, 0,
                                                           new OrderBy(sortBy, isAsc));
        }

        private void RegisterScript()
        {
            var sb = new StringBuilder();

            sb.AppendFormat(@"ASC.CRM.ListTaskView.init({0},""{1}"",{2},{3},""{4}"",{5},{6},""{7}"",""{8}"", ""{9}"");",
                CurrentContact != null ? CurrentContact.ID : 0,
                CurrentEntityType.ToString().ToLower(),
                EntityID,
                Global.VisiblePageCount,
                CookieKeyForPagination,
                (int)HistoryCategorySystem.TaskClosed,
                (int)ContactSelectorTypeEnum.All,
                WebImageSupplier.GetAbsoluteWebPath("empty_screen_tasks.png", ProductEntryPoint.ID),
                WebImageSupplier.GetAbsoluteWebPath("empty_screen_filter.png"),
                ExportErrorCookieKey
            );

            Page.RegisterInlineScript(sb.ToString());
        }

        #endregion
    }
}