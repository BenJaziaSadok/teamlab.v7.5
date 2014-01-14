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
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Core.ModuleManagement.Common;
using ASC.Web.Core.Utility;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.Utility;
using Resources;

namespace ASC.Web.People.Core
{
    public class SearchHandler : BaseSearchHandlerEx
    {
        public override Guid ProductID
        {
            get { return PeopleProduct.ID; }
        }

        public override Guid ModuleID
        {
            get { return ProductID; }
        }

        public override ImageOptions Logo
        {
            get { return new ImageOptions { ImageFileName = "product_search_icon.png", PartID = ProductID }; }
        }

        public override string SearchName
        {
            get { return Resource.EmployeesSearch; }
        }

        public override IItemControl Control
        {
            get { return new CommonResultsView(); }
        }

        public override SearchResultItem[] Search(string text)
        {
            var users = new List<UserInfo>();

            users.AddRange(CoreContext.UserManager.Search(text, EmployeeStatus.Active));

            return users.Select(user => new SearchResultItem
                {
                    Name = user.DisplayUserName(false),
                    Description = string.Format("{2}{1} {0}", user.Department, !String.IsNullOrEmpty(user.Department) ? "," : String.Empty, user.Title),
                    URL = CommonLinkUtility.GetUserProfile(user.ID),
                    Date = user.WorkFromDate,
                    Additional = new Dictionary<string, object> { { "imageRef", user.GetSmallPhotoURL() }, { "showIcon", true }, { "Hint", Resources.PeopleResource.ProductName } }
                }).ToArray();
        }
    }
}