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
using ASC.CRM.Core;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Controls.Common;
using System.Web;

#endregion

namespace ASC.Web.CRM.Controls.Cases
{
    public partial class CasesFullCardView : BaseUserControl
    {
        #region Property

        public static string Location
        {
            get { return PathProvider.GetFileStaticRelativePath("Cases/CasesFullCardView.ascx"); }
        }

        public ASC.CRM.Core.Entities.Cases TargetCase { get; set; }

        #endregion

        #region Events


        protected void Page_Load(object sender, EventArgs e)
        {
            RegisterClientScriptHelper.DataCasesFullCardView(Page, TargetCase);
            ExecHistoryView();
            RegisterScript();
        }

        #endregion

        #region Methods

        public void ExecHistoryView()
        {
            var historyViewControl = (HistoryView) LoadControl(HistoryView.Location);

            historyViewControl.TargetEntityType = EntityType.Case;
            historyViewControl.TargetEntityID = TargetCase.ID;
            historyViewControl.TargetContactID = 0;

            _phHistoryView.Controls.Add(historyViewControl);
        }

        private void RegisterScript()
        {
            var script = @"ASC.CRM.CasesFullCardView.init();";

            Page.RegisterInlineScript(script);
        }

        #endregion
    }
}