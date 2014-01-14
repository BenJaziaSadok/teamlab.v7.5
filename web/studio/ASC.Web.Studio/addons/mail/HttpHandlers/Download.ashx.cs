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
using System.Configuration;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Services;
using ASC.Core;
using ASC.Mail.Aggregator;
using ASC.Security.Cryptography;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Mail.HttpHandlers
{
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class DownloadHandler : IHttpHandler
    {
        private int TenantId
        {
            get { return CoreContext.TenantManager.GetCurrentTenant().TenantId; }
        }

        private string Username
        {
            get { return SecurityContext.CurrentAccount.ID.ToString(); }
        }

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                context.Response.ContentType = "application/octet-stream";
                context.Response.Charset = Encoding.UTF8.WebName;

                int id = Convert.ToInt32(context.Request.QueryString["attachid"]);

                DownloadFile(id, context);
            }
            catch (Exception)
            {
                context.Response.Redirect("404.html");
            }
            finally
            {
                context.Response.End();
            }
        }

        private void DownloadFile(int attachment_id, HttpContext context)
        {
            var mail_box_manager = new MailBoxManager(ConfigurationManager.ConnectionStrings["mail"], 0);

            var auth = context.Request[CommonLinkUtility.AuthKey];

            var open_temp_stream = false;

            if (!string.IsNullOrEmpty(auth))
            {
                var stream = context.Request.QueryString["stream"];

                if (!string.IsNullOrEmpty(stream))
                {
                    int validate_timespan;
                    int.TryParse(WebConfigurationManager.AppSettings["files.stream-url-minute"], out validate_timespan);
                    if (validate_timespan <= 0) validate_timespan = 5;

                    var validate_result = EmailValidationKeyProvider.ValidateEmailKey(attachment_id + stream, auth, TimeSpan.FromMinutes(validate_timespan));
                    if (validate_result != EmailValidationKeyProvider.ValidationResult.Ok)
                    {
                        var exc = new HttpException((int)HttpStatusCode.Forbidden, "You don't have enough permission to perform the operation");
                        //Global.Logger.Error(string.Format("{0} {1}: {2}", CommonLinkUtility.AuthKey, validateResult, context.Request.Url), exc);
                        throw exc;
                    }

                    open_temp_stream = true;
                }
            }

            using (var file = open_temp_stream
                                  ? mail_box_manager.GetAttachmentStream(attachment_id, TenantId)
                                  : mail_box_manager.GetAttachmentStream(attachment_id, TenantId, Username))
            {
                context.Response.AddHeader("Content-Disposition", ContentDispositionUtil.GetHeaderValue(file.FileName));
                file.FileStream.StreamCopyTo(context.Response.OutputStream);
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
