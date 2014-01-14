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
using System.Text;
using ASC.CRM.Core;
using ASC.CRM.Core.Entities;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Controls.Common;
using ASC.Web.CRM.Resources;

#endregion

namespace ASC.Web.CRM.Controls.Contacts
{
    public partial class ContactFullCardView : BaseUserControl
    {
        #region Properies

        public static string Location
        {
            get { return PathProvider.GetFileStaticRelativePath("Contacts/ContactFullCardView.ascx"); }
        }

        public Contact TargetContact { get; set; }

        protected bool MobileVer = false;

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            MobileVer = Core.Mobile.MobileDetector.IsRequestMatchesMobile(this.Context);

            RegisterClientScriptHelper.DataContactFullCardView(Page, TargetContact);

            ExecHistoryView();
            RegisterScript();
        }

        #endregion

        #region Methods

        protected void ExecHistoryView()
        {
            var historyViewControl = (HistoryView)LoadControl(HistoryView.Location);

            historyViewControl.TargetContactID = TargetContact.ID;
            historyViewControl.TargetEntityID = 0;
            historyViewControl.TargetEntityType = EntityType.Contact;

            _phHistoryView.Controls.Add(historyViewControl);
        }

        private void RegisterScript()
        {
            var sb = new StringBuilder();

            sb.AppendFormat(@"
                    ASC.CRM.ContactFullCardView.init({0},'{1}',{2},{3},{4},{5},""{6}"",""{7}"",{8});",
                TargetContact.ID,
                TargetContact.GetTitle().HtmlEncode().ReplaceSingleQuote(),
                (TargetContact is Company).ToString().ToLower(),
                TargetContact is Person ? ((Person)TargetContact).CompanyID : 0,
                Global.TenantSettings.ChangeContactStatusGroupAuto != null ? Global.TenantSettings.ChangeContactStatusGroupAuto.ToString().ToLower() : "null",
                Global.TenantSettings.AddTagToContactGroupAuto != null ? Global.TenantSettings.AddTagToContactGroupAuto.ToString().ToLower() : "null",
                Studio.Core.FileSizeComment.GetFileImageSizeNote(CRMContactResource.ContactPhotoInfo, true),
                ContactPhotoManager.GetMediumSizePhoto(0, (TargetContact is Company)),
                CRMSecurity.IsAdmin.ToString().ToLower()
            );

            Page.RegisterInlineScript(sb.ToString());
        }

        #endregion
    }
}