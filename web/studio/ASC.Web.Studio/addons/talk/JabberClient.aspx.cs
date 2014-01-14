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
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using ASC.Core;
using ASC.Core.Users;
using ASC.Data.Storage;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio;
using ASC.Web.Studio.Controls.FileUploader.HttpModule;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;
using ASC.Web.Talk.Addon;
using ASC.Web.Talk.Resources;
using ASC.Xmpp.Common;
using log4net;

namespace ASC.Web.Talk
{
    public class UploadFileHanler : FileUploadHandler
    {
        private static String MD5Hash(String input)
        {
            var data = MD5.Create().ComputeHash(Encoding.Default.GetBytes(input));
            var sBuilder = new StringBuilder();

            for (Int32 i = 0, n = data.Length; i < n; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        public override FileUploadResult ProcessUpload(HttpContext context)
        {
            try
            {
                if (context.Request.Files.Count == 0)
                {
                    throw new Exception("there is no file");
                }

                var file = context.Request.Files[0];

                if (file.ContentLength > SetupInfo.MaxImageUploadSize)
                {
                    throw FileSizeComment.FileImageSizeException;
                }

                var userId = context.Request["ownjid"];
                var fileName = file.FileName.Replace("~", "-");
                var fileSize = file.InputStream.Length;
                var fileURL = StorageFactory.GetStorage(TenantProvider.CurrentTenantID.ToString(), "talk").Save(Path.Combine(MD5Hash(userId), fileName), file.InputStream).ToString();
                fileName = fileURL.Substring(fileURL.LastIndexOf("/") + 1);
                return new FileUploadResult
                    {
                        FileName = fileName,
                        Data = FileSizeComment.FilesSizeToString(fileSize),
                        FileURL = CommonLinkUtility.GetFullAbsolutePath(fileURL),
                        Success = true
                    };
            }
            catch (Exception ex)
            {
                return new FileUploadResult
                    {
                        Success = false,
                        Message = ex.Message
                    };
            }
        }
    }

    [AjaxPro.AjaxNamespace("JabberClient")]
    public partial class JabberClient : MainPage
    {
        private static string JabberResource
        {
            get { return "TMTalk"; }
        }

        private TalkConfiguration _cfg;

        private static String EscapeJsString(String s)
        {
            return s.Replace("'", "\\'");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _cfg = new TalkConfiguration();

            AjaxPro.Utility.RegisterTypeForAjax(GetType());

            Master.DisabledSidePanel = true;
            Master.DisabledTopStudioPanel = true;

            Page.RegisterBodyScripts(VirtualPathUtility.ToAbsolute("~/addons/talk/js/gears.init.js"));
            Page.RegisterBodyScripts(VirtualPathUtility.ToAbsolute("~/addons/talk/js/iscroll.js"));
            Page.RegisterBodyScripts(VirtualPathUtility.ToAbsolute("~/addons/talk/js/talk.customevents.js"));
            Page.RegisterBodyScripts(VirtualPathUtility.ToAbsolute("~/addons/talk/js/talk.common.js"));
            Page.RegisterBodyScripts(VirtualPathUtility.ToAbsolute("~/addons/talk/js/talk.navigationitem.js"));
            Page.RegisterBodyScripts(VirtualPathUtility.ToAbsolute("~/addons/talk/js/talk.msmanager.js"));
            Page.RegisterBodyScripts(VirtualPathUtility.ToAbsolute("~/addons/talk/js/talk.mucmanager.js"));
            Page.RegisterBodyScripts(VirtualPathUtility.ToAbsolute("~/addons/talk/js/talk.roomsmanager.js"));
            Page.RegisterBodyScripts(VirtualPathUtility.ToAbsolute("~/addons/talk/js/talk.contactsmanager.js"));
            Page.RegisterBodyScripts(VirtualPathUtility.ToAbsolute("~/addons/talk/js/talk.messagesmanager.js"));
            Page.RegisterBodyScripts(VirtualPathUtility.ToAbsolute("~/addons/talk/js/talk.connectiomanager.js"));
            Page.RegisterBodyScripts(VirtualPathUtility.ToAbsolute("~/addons/talk/js/talk.default.js"));

            //if (cfg.EnabledFirebugLite)
            //{
            //    Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "firebug.lite", "https://getfirebug.com/firebug-lite.js");
            //}

            var culture = CultureInfo.CurrentCulture;

            Page.RegisterStyleControl(VirtualPathUtility.ToAbsolute("~/addons/talk/css/default/talk.style.css"));
            Page.RegisterStyleControl(VirtualPathUtility.ToAbsolute("~/addons/talk/css/default/talk.style" + "." + culture.Name.ToLower() + ".css"));
            Page.RegisterStyleControl(VirtualPathUtility.ToAbsolute("~/addons/talk/css/default/talk.text-overflow.css"));


            switch (_cfg.RequestTransportType.ToLower())
            {
                case "flash":
                    Page.RegisterBodyScripts(VirtualPathUtility.ToAbsolute("~/addons/talk/js/jlib/plugins/strophe.flxhr.js"));

                    Page.RegisterBodyScripts(VirtualPathUtility.ToAbsolute("~/addons/talk/js/jlib/flxhr/checkplayer.js"));
                    Page.RegisterBodyScripts(VirtualPathUtility.ToAbsolute("~/addons/talk/js/jlib/flxhr/flensed.js"));
                    Page.RegisterBodyScripts(VirtualPathUtility.ToAbsolute("~/addons/talk/js/jlib/flxhr/flxhr.js"));
                    Page.RegisterBodyScripts(VirtualPathUtility.ToAbsolute("~/addons/talk/js/jlib/flxhr/swfobject.js"));

                    Page.RegisterBodyScripts(VirtualPathUtility.ToAbsolute("~/addons/talk/js/jlib/strophe/base64.js"));
                    Page.RegisterBodyScripts(VirtualPathUtility.ToAbsolute("~/addons/talk/js/jlib/strophe/md5.js"));
                    Page.RegisterBodyScripts(VirtualPathUtility.ToAbsolute("~/addons/talk/js/jlib/strophe/core.js"));

                    break;
                default:
                    Page.RegisterBodyScripts(VirtualPathUtility.ToAbsolute("~/addons/talk/js/jlib/strophe/base64.js"));
                    Page.RegisterBodyScripts(VirtualPathUtility.ToAbsolute("~/addons/talk/js/jlib/strophe/md5.js"));
                    Page.RegisterBodyScripts(VirtualPathUtility.ToAbsolute("~/addons/talk/js/jlib/strophe/core.js"));

                    Page.RegisterBodyScripts(VirtualPathUtility.ToAbsolute("~/addons/talk/js/jlib/flxhr/swfobject.js"));
                    break;
            }

            var jsResources = new StringBuilder();
            jsResources.Append("window.ASC=window.ASC||{};");
            jsResources.Append("window.ASC.TMTalk=window.ASC.TMTalk||{};");
            jsResources.Append("window.ASC.TMTalk.Resources={};");
            jsResources.Append("window.ASC.TMTalk.Resources.statusTitles={}" + ';');
            jsResources.Append("window.ASC.TMTalk.Resources.statusTitles.offline='" + EscapeJsString(TalkResource.StatusOffline) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.statusTitles.online='" + EscapeJsString(TalkResource.StatusOnline) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.statusTitles.away='" + EscapeJsString(TalkResource.StatusAway) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.statusTitles.xa='" + EscapeJsString(TalkResource.StatusNA) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.addonIcon='" + EscapeJsString(WebImageSupplier.GetAbsoluteWebPath("product_logo.png", TalkAddon.AddonID)) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.addonIcon16='" + EscapeJsString(WebImageSupplier.GetAbsoluteWebPath("talk16.png", TalkAddon.AddonID)) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.addonIcon32='" + EscapeJsString(WebImageSupplier.GetAbsoluteWebPath("talk32.png", TalkAddon.AddonID)) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.addonIcon48='" + EscapeJsString(WebImageSupplier.GetAbsoluteWebPath("talk48.png", TalkAddon.AddonID)) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.addonIcon128='" + EscapeJsString(WebImageSupplier.GetAbsoluteWebPath("talk128.png", TalkAddon.AddonID)) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.iconNewMessage='" + EscapeJsString(WebImageSupplier.GetAbsoluteWebPath("icon-new-message.ico", TalkAddon.AddonID)) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.productName='" + EscapeJsString(TalkResource.ProductName) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.updateFlashPlayerUrl='" + EscapeJsString(TalkResource.UpdateFlashPlayerUrl) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.selectUserBookmarkTitle='" + EscapeJsString(TalkResource.SelectUserBookmarkTitle) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.defaultConferenceSubjectTemplate='" + EscapeJsString(TalkResource.DefaultConferenceSubjectTemplate) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.labelNewMessage='" + EscapeJsString(TalkResource.LabelNewMessage) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.labelRecvInvite='" + EscapeJsString(TalkResource.LabelRecvInvite) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.titleRecvInvite='" + EscapeJsString(TalkResource.TitleRecvInvite) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.hintClientConnecting='" + EscapeJsString(TalkResource.HintClientConnecting) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.hintClientDisconnected='" + EscapeJsString(TalkResource.HintClientDisconnected) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.hintEmotions='" + EscapeJsString(TalkResource.HintEmotions) + "',");
            jsResources.Append("window.ASC.TMTalk.Resources.hintFlastPlayerIncorrect='" + EscapeJsString(TalkResource.HintFlastPlayerIncorrect) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.hintGroups='" + EscapeJsString(TalkResource.HintGroups) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.hintNoFlashPlayer='" + EscapeJsString(TalkResource.HintNoFlashPlayer) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.hintOfflineContacts='" + EscapeJsString(TalkResource.HintOfflineContacts) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.hintSendFile='" + EscapeJsString(TalkResource.HintSendFile) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.hintSounds='" + EscapeJsString(TalkResource.HintSounds) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.hintUpdateHrefText='" + EscapeJsString(TalkResource.HintUpdateHrefText) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.hintSelectContact='" + EscapeJsString(TalkResource.HintSelectContact) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.hintSendInvite='" + EscapeJsString(TalkResource.HintSendInvite) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.hintPossibleClientConflict='" + EscapeJsString(TalkResource.HintPossibleClientConflict) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.hintCreateShortcutDialog='" + EscapeJsString(TalkResource.HintCreateShortcutDialog) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.sendFileMessage='" + EscapeJsString(string.Format(TalkResource.SendFileMessage, "{0}<br/>", "{1}")) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.mailingsGroupName='" + EscapeJsString(TalkResource.MailingsGroupName) + "';");
            jsResources.Append("window.ASC.TMTalk.Resources.conferenceGroupName='" + EscapeJsString(TalkResource.ConferenceGroupName) + "';");

            Page.RegisterInlineScript(jsResources.ToString(), true, false);

            jsResources = new StringBuilder();

            jsResources.Append("TMTalk.init('ajaxupload.ashx?type=ASC.Web.Talk.UploadFileHanler,ASC.Web.Talk');");
            jsResources.Append("ASC.TMTalk.properties.init('2.0');");
            jsResources.Append("ASC.TMTalk.iconManager.init();");
            jsResources.AppendFormat("ASC.TMTalk.notifications.init('{0}', '{1}');", GetUserPhotoHandlerPath(), GetNotificationHandlerPath());
            jsResources.AppendFormat("ASC.TMTalk.msManager.init('{0}');", GetValidSymbols());
            jsResources.AppendFormat("ASC.TMTalk.mucManager.init('{0}');", GetValidSymbols());
            jsResources.Append("ASC.TMTalk.roomsManager.init();");
            jsResources.Append("ASC.TMTalk.contactsManager.init();");
            jsResources.AppendFormat("ASC.TMTalk.messagesManager.init('{0}', '{1}', '{2}', '{3}');", GetShortDateFormat(), GetFullDateFormat(), GetMonthNames(), GetHistoryLength());
            jsResources.AppendFormat("ASC.TMTalk.connectionManager.init('{0}', '{1}', '{2}', '{3}');", GetBoshUri(), GetJabberAccount(), GetResourcePriority(), GetInactivity());
            jsResources.AppendFormat("ASC.TMTalk.properties.item('addonID', '{0}');", TalkAddon.AddonID);
            jsResources.AppendFormat("ASC.TMTalk.properties.item('enabledMassend', '{0}');", GetMassendState());
            jsResources.AppendFormat("ASC.TMTalk.properties.item('enabledConferences', '{0}');", GetConferenceState());
            jsResources.AppendFormat("ASC.TMTalk.properties.item('requestTransportType', '{0}');", GetRequestTransportType());
            jsResources.AppendFormat("ASC.TMTalk.properties.item('fileTransportType', '{0}');", GetFileTransportType());
            jsResources.AppendFormat("ASC.TMTalk.properties.item('maxUploadSize', '{0}');", SetupInfo.MaxImageUploadSize);
            jsResources.AppendFormat("ASC.TMTalk.properties.item('sounds', '{0}');", VirtualPathUtility.ToAbsolute("~/addons/talk/swf/sounds.swf"));
            jsResources.AppendFormat("ASC.TMTalk.properties.item('expressInstall', '{0}');", VirtualPathUtility.ToAbsolute("~/addons/talk/swf/expressinstall.swf"));

            Page.RegisterInlineScript(jsResources.ToString());

            try
            {
                Page.Title = TalkResource.ProductName + " - " + CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).DisplayUserName();
            }
            catch (System.Security.SecurityException)
            {
                Page.Title = TalkResource.ProductName + " - " + HeaderStringHelper.GetPageTitle(TalkResource.DefaultContactTitle);
            }
        }

        [AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.ReadWrite), Core.Security.SecurityPassthrough]
        public string GetAuthToken()
        {
            try
            {
                return new JabberServiceClient().GetAuthToken(TenantProvider.CurrentTenantID);
            }
            catch (Exception ex)
            {
                LogManager.GetLogger("ASC.Talk").Error(ex);
                return String.Empty;
            }
        }

        protected String GetBoshUri()
        {
            return _cfg.BoshUri;
        }

        protected String GetResourcePriority()
        {
            return _cfg.ResourcePriority;
        }

        protected String GetInactivity()
        {
            return _cfg.ClientInactivity;
        }

        protected String GetFileTransportType()
        {
            return _cfg.FileTransportType ?? String.Empty;
        }

        protected String GetRequestTransportType()
        {
            return _cfg.RequestTransportType ?? String.Empty;
        }

        protected String GetMassendState()
        {
            return _cfg.EnabledMassend.ToString().ToLower();
        }

        protected String GetConferenceState()
        {
            return _cfg.EnabledConferences.ToString().ToLower();
        }

        protected String GetMonthNames()
        {
            return String.Join(",", CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedMonthNames);
        }

        protected String GetHistoryLength()
        {
            return _cfg.HistoryLength ?? String.Empty;
        }

        protected String GetValidSymbols()
        {
            return _cfg.ValidSymbols ?? String.Empty;
        }

        protected String GetFullDateFormat()
        {
            return TalkResource.FullDateFormat;
        }

        protected String GetShortDateFormat()
        {
            return TalkResource.ShortDateFormat;
        }

        protected String GetUserPhotoHandlerPath()
        {
            return VirtualPathUtility.ToAbsolute("~/addons/talk/userphoto.ashx");
        }

        protected String GetNotificationHandlerPath()
        {
            return VirtualPathUtility.ToAbsolute("~/addons/talk/notification.html");
        }

        protected String GetJabberAccount()
        {
            try
            {
                return EscapeJsString(CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).UserName.ToLower()) + "@" +
                       CoreContext.TenantManager.GetCurrentTenant().TenantDomain + "/" + JabberResource;
            }
            catch (Exception)
            {
                return String.Empty;
            }
        }
    }
}