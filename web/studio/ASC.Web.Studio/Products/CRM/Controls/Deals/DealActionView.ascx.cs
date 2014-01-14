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
using System.Web;
using System.Web.UI.WebControls;
using System.Linq;
using ASC.CRM.Core;
using ASC.CRM.Core.Entities;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Resources;
using ASC.Web.CRM.Controls.Common;
using ASC.Core;
using ASC.Web.CRM.Services.NotifyService;
using ASC.Web.Studio.Controls.Users;
using ASC.Web.Studio.Core.Users;
using ASC.Core.Tenants;
using ASC.Core.Users;
using System.Text;

#endregion

namespace ASC.Web.CRM.Controls.Deals
{
    public partial class DealActionView : BaseUserControl
    {
        #region Members

        public static string Location
        {
            get { return PathProvider.GetFileStaticRelativePath("Deals/DealActionView.ascx"); }
        }

        public Deal TargetDeal { get; set; }

        protected bool HavePermission { get; set; }

        #endregion

        #region Events

        public bool IsSelectedBidCurrency(String abbreviation)
        {
            return TargetDeal != null ?
                       String.Compare(abbreviation, TargetDeal.BidCurrency) == 0 :
                       String.Compare(abbreviation, Global.TenantSettings.DefaultCurrency.Abbreviation, true) == 0;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (TargetDeal != null)
            {
                HavePermission = CRMSecurity.IsAdmin || TargetDeal.CreateBy == SecurityContext.CurrentAccount.ID;
            }
            else
            {
                HavePermission = true;
            }

            if (IsPostBack) return;

            if (TargetDeal == null)
            {
                saveDealButton.Text = CRMDealResource.AddThisDealButton;
                saveAndCreateDealButton.Text = CRMDealResource.AddThisAndCreateDealButton;

                cancelButton.Attributes.Add("href",
                                            Request.UrlReferrer != null && Request.Url != null && String.Compare(Request.UrlReferrer.PathAndQuery, Request.Url.PathAndQuery) != 0
                                                ? Request.UrlReferrer.OriginalString
                                                : "deals.aspx");
            }
            else
            {
                saveDealButton.Text = CRMCommonResource.SaveChanges;

                cancelButton.Attributes.Add("href", String.Format("deals.aspx?id={0}", TargetDeal.ID));
            }


            /* Block for advUserSelector */
            var ResponsibleSelectedUserId = TargetDeal == null ?
                                                SecurityContext.CurrentAccount.ID :
                                                (TargetDeal.ResponsibleID != Guid.Empty ? TargetDeal.ResponsibleID : Guid.Empty);

            var dealResponsibleSelector = new AdvancedUserSelector
                {
                    ID = "userSelector",
                    IsLinkView = false,
                    EmployeeType = EmployeeType.User,
                    SelectedUserId = ResponsibleSelectedUserId,
                    LinkText = CustomNamingPeople.Substitute<CRMCommonResource>("AddUser"),
                    ParentContainerHtmlSelector = "#AdvUserSelectorContainer"
                };
            AdvancedUserSelector.RegisterStartupScripts(Page, dealResponsibleSelector);


            RegisterClientScriptHelper.DataDealActionView(Page, TargetDeal);

            InitPrivatePanel();
            RegisterScript();
        }

        protected void InitPrivatePanel()
        {
            var cntrlPrivatePanel = (PrivatePanel)LoadControl(PrivatePanel.Location);

            cntrlPrivatePanel.CheckBoxLabel = CRMDealResource.PrivatePanelCheckBoxLabel;

            if (TargetDeal != null)
            {
                cntrlPrivatePanel.IsPrivateItem = CRMSecurity.IsPrivate(TargetDeal);
                if (cntrlPrivatePanel.IsPrivateItem)
                    cntrlPrivatePanel.SelectedUsers = CRMSecurity.GetAccessSubjectTo(TargetDeal);
            }

            var usersWhoHasAccess = new List<string> { CustomNamingPeople.Substitute<CRMCommonResource>("CurrentUser").HtmlEncode(), CRMDealResource.ResponsibleDeal };

            cntrlPrivatePanel.UsersWhoHasAccess = usersWhoHasAccess;
            cntrlPrivatePanel.DisabledUsers = new List<Guid> { SecurityContext.CurrentAccount.ID };
            phPrivatePanel.Controls.Add(cntrlPrivatePanel);

        }

