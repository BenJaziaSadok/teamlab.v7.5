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
using System.Web.UI.WebControls;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Resources;
using ASC.CRM.Core;
using ASC.Web.CRM.Controls.Common;
using ASC.Core;
using ASC.Web.Studio.Core.Users;
using Newtonsoft.Json.Linq;
using System.Web;

#endregion

namespace ASC.Web.CRM.Controls.Cases
{
    public partial class CasesActionView : BaseUserControl
    {
        
        #region Properies

        public static string Location { get { return PathProvider.GetFileStaticRelativePath("Cases/CasesActionView.ascx"); } }

        public ASC.CRM.Core.Entities.Cases TargetCase { get; set; }

        protected bool HavePermission { get; set; }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            HavePermission = TargetCase == null ||
                             (CRMSecurity.IsAdmin || TargetCase.CreateBy == SecurityContext.CurrentAccount.ID);

            if (IsPostBack) return;

            if (TargetCase != null)
            {
                saveCaseButton.Text = CRMCasesResource.SaveChanges;
                cancelButton.Attributes.Add("href", String.Format("cases.aspx?id={0}", TargetCase.ID));
            }
            else
            {
                saveCaseButton.Text = CRMCasesResource.CreateThisCaseButton;
                saveAndCreateCaseButton.Text = CRMCasesResource.AddThisAndCreateCaseButton;
                cancelButton.Attributes.Add("href",
                             Request.UrlReferrer != null && Request.Url != null && String.Compare(Request.UrlReferrer.PathAndQuery, Request.Url.PathAndQuery) != 0
                                 ? Request.UrlReferrer.OriginalString
                                 : "cases.aspx");
            }

            RegisterClientScriptHelper.DataCasesActionView(Page, TargetCase);

            InitPrivatePanel();
            RegisterScript();
        }

        #endregion

        #region Methods

        public String GetCaseTitle()
        {
            return TargetCase == null ? String.Empty : TargetCase.Title.HtmlEncode();
        }

        protected void InitPrivatePanel()
        {
            var cntrlPrivatePanel = (PrivatePanel)LoadControl(PrivatePanel.Location);

            cntrlPrivatePanel.CheckBoxLabel = CRMCasesResource.PrivatePanelCheckBoxLabel;

            if (TargetCase != null)
            {
                cntrlPrivatePanel.IsPrivateItem = CRMSecurity.IsPrivate(TargetCase);
                if (cntrlPrivatePanel.IsPrivateItem)
                    cntrlPrivatePanel.SelectedUsers = CRMSecurity.GetAccessSubjectTo(TargetCase);
            }

            var usersWhoHasAccess = new List<string> { CustomNamingPeople.Substitute<CRMCommonResource>("CurrentUser").HtmlEncode() };

            cntrlPrivatePanel.UsersWhoHasAccess = usersWhoHasAccess;
            cntrlPrivatePanel.DisabledUsers = new List<Guid> { SecurityContext.CurrentAccount.ID };
            phPrivatePanel.Controls.Add(cntrlPrivatePanel);
        }

        protected void SaveOrUpdateCase(Object sender, CommandEventArgs e)
        {
            int caseID;

            if (TargetCase != null)
            {
                caseID = TargetCase.ID;
                TargetCase.Title = Request["caseTitle"];
                Global.DaoFactory.GetCasesDao().UpdateCases(TargetCase);
                SetPermission(TargetCase);
            }
            else
            {
                caseID = Global.DaoFactory.GetCasesDao().CreateCases(Request["caseTitle"]);
                var newCase = Global.DaoFactory.GetCasesDao().GetByID(caseID);
                SetPermission(newCase);
            }


            Global.DaoFactory.GetCasesDao().SetMembers(caseID,
                                                       !String.IsNullOrEmpty(Request["memberID"])
                                                           ? Request["memberID"].Split(',').Select(
                                                               id => Convert.ToInt32(id)).ToArray()
                                                           : new List<int>().ToArray());


            var assignedTags = Request["baseInfo_assignedTags"];
            if (assignedTags != null)
            {
                var oldTagList = Global.DaoFactory.GetTagDao().GetEntityTags(EntityType.Case, caseID);
                foreach (var tag in oldTagList)
                {
                    Global.DaoFactory.GetTagDao().DeleteTagFromEntity(EntityType.Case, caseID, tag);
                }
                if (assignedTags != string.Empty)
                {
                    var tagListInfo = JObject.Parse(assignedTags)["tagListInfo"].ToArray();
                    var newTagList = tagListInfo.Select(t => t.ToString()).ToArray();
                    Global.DaoFactory.GetTagDao().SetTagToEntity(EntityType.Case, caseID, newTagList);
                }
            }

            foreach (var customField in Request.Form.AllKeys)
            {
                if (!customField.StartsWith("customField_")) continue;
                int fieldID = Convert.ToInt32(customField.Split('_')[1]);
                String fieldValue = Request.Form[customField];
               
                if (String.IsNullOrEmpty(fieldValue) && TargetCase == null)
                    continue;
               
                Global.DaoFactory.GetCustomFieldDao().SetFieldValue(EntityType.Case,caseID, fieldID, fieldValue);
            }

            Response.Redirect(String.Compare(e.CommandArgument.ToString(), "0", true) == 0
                                  ? String.Format("cases.aspx?id={0}", caseID)
                                  : "cases.aspx?action=manage");
        }

        protected void SetPermission(ASC.CRM.Core.Entities.Cases targetCase, bool isPrivate, string selectedUsers)
        {
            if (isPrivate)
            {
                var selectedUserList = selectedUsers
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(item => new Guid(item)).ToList();

                CRMSecurity.MakePublic(targetCase);

                if (selectedUserList.Count > 0)
                    CRMSecurity.SetAccessTo(targetCase, selectedUserList);
            }
            else
            {
                CRMSecurity.MakePublic(targetCase);
            }
        }

        protected void SetPermission(ASC.CRM.Core.Entities.Cases caseItem)
        {
            if (CRMSecurity.IsAdmin || caseItem.CreateBy == SecurityContext.CurrentAccount.ID)
            {
                var isPrivate = false;
                var notifyPrivateUsers = false;

                bool value;
                if (bool.TryParse(Request.Form["isPrivateCase"], out value))
                {
                    isPrivate = value;
                }
                if (bool.TryParse(Request.Form["notifyPrivateUsers"], out value))
                {
                    notifyPrivateUsers = value;
                }

                if (isPrivate)
                {
                    var selectedUserList = Request["selectedUsersCase"]
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(item => new Guid(item)).ToList();

                    if (notifyPrivateUsers)
                        ASC.Web.CRM.Services.NotifyService.NotifyClient.Instance.SendAboutSetAccess(EntityType.Case, caseItem.ID, selectedUserList.ToArray());

                    selectedUserList.Add(ASC.Core.SecurityContext.CurrentAccount.ID);
                    CRMSecurity.SetAccessTo(caseItem, selectedUserList);
                }
                else
                {
                    CRMSecurity.MakePublic(caseItem);
                }
            }
        }

        private void RegisterScript()
        {
            var script = @"
                ASC.CRM.CasesActionView.init(
                    '" + DateTimeExtension.DateMaskForJQuery + @"',
                    " + (int)ContactSelectorTypeEnum.All + @",
                    " + CRMSecurity.IsAdmin.ToString().ToLower() + @");";

            Page.RegisterInlineScript(script);
        }

        #endregion
    }
}