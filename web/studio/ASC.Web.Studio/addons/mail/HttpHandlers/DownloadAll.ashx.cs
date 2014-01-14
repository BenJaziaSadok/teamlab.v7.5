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
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Services;
using ASC.Core;
using ASC.Mail.Aggregator;
using ASC.Web.Studio.Utility;
using Ionic.Zip;
using Ionic.Zlib;

namespace ASC.Web.Mail.HttpHandlers
{
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class DownloadAllHandler : IHttpHandler
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
                if (MailPage.IsTurnOnAttachmentsGroupOperations())
                    throw new Exception("Operation is turned off.");

                context.Response.ContentType = "application/octet-stream";
                context.Response.Charset = Encoding.UTF8.WebName;

                int message_id = Convert.ToInt32(context.Request.QueryString["messageid"]);

                DownloadAllZipped(message_id, context);

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

        private const string ARCHIVE_NAME = "download.zip";

        private void DownloadAllZipped(int message_id, HttpContext context)
        {
            var mail_box_manager = new MailBoxManager(ConfigurationManager.ConnectionStrings["mail"], 0);

            var attachments = mail_box_manager.GetMessageAttachments(TenantId, Username, message_id);

            if (attachments.Any())
            {
                using (var zip = new ZipFile())
                {
                    zip.CompressionLevel = CompressionLevel.Level3;
                    zip.AlternateEncodingUsage = ZipOption.AsNecessary;
                    zip.AlternateEncoding = Encoding.GetEncoding(Thread.CurrentThread.CurrentCulture.TextInfo.OEMCodePage);

                    foreach (var attachment in attachments)
                    {
                        using (var file = mail_box_manager.GetAttachmentStream(attachment))
                        {
                            var filename = file.FileName;

                            if (zip.ContainsEntry(filename))
                            {
                                var counter = 1;
                                var temp_name = filename;
                                while (zip.ContainsEntry(temp_name))
                                {
                                    temp_name = filename;
                                    var suffix = " (" + counter + ")";
                                    temp_name = 0 < temp_name.IndexOf('.')
                                                   ? temp_name.Insert(temp_name.LastIndexOf('.'), suffix)
                                                   : temp_name + suffix;

                                    counter++;
                                }
                                filename = temp_name;
                            }

                            zip.AddEntry(filename, file.FileStream.GetCorrectBuffer());
                        }
                    }

                    context.Response.AddHeader("Content-Disposition", ContentDispositionUtil.GetHeaderValue(ARCHIVE_NAME));

                    zip.Save(context.Response.OutputStream);

                }

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
