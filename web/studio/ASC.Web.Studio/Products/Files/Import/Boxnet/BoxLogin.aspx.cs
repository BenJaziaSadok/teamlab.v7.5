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
using System.Net;
using System.Web;
using System.Xml.Linq;
using ASC.Thrdparty;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Files.Import.Boxnet
{
    public partial class BoxLogin : OAuthBase
    {
        public static string Location
        {
            get { return CommonLinkUtility.FilesBaseAbsolutePath + "import/boxnet/boxlogin.aspx"; }
        }

        protected const string Source = "boxnet";

        private const string InteractiveLoginRedirect = "https://www.box.com/api/1.0/auth/{0}";

        private string AuthToken
        {
            get { return TokenHolder.GetToken("box.net_auth_token"); }
            set { TokenHolder.AddToken("box.net_auth_token", value); }
        }

        private string AuthTicket
        {
            get { return TokenHolder.GetToken("box.net_auth_ticket"); }
            set { TokenHolder.AddToken("box.net_auth_ticket", value); }
        }

        protected string LoginUrl { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(AuthToken))
            {
                if (string.IsNullOrEmpty(Request["auth_token"]))
                {
                    try
                    {
                        //We are not authorized. get ticket and redirect
                        var url = "https://www.box.com/api/1.0/rest?action=get_ticket&api_key=" +
                                  ImportConfiguration.BoxNetApiKey;
                        var ticketResponce = new WebClient().DownloadString(url);
                        var response = XDocument.Parse(ticketResponce).Element("response");

                        if (response.Element("status").Value != "get_ticket_ok")
                        {
                            throw new InvalidOperationException("Can't retrieve ticket " + response.Element("status").Value + ".");
                        }

                        AuthTicket = response.Element("ticket").Value;
                        var loginRedir = string.Format(InteractiveLoginRedirect, AuthTicket);

                        var frameCallback = new Uri(ImportConfiguration.BoxNetIFrameAddress, UriKind.Absolute);
                        LoginUrl = string.Format("{0}?origin={1}&go={2}",
                                                 frameCallback,
                                                 HttpUtility.UrlEncode(new Uri(Request.GetUrlRewriter(), "/").ToString()),
                                                 HttpUtility.UrlEncode(loginRedir));
                    }
                    catch (Exception ex)
                    {
                        //Something goes wrong
                        SubmitError(ex.Message, Source);
                    }
                }
                else
                {
                    //We got token
                    AuthToken = Request["auth_token"];
                    //Now we can callback somewhere
                    SubmitToken(AuthToken, Source);
                }
            }
            else
            {
                SubmitToken(AuthToken, Source);
            }
        }
    }
}