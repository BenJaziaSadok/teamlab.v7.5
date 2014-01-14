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
using ASC.Web.Core;
using ASC.Web.Studio.Utility;
using Resources;

namespace ASC.Core.Users
{
    public static class StudioUserInfoExtension
    {
        public static string GetUserProfilePageURL(this UserInfo userInfo)
        {
            return userInfo == null ? "" : CommonLinkUtility.GetUserProfile(userInfo.ID);
        }

        public static string RenderProfileLink(this UserInfo userInfo, Guid productID)
        {
            var sb = new StringBuilder();

            if (userInfo == null || !CoreContext.UserManager.UserExists(userInfo.ID))
            {
                sb.Append("<span class='userLink text-medium-describe'>");
                sb.Append(Resource.ProfileRemoved);
                sb.Append("</span>");
            }
            else if (Array.Exists(Configuration.Constants.SystemAccounts, a => a.ID == userInfo.ID))
            {
                sb.Append("<span class='userLink text-medium-describe'>");
                sb.Append(userInfo.LastName);
                sb.Append("</span>");
            }
            else
            {
                var popupID = Guid.NewGuid();
                sb.AppendFormat("<span class=\"userLink\" id=\"{0}\" data-uid=\"{1}\" data-pid=\"{2}\">", popupID, userInfo.ID, productID);
                sb.AppendFormat("<a class='linkDescribe' href=\"{0}\">{1}</a>", userInfo.GetUserProfilePageURL(), userInfo.DisplayUserName());
                sb.Append("</span>");
            }
            return sb.ToString();
        }

        public static string RenderCustomProfileLink(this UserInfo userInfo, Guid productID, String containerCssClass, String linkCssClass)
        {
            var containerCss = string.IsNullOrEmpty(containerCssClass) ? "userLink" : "userLink " + containerCssClass;
            var linkCss = string.IsNullOrEmpty(linkCssClass) ? "" : linkCssClass;
            var sb = new StringBuilder();

            if (userInfo == null || !CoreContext.UserManager.UserExists(userInfo.ID))
            {
                sb.AppendFormat("<span class='{0}'>", containerCss);
                sb.Append(Resource.ProfileRemoved);
                sb.Append("</span>");
            }
            else if (Array.Exists(Configuration.Constants.SystemAccounts, a => a.ID == userInfo.ID))
            {
                sb.AppendFormat("<span class='{0}'>", containerCss);
                sb.Append(userInfo.LastName);
                sb.Append("</span>");
            }
            else
            {
                var popupID = Guid.NewGuid();
                sb.AppendFormat("<span class=\"{0}\" id=\"{1}\" data-uid=\"{2}\" data-pid=\"{3}\">", containerCss, popupID, userInfo.ID, productID);
                sb.AppendFormat("<a class='{0}' href=\"{1}\">{2}</a>", linkCss, userInfo.GetUserProfilePageURL(), userInfo.DisplayUserName());
                sb.Append("</span>");
            }
            return sb.ToString();
        }

        public static string RenderPopupInfoScript(this UserInfo userInfo, Guid productID, string elementID)
        {
            if (Equals(userInfo, Constants.LostUser)) return "";
            var sb = new StringBuilder();
            sb.Append("<script language='javascript'> StudioUserProfileInfo.RegistryElement('" + elementID + "','\"" + userInfo.ID + "\",\"" + productID + "\"'); </script>");
            return sb.ToString();
        }

        public static List<string> GetListAdminModules(this UserInfo ui)
        {
            var listModules = new List<string>();

            var productsForAccessSettings = WebItemManager.Instance.GetItemsAll<IProduct>().Where(n => String.Compare(n.GetSysName(), "people") != 0).ToList();

            foreach (var product in productsForAccessSettings)
            {
                if (WebItemSecurity.IsProductAdministrator(product.ID, ui.ID))
                {
                    listModules.Add(product.ProductClassName);
                }
            }

            return listModules;
        }
    }
}