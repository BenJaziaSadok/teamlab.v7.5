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
using ASC.Web.CRM.Classes;
using ASC.Web.Studio.Core;
using ASC.Web.CRM.Resources;

#endregion

namespace ASC.Web.CRM.Controls.Settings
{
    public partial class DealMilestoneView : BaseUserControl
    {
        #region Members

        public static string Location { get { return PathProvider.GetFileStaticRelativePath("Settings/DealMilestoneView.ascx"); } }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            _manageDealMilestonePopup.Options.IsPopup = true;
            RegisterClientScriptHelper.DataDealMilestoneView(Page);

            RegisterScript();
        }

        private void RegisterScript()
        {
            var sb = new StringBuilder();

            sb.AppendFormat(@"
                    ASC.CRM.DealMilestoneView.init();

                    ASC.CRM.DealMilestoneView.EditDealMilestoneHeaderText = '{0}';
                    ASC.CRM.DealMilestoneView.AddDealMilestoneHeaderText = '{1}';
                    ASC.CRM.DealMilestoneView.AddDealMilestoneButtonText = '{2}';
                    ASC.CRM.DealMilestoneView.AddDealMilestoneProcessText = '{3}';
                    ASC.CRM.DealMilestoneView.EditDealMilestoneProcessText = '{4}';",
                CRMSettingResource.EditSelectedDealMilestone.ReplaceSingleQuote(),
                CRMSettingResource.CreateNewDealMilestone.ReplaceSingleQuote(),
                CRMSettingResource.AddThisDealMilestone.ReplaceSingleQuote(),
                CRMSettingResource.CreateDealMilestoneInProgressing.ReplaceSingleQuote(),
                CRMCommonResource.SaveChangesProggress.ReplaceSingleQuote()
            );

            Page.RegisterInlineScript(sb.ToString());
        }

        #endregion
    }
}


