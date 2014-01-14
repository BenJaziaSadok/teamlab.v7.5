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
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Core.Users;
using ASC.CRM.Core;
using ASC.CRM.Core.Entities;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Controls.Common;
using ASC.Web.CRM.Resources;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#endregion

namespace ASC.Web.CRM.Controls.Deals
{
    public partial class ListDealView : BaseUserControl
    {
        #region Properies

        public static string Location
        {
            get { return PathProvider.GetFileStaticRelativePath("Deals/ListDealView.ascx"); }
        }

        protected bool MobileVer = false;

        protected string CookieKeyForPagination
        {
            get { return "dealPageNumber"; }
        }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            MobileVer = ASC.Web.Core.Mobile.MobileDetector.IsRequestMatchesMobile(this.Context);

            if (UrlParameters.Action != "export")
            {
                InitPanels();

                Page.RegisterClientScript(typeof(Masters.ClientScripts.ListDealViewData));
                RegisterScript();
                InitExchangeRateView();
            }
            else
            {
                var deals = GetDealsByFilter();

                if (UrlParameters.View != "editor")
                {
                    Response.Clear();
                    Response.ContentType = "text/csv; charset=utf-8";
                    Response.ContentEncoding = Encoding.UTF8;
                    Response.Charset = Encoding.UTF8.WebName;
                    var fileName = "opportunity.csv";

                    Response.AppendHeader("Content-Disposition", String.Format("attachment; filename={0}", fileName)); Response.Write(ExportToCSV.ExportDealsToCSV(deals, false));
                    Response.End();
                }
                else
                {
                    var fileUrl = ExportToCSV.ExportDealsToCSV(deals, true);
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

        private void InitExchangeRateView()
        {
            Page.RegisterClientScript(typeof(Masters.ClientScripts.ExchangeRateViewData));
        }

        private class FilterObject
        {
            public string SortBy { get; set; }
            public string SortOrder { get; set; }
            public string FilterValue { get; set; }

            public Guid ResponsibleID { get; set; }
            
            public String StageType { get; set; }
            public int OpportunityStagesID { get; set; }

            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }

            public String FromDateString { get; set; }
            public String ToDateString { get; set; }

            public int ContactID { get; set; }
            public bool? ContactAlsoIsParticipant { get; set; }

            public List<string> Tags { get; set; }

            public FilterObject()
            {
                ContactAlsoIsParticipant = null;
                FromDate = DateTime.MinValue;
                ToDate = DateTime.MinValue;
            }
        };

        private FilterObject GetFilterObjectFromCookie()
        {
            var result = new FilterObject();

            var cookieKey = Request.Url.AbsoluteUri.Substring(0, Request.Url.AbsoluteUri.Length - Request.Url.Query.Length);

            var cookie = Request.Cookies[HttpUtility.UrlEncode(cookieKey)];

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

                            case "stageTypeOpen":
                            case "stageTypeClosedAndWon":
                            case "stageTypeClosedAndLost":
                                result.StageType = filterParam.Value<string>("value");
                                break;
                            case "opportunityStagesID":
                                result.OpportunityStagesID = filterParam.Value<int>("value");
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

                            case "participantID":
                                result.ContactID = filterParam.Value<int>("id");
                                result.ContactAlsoIsParticipant = true;
                                break;
                            case "contactID":
                                result.ContactID = filterParam.Value<int>("id");
                                result.ContactAlsoIsParticipant = false;
                                break;

                            case "tags":
                                result.Tags = new List<string>();
                                result.Tags = filterParam.Value<JArray>("value").ToList().ConvertAll(n => n.ToString());
                                break;
                        }
                    }
                }
                catch (Exception)
                {
                    result.SortBy = "stage";
                    result.SortOrder = "ascending";
                }
            }
            else
            {
                result.SortBy = "stage";
                result.SortOrder = "ascending";
            }

            return result;
        }

        protected List<Deal> GetDealsByFilter()
        {
            var filterObj = GetFilterObjectFromCookie();

            DealSortedByType sortBy;
            if (!Web.CRM.Classes.EnumExtension.TryParse(filterObj.SortBy, true, out sortBy))
            {
                sortBy = DealSortedByType.Title;
            }

            var isAsc = !String.IsNullOrEmpty(filterObj.SortOrder) && filterObj.SortOrder != "descending";

            DealMilestoneStatus? stageType = null;
            if (!String.IsNullOrEmpty(filterObj.StageType))
            {
                if (filterObj.StageType.ToLower() == DealMilestoneStatus.Open.ToString().ToLower())
                    stageType = DealMilestoneStatus.Open;
                if (filterObj.StageType.ToLower() == DealMilestoneStatus.ClosedAndLost.ToString().ToLower())
                    stageType = DealMilestoneStatus.ClosedAndLost;
                if (filterObj.StageType.ToLower() == DealMilestoneStatus.ClosedAndWon.ToString().ToLower())
                    stageType = DealMilestoneStatus.ClosedAndWon;
            }

            return Global.DaoFactory.GetDealDao().GetDeals(filterObj.FilterValue,
                                                           filterObj.ResponsibleID,
                                                           filterObj.OpportunityStagesID,
                                                           filterObj.Tags,
                                                           filterObj.ContactID,
                                                           stageType,
                                                           filterObj.ContactAlsoIsParticipant,
                                                           filterObj.FromDate,
                                                           filterObj.ToDate,
                                                           0, 0,
                                                           new OrderBy(sortBy, isAsc));
        }

        private void RegisterScript()
        {
            var sb = new StringBuilder();

            sb.AppendFormat(@"ASC.CRM.ListDealView.init(0,{0},""{1}"",""{2}"",{3},{4},""{5}"",""{6}"");",
                Global.VisiblePageCount,
                CookieKeyForPagination,
                System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator,
                CRMSecurity.IsAdmin.ToString().ToLower(),
                (int)ContactSelectorTypeEnum.All,
                WebImageSupplier.GetAbsoluteWebPath("empty_screen_deals.png", ProductEntryPoint.ID),
                WebImageSupplier.GetAbsoluteWebPath("empty_screen_filter.png")
            );

            Page.RegisterInlineScript(sb.ToString());
        }

        #endregion

    }
}