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
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Resources;

#endregion

namespace ASC.Web.CRM.Controls.Settings
{
    public partial class ListItemView : BaseUserControl
    {
        #region Members

        public static string Location { get { return PathProvider.GetFileStaticRelativePath("Settings/ListItemView.ascx"); } }

        public ListType CurrentTypeValue { get; set;}
        
        public string AddButtonText { get; set; }
        public string AddListButtonText { get; set; }

        public string AddPopupWindowText { get; set; }

        public string EditText { get; set; }
        public string EditPopupWindowText { get; set; }

        public string AjaxProgressText { get; set; }
        public string DeleteText { get; set; }

        public string DescriptionText { get; set; }
        public string DescriptionTextEditDelete { get; set; }


        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            _manageItemPopup.Options.IsPopup = true;
            RegisterClientScriptHelper.DataListItemView(Page, CurrentTypeValue);

            RegisterScript();
        }

        private void RegisterScript()
        {
            var sb = new StringBuilder();

            sb.AppendFormat(@"
                    ASC.CRM.ListItemView.init({0});

                    ASC.CRM.ListItemView.AddItemHeaderText = '{1}';
                    ASC.CRM.ListItemView.AddItemButtonText = '{2}';
                    ASC.CRM.ListItemView.EditItemHeaderText = '{3}';
                    ASC.CRM.ListItemView.AddItemProcessText = '{4}';
                    ASC.CRM.ListItemView.EditItemProcessText = '{5}';",
                (int)CurrentTypeValue,
                AddPopupWindowText.ReplaceSingleQuote(),
                AddButtonText.ReplaceSingleQuote(),
                EditPopupWindowText.ReplaceSingleQuote(),
                AjaxProgressText.ReplaceSingleQuote(),
                CRMCommonResource.SaveChangesProggress.ReplaceSingleQuote()
            );

            Page.RegisterInlineScript(sb.ToString());
        }

        #endregion
    }
}