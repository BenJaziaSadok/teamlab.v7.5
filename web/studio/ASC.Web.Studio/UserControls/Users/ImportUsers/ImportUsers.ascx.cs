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
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using AjaxPro;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Core.Utility;
using ASC.Web.Studio.Core.Import;
using ASC.Web.Studio.Core.Users;
using ASC.Web.Studio.UserControls.Statistics;
using ASC.Web.Studio.Utility;
using Constants = ASC.Core.Users.Constants;

namespace ASC.Web.Studio.UserControls.Users
{
    internal class ContactsUploader : IFileUploadHandler
    {
        #region IFileUploadHandler Members

        public FileUploadResult ProcessUpload(HttpContext context)
        {
            var result = new FileUploadResult();
            try
            {
                if (context.Request.Files.Count != 0)
                {
                    var logo = context.Request.Files[0];
                    var ext = FileUtility.GetFileExtension(logo.FileName);

                    if (ext != ".csv")
                    {
                        result.Success = false;
                        result.Message = Resources.Resource.ErrorEmptyUploadFileSelected;
                        return result;
                    }

                    IUserImporter importer = context.Request["obj"] == "txt"
                                                 ? new TextFileUserImporter(logo.InputStream) { DefaultHeader = "Email;FirstName;LastName", }
                                                 : new OutlookCSVUserImporter(logo.InputStream);

                    var users = importer.GetDiscoveredUsers();

                    result.Success = true;
                    result.Message = JsonContacts(users);
                }
                else
                {
                    result.Success = false;
                    result.Message = Resources.Resource.ErrorEmptyUploadFileSelected;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message.HtmlEncode();
            }

            return result;
        }

        private static string JsonContacts(IEnumerable<ContactInfo> contacts)
        {
            var serializer = new DataContractJsonSerializer(contacts.GetType());
            using (var ms = new MemoryStream())
            {
                serializer.WriteObject(ms, contacts);
                return Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int)ms.Length);
            }
        }

