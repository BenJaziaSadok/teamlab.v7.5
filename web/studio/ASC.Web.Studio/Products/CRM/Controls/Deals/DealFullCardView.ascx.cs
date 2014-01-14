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
using System.Web;
using System.Collections.Generic;
using ASC.CRM.Core;
using ASC.CRM.Core.Entities;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Controls.Common;
using ASC.Web.CRM.Resources;

#endregion

namespace ASC.Web.CRM.Controls.Deals
{
    public partial class DealFullCardView : BaseUserControl
    {
        #region Property

        public Deal TargetDeal { get; set; }

        public static String Location
        {
            get { return PathProvider.GetFileStaticRelativePath("Deals/DealFullCardView.ascx"); }
        }

        public List<DealMilestone> AllDealMilestones;

        #endregion

        #region Events

        /// <summary>
        /// The method to Decode your Base64 strings.
        /// </summary>
        /// <param name="encodedData">The String containing the characters to decode.</param>
        /// <returns>A String containing the results of decoding the specified sequence of bytes.</returns>
        public static string DecodeFrom64(string encodedData)
        {
            var encodedDataAsBytes = Convert.FromBase64String(encodedData);
            return System.Text.Encoding.UTF8.GetString(encodedDataAsBytes);
        }

        /// <summary>
        /// The method create a Base64 encoded string from a normal string.
        /// </summary>
        /// <param name="toEncode">The String containing the characters to encode.</param>
        /// <returns>The Base64 encoded string.</returns>
        public static string EncodeTo64(string toEncode)
        {
            var toEncodeAsBytes = System.Text.Encoding.UTF8.GetBytes(toEncode);

            return Convert.ToBase64String(toEncodeAsBytes);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            RegisterClientScriptHelper.DataDealFullCardView(Page, TargetDeal);
            AllDealMilestones = Global.DaoFactory.GetDealMilestoneDao().GetAll();

            ExecHistoryView();
            RegisterScript();
        }

        #endregion

        #region Methods

        private void ExecHistoryView()
        {
            var historyViewControl = (HistoryView)LoadControl(HistoryView.Location);
            historyViewControl.TargetEntityID = TargetDeal.ID;
            historyViewControl.TargetEntityType = EntityType.Opportunity;

            _phHistoryView.Controls.Add(historyViewControl);
        }

        protected String RenderExpectedValue()
        {
            switch (TargetDeal.BidType)
            {
                case BidType.PerYear:
                    return String.Concat(CRMDealResource.BidType_PerYear, " ",
                                         String.Format(CRMJSResource.PerPeriodYears, TargetDeal.PerPeriodValue));
                case BidType.PerWeek:
                    return String.Concat(CRMDealResource.BidType_PerWeek, " ",
                                         String.Format(CRMJSResource.PerPeriodWeeks, TargetDeal.PerPeriodValue));
                case BidType.PerMonth:
                    return String.Concat(CRMDealResource.BidType_PerMonth, " ",
                                         String.Format(CRMJSResource.PerPeriodMonths, TargetDeal.PerPeriodValue));
                case BidType.PerHour:
                    return String.Concat(CRMDealResource.BidType_PerHour, " ",
                                         String.Format(CRMJSResource.PerPeriodHours, TargetDeal.PerPeriodValue));
                case BidType.PerDay:
                    return String.Concat(CRMDealResource.BidType_PerDay, " ",
                                         String.Format(CRMJSResource.PerPeriodDays, TargetDeal.PerPeriodValue));
                default:
                    return String.Empty;
            }
        }

        protected String GetExpectedOrActualCloseDateStr()
        {
            if (TargetDeal.ActualCloseDate == DateTime.MinValue)
            {
                return TargetDeal.ExpectedCloseDate == DateTime.MinValue ?
                                                CRMJSResource.NoCloseDate :
                                                TargetDeal.ExpectedCloseDate.ToString(DateTimeExtension.ShortDatePattern);
            }
            return TargetDeal.ActualCloseDate.ToString(DateTimeExtension.ShortDatePattern);

        }

        protected String GetExpectedValueStr()
        {
            if (TargetDeal.BidValue == 0)
                return CRMDealResource.NoExpectedValue;

            var currencyInfo = CurrencyProvider.Get(TargetDeal.BidCurrency);

            return String.Format("{2}{0:N} {1} <br/> <span>{3}</span>", TargetDeal.BidValue,
                                 currencyInfo.Abbreviation, currencyInfo.Symbol,
                                 RenderExpectedValue());

        }

        private void RegisterScript()
        {
            var script = @"ASC.CRM.DealFullCardView.init();";

            Page.RegisterInlineScript(script);
        }

        #endregion
    }
}