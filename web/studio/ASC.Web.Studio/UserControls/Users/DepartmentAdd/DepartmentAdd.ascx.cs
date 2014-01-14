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
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Studio.UserControls.Users
{
    public partial class DepartmentAdd : System.Web.UI.UserControl
    {
        public static string Location
        {
            get { return "~/UserControls/Users/DepartmentAdd/DepartmentAdd.ascx"; }
        }

        public Guid ProductID { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            _departmentAddContainer.Options.IsPopup = true;
            _depProductID.Value = ProductID.ToString();
            _departmentAddContainer.Options.InfoMessageText = "";
            _departmentAddContainer.Options.InfoType = InfoType.Alert;

            headSelector.IsLinkView = false;
            headSelector.DefaultGroupText = CustomResourceHelper.GetResource("EmployeeAllDepartments");
            headSelector.InputWidth = 340;
            headSelector.EmployeeType = EmployeeType.User;
        }

        public IEnumerable<UserInfo> GetSortedUsers()
        {
            return CoreContext.UserManager.GetUsers().SortByUserName();
        }
    }
}