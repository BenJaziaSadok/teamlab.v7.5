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
    public class ViewDocumentHandler : IHttpHandler
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
                int id = Convert.ToInt32(context.Request.QueryString["attachid"]);
                ViewDocument(id, context);
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

        private void ViewDocument(int attachment_id, HttpContext context)
        {
            var mail_box_manager = new MailBoxManager(ConfigurationManager.ConnectionStrings["mail"], 0);
            var file = mail_box_manager.GetMessageAttachment(attachment_id, TenantId, Username);
            var temp_file_url = file.GerPreSignedUrl();
            var viewer_url = CommonLinkUtility.GetFileWebViewerExternalUrl(temp_file_url, file.fileName);
            viewer_url = new UriBuilder(CommonLinkUtility.GetFullAbsolutePath(viewer_url)).ToString();
            context.Response.Redirect(viewer_url, false);
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
