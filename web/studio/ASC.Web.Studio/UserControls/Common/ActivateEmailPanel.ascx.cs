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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ASC.Web.Studio.Core;
using ASC.Core.Users;
using ASC.Core;

namespace ASC.Web.Studio.UserControls.Common
{
    public partial class ActivateEmailPanel : UserControl
    {
        public static string Location
        {
            get { return "~/UserControls/Common/ActivateEmailPanel.ascx"; }
        }
        
        protected UserInfo CurrentUser
        {
            get { return CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            AjaxPro.Utility.RegisterTypeForAjax(typeof(EmailOperationService));

            var stringBuilder = new StringBuilder();
            stringBuilder.Append("if (jq('div.mainPageLayout table.mainPageTable').hasClass('with-mainPageTableSidePanel'))jq('.info-box.excl').removeClass('display-none');");

            Page.RegisterInlineScript(stringBuilder.ToString());
        }
    }
}