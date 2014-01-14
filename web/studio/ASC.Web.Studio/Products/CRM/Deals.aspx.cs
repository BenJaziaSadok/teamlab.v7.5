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
using ASC.CRM.Core.Entities;
using ASC.Web.CRM.Controls.Common;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Controls.Deals;
using ASC.Web.CRM.Resources;
using ASC.Web.Studio.Utility;
using ASC.CRM.Core;

#endregion

namespace ASC.Web.CRM
{
    public partial class Deals : BasePage
    {
        protected override void PageLoad()
        {
            int dealID;

            if (int.TryParse(Request["id"], out dealID))
            {

                Deal targetDeal = Global.DaoFactory.GetDealDao().GetByID(dealID);

                if (targetDeal == null || !CRMSecurity.CanAccessTo(targetDeal))
                    Response.Redirect(PathProvider.StartURL() + "deals.aspx");

                if (String.Compare(Request["action"], "manage", true) == 0)
                    ExecDealActionView(targetDeal);
                else
                    ExecDealDetailsView(targetDeal);

            }
            else
            {
                if (String.Compare(Request["action"], "manage", true) == 0)
                    ExecDealActionView(null);
                else if (String.Compare(UrlParameters.Action, "import", true) == 0)
                    ExecImportView();
                else
                    ExecListDealView();
            }

        }

        #region Methods

        protected void ExecImportView()
        {
            var importViewControl = (ImportFromCSVView)LoadControl(ImportFromCSVView.Location);
            importViewControl.EntityType = EntityType.Opportunity;
            CommonContainerHolder.Controls.Add(importViewControl);

            Master.CurrentPageCaption = CRMDealResource.ImportDeals;
            Title = HeaderStringHelper.GetPageTitle(CRMDealResource.ImportDeals);
        }

        protected void ExecDealDetailsView(Deal targetDeal)
        {
            if (!CRMSecurity.CanAccessTo(targetDeal))
                Response.Redirect(PathProvider.StartURL());

            var dealActionViewControl = (DealDetailsView)LoadControl(DealDetailsView.Location);
            dealActionViewControl.TargetDeal = targetDeal;
            CommonContainerHolder.Controls.Add(dealActionViewControl);

            var headerTitle = targetDeal.Title.HtmlEncode();

            Master.CurrentPageCaption = headerTitle;

            Master.CommonContainerHeader = Global.RenderItemHeaderWithMenu(headerTitle, EntityType.Opportunity, CRMSecurity.IsPrivate(targetDeal));

            Title = HeaderStringHelper.GetPageTitle(headerTitle);
        }

        protected void ExecListDealView()
        {
            var listDealViewControl = (ListDealView)LoadControl(ListDealView.Location);
            CommonContainerHolder.Controls.Add(listDealViewControl);

            var headerTitle = CRMDealResource.AllDeals;
            if (!String.IsNullOrEmpty(Request["userID"])) headerTitle = CRMDealResource.MyDeals;
            Title = HeaderStringHelper.GetPageTitle(Master.CurrentPageCaption ?? headerTitle);
        }

        protected void ExecDealActionView(Deal targetDeal)
        {
            var dealActionViewControl = (DealActionView)LoadControl(DealActionView.Location);
            dealActionViewControl.TargetDeal = targetDeal;
            CommonContainerHolder.Controls.Add(dealActionViewControl);

            var headerTitle = targetDeal == null ? CRMDealResource.CreateNewDeal : String.Format(CRMDealResource.EditDealLabel, targetDeal.Title);
            Master.CurrentPageCaption = headerTitle;
            Title = HeaderStringHelper.GetPageTitle(headerTitle);
        }

        #endregion
    }
}