        protected void SaveOrUpdateDeal(Object sender, CommandEventArgs e)
        {
            int dealID;

            var deal = new Deal
                {
                    Title = Request["nameDeal"],
                    Description = Request["descriptionDeal"],
                    DealMilestoneID = Convert.ToInt32(Request["dealMilestone"])
                };

            int contactID;

            if (int.TryParse(Request["selectedContactID"], out contactID))
                deal.ContactID = contactID;

            int probability;

            if (int.TryParse(Request["probability"], out probability))
                deal.DealMilestoneProbability = probability;

            deal.BidCurrency = Request["bidCurrency"];

            if (String.IsNullOrEmpty(deal.BidCurrency))
                deal.BidCurrency = Global.TenantSettings.DefaultCurrency.Abbreviation;

            if (!String.IsNullOrEmpty(Request["bidValue"]))
            {
                decimal bidValue;

                if (!decimal.TryParse(Request["bidValue"], out bidValue))
                    bidValue = 0;

                deal.BidValue = bidValue;



                deal.BidType = (BidType)Enum.Parse(typeof(BidType), Request["bidType"]);

                if (deal.BidType != BidType.FixedBid)
                {
                    int perPeriodValue;

                    if (int.TryParse(Request["perPeriodValue"], out perPeriodValue))
                        deal.PerPeriodValue = perPeriodValue;
                }
            }
            else
            {
                deal.BidValue = 0;
                deal.BidType = BidType.FixedBid;
            }

            DateTime expectedCloseDate;

            if (!DateTime.TryParse(Request["expectedCloseDate"], out expectedCloseDate))
                expectedCloseDate = DateTime.MinValue;

            deal.ExpectedCloseDate = expectedCloseDate;

            deal.ResponsibleID = new Guid(Request["responsibleID"]);

            var dealMilestone = Global.DaoFactory.GetDealMilestoneDao().GetByID(deal.DealMilestoneID);

            if (TargetDeal == null)
            {
                if (dealMilestone.Status != DealMilestoneStatus.Open)
                    deal.ActualCloseDate = TenantUtil.DateTimeNow();


                dealID = Global.DaoFactory.GetDealDao().CreateNewDeal(deal);
                deal.ID = dealID;
                deal.CreateBy = SecurityContext.CurrentAccount.ID;
                deal.CreateOn = TenantUtil.DateTimeNow();
                deal = Global.DaoFactory.GetDealDao().GetByID(dealID);

                SetPermission(deal);

                if (deal.ResponsibleID != Guid.Empty && deal.ResponsibleID != SecurityContext.CurrentAccount.ID)
                    NotifyClient.Instance.SendAboutResponsibleForOpportunity(deal);
            }
            else
            {
                dealID = TargetDeal.ID;
                deal.ID = TargetDeal.ID;
                deal.ActualCloseDate = TargetDeal.ActualCloseDate;

                if (TargetDeal.ResponsibleID != Guid.Empty && TargetDeal.ResponsibleID != deal.ResponsibleID)
                    NotifyClient.Instance.SendAboutResponsibleForOpportunity(deal);


                if (TargetDeal.DealMilestoneID != deal.DealMilestoneID)
                    deal.ActualCloseDate = dealMilestone.Status != DealMilestoneStatus.Open ? TenantUtil.DateTimeNow() : DateTime.MinValue;

                Global.DaoFactory.GetDealDao().EditDeal(deal);
                deal = Global.DaoFactory.GetDealDao().GetByID(dealID);
                SetPermission(deal);
            }

            var dealMembers = !String.IsNullOrEmpty(Request["selectedMembersID"])
                                  ? Request["selectedMembersID"].Split(new[] { ',' }).Select(
                                      id => Convert.ToInt32(id)).Where(id => id != deal.ContactID).ToList()
                                  : new List<int>();

            if (deal.ContactID > 0 && !dealMembers.Contains(deal.ContactID))
                dealMembers.Add(deal.ContactID);

            Global.DaoFactory.GetDealDao().SetMembers(dealID, dealMembers.ToArray());


            foreach (var customField in Request.Form.AllKeys)
            {
                if (!customField.StartsWith("customField_")) continue;

                var fieldID = Convert.ToInt32(customField.Split('_')[1]);

                var fieldValue = Request.Form[customField];

                if (String.IsNullOrEmpty(fieldValue) && TargetDeal == null)
                    continue;

                Global.DaoFactory.GetCustomFieldDao().SetFieldValue(EntityType.Opportunity, dealID, fieldID, fieldValue);

            }

            if (TargetDeal == null && UrlParameters.ContactID != 0)
                Response.Redirect(String.Format("default.aspx?id={0}#deals", UrlParameters.ContactID));

            Response.Redirect(String.Compare(e.CommandArgument.ToString(), "0", true) == 0
                                  ? String.Format("deals.aspx?id={0}", dealID)
                                  : "deals.aspx?action=manage");
        }

        #endregion

        #region Methods

        protected void SetPermission(Deal deal)
        {
            if (CRMSecurity.IsAdmin || deal.CreateBy == SecurityContext.CurrentAccount.ID)
            {
                var isPrivate = false;
                var notifyPrivateUsers = false;

                bool value;
                if (bool.TryParse(Request.Form["isPrivateDeal"], out value))
                {
                    isPrivate = value;
                }
                if (bool.TryParse(Request.Form["notifyPrivateUsers"], out value))
                {
                    notifyPrivateUsers = value;
                }

                if (isPrivate)
                {
                    var selectedUsers = Request.Form["selectedPrivateUsers"]
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(item => new Guid(item)).ToList();

                    var responsibleID = new Guid(Request["responsibleID"]);
                    if (responsibleID != SecurityContext.CurrentAccount.ID)
                    {
                        selectedUsers.Add(responsibleID);
                    }

                    if (notifyPrivateUsers)
                        Services.NotifyService.NotifyClient.Instance.SendAboutSetAccess(EntityType.Opportunity, deal.ID, selectedUsers.ToArray());

                    selectedUsers.Add(SecurityContext.CurrentAccount.ID);

                    CRMSecurity.SetAccessTo(deal, selectedUsers);
                }
                else
                {
                    CRMSecurity.MakePublic(deal);
                }
            }
        }

        private void RegisterScript()
        {
            var sb = new StringBuilder();

            sb.AppendFormat(@"ASC.CRM.DealActionView.init(""{0}"",""{1}"",""{2}"",""{3}"",{4},{5},{6});",
                System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator,
                (int)System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator[0],
                DateTimeExtension.DateMaskForJQuery,
                TenantUtil.DateTimeNow().ToString(DateTimeExtension.DateFormatPattern),
                (int)ContactSelectorTypeEnum.CompaniesAndPersonsWithoutCompany,
                (int)ContactSelectorTypeEnum.All,
                CRMSecurity.IsAdmin.ToString().ToLower()
            );

            Page.RegisterInlineScript(sb.ToString());
        }

        #endregion
    }
}