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
using ASC.Web.CRM.Configuration;
using ASC.Web.Core.Utility.Skins;
using ASC.CRM.Core;
using ASC.CRM.Core.Entities;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Resources;
using System.Text;

#endregion

namespace ASC.Web.CRM.Controls.Deals
{
    public partial class DealDetailsView : BaseUserControl
    {
        #region Property

        public Deal TargetDeal { get; set; }

        public static String Location
        {
            get { return PathProvider.GetFileStaticRelativePath("Deals/DealDetailsView.ascx"); }
        }

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
            ExecFullCardView();
            ExecPeopleView();
            ExecFilesView();
            RegisterScript();
        }

        #endregion

        #region Methods

        protected void ExecFullCardView()
        {
            var dealFullCardControl = (DealFullCardView)LoadControl(DealFullCardView.Location);
            dealFullCardControl.TargetDeal = TargetDeal;

            _phProfileView.Controls.Add(dealFullCardControl);
        }

        private void ExecPeopleView()
        {
            RegisterClientScriptHelper.DataListContactTab(Page, TargetDeal.ID, EntityType.Opportunity);
        }

        private void ExecFilesView()
        {
            var filesViewControl = (Studio.UserControls.Common.Attachments.Attachments)LoadControl(Studio.UserControls.Common.Attachments.Attachments.Location);
            filesViewControl.EntityType = "opportunity";
            filesViewControl.ModuleName = "crm";
            _phFilesView.Controls.Add(filesViewControl);

            //var mainContent = (MainContent) LoadControl(MainContent.Location);
            //mainContent.FolderIDCurrentRoot = FilesIntegration.RegisterBunch("crm", "opportunity", TargetDeal.ID.ToString());
            //mainContent.TitlePage = "crm";
            //_phFilesView.Controls.Add(mainContent);

            //if (FileUtility.ExtsImagePreviewed.Count != 0)
            //    _phFilesView.Controls.Add(LoadControl(FileViewer.Location));

            //if (!Core.Mobile.MobileDetector.IsRequestMatchesMobile(Context))
            //    _phFilesView.Controls.Add(LoadControl(ChunkUploadDialog.Location));

        }

        private void RegisterScript()
        {
            var sb = new StringBuilder();

            sb.Append(@"
                var hash = window.location.hash;
                hash = hash.charAt(0) == '#' ? hash.substring(1) : hash;
                if (!hash) {
                    window.location.hash = 'profile';
                }");

            sb.AppendFormat(@"
                    ASC.CRM.ListTaskView.initTab(0,""{0}"",{1},{2},{3},{4},""{5}"");
                    ASC.CRM.DealDetailsView.init(""{6}"");",
                EntityType.Opportunity.ToString().ToLower(),
                TargetDeal.ID,
                Global.EntryCountOnPage,
                Global.VisiblePageCount,
                (int)HistoryCategorySystem.TaskClosed,
                WebImageSupplier.GetAbsoluteWebPath("empty_screen_tasks.png", ProductEntryPoint.ID),
                WebImageSupplier.GetAbsoluteWebPath("empty_screen_opportunity_participants.png", ProductEntryPoint.ID)
            );

            Page.RegisterInlineScript(sb.ToString());
        }

        #endregion
    }
}