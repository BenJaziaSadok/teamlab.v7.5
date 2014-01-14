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
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using ASC.CRM.Core;
using ASC.Web.CRM.Controls.Common;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.CRM.Classes;
using ASC.Web.Studio.Utility;
using AjaxPro;
using ASC.Common.Threading.Progress;
using ASC.Core.Common.Logging;
using System.Collections.Generic;
using System.Globalization;

namespace ASC.Web.CRM.Controls.Sender
{
    [AjaxNamespace("AjaxPro.SmtpSender")]
    public partial class SmtpSender : BaseUserControl
    {
        public static String Location
        {
            get { return PathProvider.GetFileStaticRelativePath("sender/smtpsender.ascx"); }
        }

        protected FredCK.FCKeditorV2.FCKeditor Editor { get; set; }

        protected bool MobileVer = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            Utility.RegisterTypeForAjax(typeof(SmtpSender));
            
            MobileVer = Core.Mobile.MobileDetector.IsRequestMatchesMobile(this.Context);

            //fix for IE 10 && IE 11
            var browser = HttpContext.Current.Request.Browser.Browser;

            var userAgent = Context.Request.Headers["User-Agent"];
            var regExp = new Regex("MSIE 10.0", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var regExpIe11 = new Regex("(?=.*Trident.*rv:11.0).+", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (browser == "IE" && regExp.Match(userAgent).Success || regExpIe11.Match(userAgent).Success)
            {
                MobileVer = true;
            }

            if (!MobileVer && CRMSecurity.IsAdmin)
            {
                Editor = new FredCK.FCKeditorV2.FCKeditor
                {
                    ID = "fckEditor",
                    Height = 400,
                    BasePath = CommonControlsConfigurer.FCKEditorBasePath,
                    EditorAreaCSS = WebSkin.BaseCSSFileAbsoluteWebPath
                };
                phFckEditor.Controls.Add(Editor);

                var uploader = (FileUploader)LoadControl(FileUploader.Location);
                phFileUploader.Controls.Add(uploader);
            }

            Page.RegisterClientScript(typeof(Masters.ClientScripts.SmtpSenderData));
            RegisterScript();
        }

        protected string RenderTagSelector(bool isCompany)
        {
            var sb = new StringBuilder();
            var manager = new MailTemplateManager();
            var tags = manager.GetTags(isCompany);

            var current = tags[0].Category;
            sb.AppendFormat("<optgroup label='{0}'>", current);

            foreach (var tag in tags)
            {
                if (tag.Category != current)
                {
                    current = tag.Category;
                    sb.Append("</optgroup>");
                    sb.AppendFormat("<optgroup label='{0}'>", current);
                }

                sb.AppendFormat("<option value='{0}'>{1}</option>",
                                tag.Name,
                                tag.DisplayName);
            }

            sb.Append("</optgroup>");

            return sb.ToString();
        }

        private void RegisterScript()
        {
            var sb = new StringBuilder();

            sb.AppendFormat(@"ASC.CRM.SmtpSender.init({0},""{1}"");",
                MailSender.GetQuotas(),
                MobileVer ? string.Empty : Editor.ClientID
            );

            Page.RegisterInlineScript(sb.ToString());
        }

        #region AjaxMethods

        [AjaxMethod]
        public IProgressItem SendEmail(List<int> fileIDs, List<int> contactIds, String subjectTemplate, String bodyTemplate, bool storeInHistory)
        {
            var joinFileIDs = string.Join("|", fileIDs.ConvertAll(x => x.ToString(CultureInfo.InvariantCulture)).ToArray());
            var joinContactIDs = string.Join("|", contactIds.ConvertAll(x => x.ToString(CultureInfo.InvariantCulture)).ToArray());
            AdminLog.PostAction("CRM: started email sending with parameters subjectTemlate={0},bodyTemplate={1},contactIDs={2},fileIDs={3}", subjectTemplate, bodyTemplate, joinContactIDs, joinFileIDs);

            return MailSender.Start(fileIDs, contactIds, subjectTemplate, bodyTemplate, storeInHistory);
        }

        [AjaxMethod]
        public IProgressItem GetStatus()
        {
            return MailSender.GetStatus();
        }

        [AjaxMethod]
        public IProgressItem Cancel()
        {
            var progressItem = GetStatus();

            MailSender.Cancel();

            AdminLog.PostAction("CRM: canceled mail sending");

            return progressItem;
        }

        [AjaxMethod]
        public string GetMessagePreview(string template, int contactId)
        {
            var manager = new MailTemplateManager();

            return manager.Apply(template, contactId);
        }

        #endregion
    }
}