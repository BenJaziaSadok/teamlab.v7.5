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

using System;
using System.Web;
using ASC.CRM.Core;
using ASC.Web.CRM.Resources;
using ASC.Web.CRM.Classes;
using System.Text;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.CRM.Configuration;

namespace ASC.Web.CRM.Controls.Cases
{
    public partial class CasesDetailsView : BaseUserControl
    {
        #region Properies

        public static string Location
        {
            get { return PathProvider.GetFileStaticRelativePath("Cases/CasesDetailsView.ascx"); }
        }

        public ASC.CRM.Core.Entities.Cases TargetCase { get; set; }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            ExecFullCardView();
            ExecPeopleInCaseView();
            ExecFilesView();
            RegisterScript();
        }

        #endregion

        #region Methods

        public void ExecPeopleInCaseView()
        {
            RegisterClientScriptHelper.DataListContactTab(Page, TargetCase.ID, EntityType.Case);
        }

        protected void ExecFullCardView()
        {
            var dealFullCardControl = (CasesFullCardView)LoadControl(CasesFullCardView.Location);
            dealFullCardControl.TargetCase = TargetCase;
            _phProfileView.Controls.Add(dealFullCardControl);
        }

        public void ExecFilesView()
        {
            var filesViewControl = (Studio.UserControls.Common.Attachments.Attachments)LoadControl(Studio.UserControls.Common.Attachments.Attachments.Location);
            filesViewControl.EntityType = "case";
            filesViewControl.ModuleName = "crm";
            _phFilesView.Controls.Add(filesViewControl);
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
                    ASC.CRM.ListTaskView.initTab(0,'{0}',{1},{2},{3},{4},'{5}');
                    ASC.CRM.CasesDetailsView.init('{6}');",
                EntityType.Case.ToString().ToLower(),
                TargetCase.ID,
                Global.EntryCountOnPage,
                Global.VisiblePageCount,
                (int)HistoryCategorySystem.TaskClosed,
                WebImageSupplier.GetAbsoluteWebPath("empty_screen_tasks.png", ProductEntryPoint.ID),
                WebImageSupplier.GetAbsoluteWebPath("empty_screen_case_participants.png", ProductEntryPoint.ID)
            );

            Page.RegisterInlineScript(sb.ToString());
        }

        #endregion
    }
}