        #endregion
    }

    [AjaxNamespace("ImportUsersController")]
    public partial class ImportUsers : System.Web.UI.UserControl
    {
        protected bool EnableInviteLink = TenantStatisticsProvider.GetUsersCount() < TenantExtra.GetTenantQuota().ActiveUsers;

        protected bool IsMobileVersion
        {
            get { return Web.Core.Mobile.MobileDetector.IsRequestMatchesMobile(Context); }
        }

        public enum Operation
        {
            Success = 1,
            Error = 0,
            LimitExceeded = -1
        }

        public static string Location
        {
            get { return "~/UserControls/Users/ImportUsers/ImportUsers.ascx"; }
        }

        protected int PeopleLimit { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            var quota = TenantExtra.GetTenantQuota().ActiveUsers - TenantStatisticsProvider.GetUsersCount();
            PeopleLimit = quota > 0 ? quota : 0;

            icon.Options.IsPopup = true;
            icon.Options.PopupContainerCssClass = "okcss popupContainerClass";
            icon.Options.OnCancelButtonClick = "ImportUsersManager.HideInfoWindow('okcss');";

            limitPanel.Options.IsPopup = true;
            limitPanel.Options.OnCancelButtonClick = "ImportUsersManager.HideImportUserLimitPanel();";

            AjaxPro.Utility.RegisterTypeForAjax(GetType());

            RegisterScript();
        }

        private void RegisterScript()
        {
            Page.RegisterStyleControl(VirtualPathUtility.ToAbsolute("~/usercontrols/users/importusers/css/import.less"));

            Page.RegisterBodyScripts(ResolveUrl("~/js/uploader/ajaxupload.js"));
            Page.RegisterBodyScripts(ResolveUrl("~/js/third-party/zeroclipboard.js"));
            Page.RegisterBodyScripts(ResolveUrl("~/usercontrols/users/ImportUsers/js/ImportUsers.js"));

            var script = new StringBuilder();

            script.AppendFormat("ImportUsersManager.FName = '{0}';", Resources.Resource.ImportContactsFirstName.ReplaceSingleQuote());
            script.AppendFormat("ImportUsersManager.EmptyFName = '{0}';", Resources.Resource.ImportContactsEmptyFirstName.ReplaceSingleQuote());
            script.AppendFormat("ImportUsersManager.LName = '{0}';", Resources.Resource.ImportContactsLastName.ReplaceSingleQuote().Replace("\n", ""));
            script.AppendFormat("ImportUsersManager.EmptyLName = '{0}';", Resources.Resource.ImportContactsEmptyLastName.ReplaceSingleQuote());
            script.AppendFormat("ImportUsersManager.Email = '{0}';", Resources.Resource.ImportContactsEmail.ReplaceSingleQuote());
            script.AppendFormat("ImportUsersManager._errorImport = '{0}';", String.Format(Resources.Resource.ImportContactsFromFileError.ReplaceSingleQuote(), "<br />"));
            script.AppendFormat("ImportUsersManager._errorEmail = '{0}';", Resources.Resource.ImportContactsIncorrectFields.ReplaceSingleQuote());
            script.AppendFormat("ImportUsersManager._emptySocImport = '{0}';", String.Format(Resources.Resource.ImportContactsEmptyData.ReplaceSingleQuote().Replace("\n", ""), "<br />"));
            script.AppendFormat("ImportUsersManager._portalLicence.maxUsers = '{0}';", TenantExtra.GetTenantQuota().ActiveUsers);
            script.AppendFormat("ImportUsersManager._portalLicence.currectUsers = '{0}';", TenantStatisticsProvider.GetUsersCount());

            script.Append("jq(document).click(function(event) {");
            script.Append("jq.dropdownToggle().registerAutoHide(event, '.file', '.fileSelector');");
            script.Append("jq('#upload img').attr('src', StudioManager.GetImage('loader_16.gif'));");
            script.Append("});");

            Page.RegisterInlineScript(script.ToString());

            var sb = new StringBuilder();

            sb.AppendFormat(@"ZeroClipboard.setMoviePath('{0}');",
                CommonLinkUtility.ToAbsolute("~/js/flash/zeroclipboard/ZeroClipboard10.swf")
            );

            Page.RegisterInlineScript(sb.ToString(), true);
        }

        [AjaxMethod]
        public object SaveUsers(string userList, bool importUsersAsCollaborators)
        {
            if (!SecurityContext.CheckPermissions(Constants.Action_AddRemoveUser))
                return new { Status = (int)Operation.Error, Message = Resources.Resource.ErrorAccessDenied };

            var coll = new List<UserResults>();
            try
            {
                var jsSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var ruleObj = jsSerializer.Deserialize<List<UserData>>(userList);
                var error = 0;

                foreach (var userData in ruleObj)
                {
                    var validateEmail = UserManagerWrapper.ValidateEmail(userData.Email);
                    if (!validateEmail || String.IsNullOrEmpty(userData.FirstName) || String.IsNullOrEmpty(userData.LastName))
                    {
                        coll.Add(new UserResults
                            {
                                Email = userData.Email,
                                Result = Resources.Resource.ImportContactsIncorrectFields,
                                Class = !validateEmail ? "error3" : "error1"
                            });
                        error++;
                        continue;
                    }

                    var us = CoreContext.UserManager.GetUserByEmail(userData.Email);

                    if (us.ID != Constants.LostUser.ID)
                    {
                        coll.Add(new UserResults
                            {
                                Email = userData.Email,
                                Result = Resources.Resource.ImportContactsAlreadyExists,
                                Class = "error2"
                            });
                        error++;
                        continue;
                    }

                    if (error != 0) continue;

                    if (!importUsersAsCollaborators && TenantStatisticsProvider.GetUsersCount() >= TenantExtra.GetTenantQuota().ActiveUsers)
                    {
                        importUsersAsCollaborators = true;
                    }

                    UserManagerWrapper.AddUser(new UserInfo
                        {
                            Email = userData.Email,
                            FirstName = userData.FirstName,
                            LastName = userData.LastName
                        }, UserManagerWrapper.GeneratePassword(), false, true, importUsersAsCollaborators);
                    coll.Add(new UserResults { Email = userData.Email, Result = String.Empty });
                }
                return new { Status = (int)Operation.Success, Data = coll };
            }
            catch (Exception ex)
            {
                return new { Status = (int)Operation.Error, Message = ex.Message };
            }
        }

        internal class UserResults
        {
            public string Email { get; set; }
            public string Result { get; set; }
            public string Class { get; set; }
        }
    }
}