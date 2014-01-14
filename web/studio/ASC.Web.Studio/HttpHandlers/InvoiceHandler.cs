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
using System.Linq;
using System.Web;
using ASC.Common.Web;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Studio.Utility;
using log4net;
using MimeMapping = System.Web.MimeMapping;

namespace ASC.Web.Studio.HttpHandlers
{
    public class InvoiceHandler : IHttpHandler
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(InvoiceHandler));


        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                if (!SecurityContext.IsAuthenticated && !CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).IsAdmin())
                {
                    throw new HttpException(403, "Access denied.");
                }

                var pid = context.Request.QueryString["pid"];
                if (string.IsNullOrEmpty(pid))
                {
                    throw new HttpException(400, "Bad request.");
                }

                if (CoreContext.PaymentManager.GetTariffPayments(TenantProvider.CurrentTenantID).All(p => p.CartId != pid))
                {
                    throw new HttpException(403, "Access denied.");
                }

                var invoice = CoreContext.PaymentManager.GetPaymentInvoice(pid);
                if (invoice == null || string.IsNullOrEmpty(invoice.Sale))
                {
                    throw new HttpException(404, "Not found.");
                }

                var pdf = Convert.FromBase64String(invoice.Sale);

                context.Response.Clear();
                context.Response.ContentType = MimeMapping.GetMimeMapping(".pdf");
                context.Response.AddHeader("Content-Disposition", "inline; filename=\"" + pid + ".pdf\"");
                context.Response.AddHeader("Content-Length", pdf.Length.ToString());

                for (int i = 0, count = 1024; i < pdf.Length; i += count)
                {
                    context.Response.OutputStream.Write(pdf, i, Math.Min(count, pdf.Length - i));
                }
                context.Response.Flush();
            }
            catch (HttpException he)
            {
                context.Response.StatusCode = he.GetHttpCode();
                context.Response.Write(HttpUtility.HtmlEncode(he.Message));
            }
            catch (Exception error)
            {
                log.ErrorFormat("Url: {0} {1}", context.Request.Url, error);
                context.Response.StatusCode = 500;
                context.Response.Write(HttpUtility.HtmlEncode(error.Message));
            }
        }
    }
}