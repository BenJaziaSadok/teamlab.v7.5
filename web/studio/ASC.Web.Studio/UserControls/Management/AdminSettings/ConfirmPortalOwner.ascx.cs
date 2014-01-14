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
using System.Web.UI;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Core.Utility.Skins;
using System.Web.UI.HtmlControls;
using System.Web;

namespace ASC.Web.Studio.UserControls.Management
{
    partial class ConfirmPortalOwner : System.Web.UI.UserControl
    {
        public static readonly string Location = "~/UserControls/Management/AdminSettings/ConfirmPortalOwner.ascx";

        protected string _newOwnerName = string.Empty;


        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Page.RegisterStyleControl(VirtualPathUtility.ToAbsolute("~/usercontrols/management/adminsettings/css/ownerconfirm.less"));

                var newOwner = Constants.LostUser;
                try
                {
                    newOwner = CoreContext.UserManager.GetUsers(new Guid(Request["uid"]));
                }
                catch { };
                if (Constants.LostUser.Equals(newOwner))
                {
                    throw new Exception(Resources.Resource.ErrorUserNotFound);
                }
                _newOwnerName = newOwner.DisplayUserName();

                if (IsPostBack)
                {
                    if (CoreContext.UserManager.IsUserInGroup(newOwner.ID, ASC.Core.Users.Constants.GroupVisitor.ID))
                    {
                        throw new Exception(Resources.Resource.ErrorUserNotFound);
                    }

                    var curTenant = CoreContext.TenantManager.GetCurrentTenant();
                    curTenant.OwnerId = newOwner.ID;
                    CoreContext.TenantManager.SaveTenant(curTenant);

                    _messageHolder.Visible = true;
                    _confirmContentHolder.Visible = false;
                }
                else
                {
                    _messageHolder.Visible = false;
                    _confirmContentHolder.Visible = true;
                }
            }
            catch (Exception err)
            {
                ((confirm)Page).ErrorMessage = err.Message.HtmlEncode();
                _messageHolder.Visible = true;
                _confirmContentHolder.Visible = false;
                log4net.LogManager.GetLogger("ASC.Web").Error(err);
            }
        }
    }
}