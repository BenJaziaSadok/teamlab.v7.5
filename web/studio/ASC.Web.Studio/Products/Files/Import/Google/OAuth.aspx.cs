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
using System.Threading;
using ASC.Thrdparty;
using ASC.Web.Studio.Utility;
using DotNetOpenAuth.ApplicationBlock;
using DotNetOpenAuth.OAuth;

namespace ASC.Web.Files.Import.Google
{
    public partial class OAuth : OAuthBase
    {
        public static string Location
        {
            get { return CommonLinkUtility.FilesBaseAbsolutePath + "import/google/oauth.aspx"; }
        }

        private const string Source = "google";

        protected string AccessToken
        {
            get { return TokenHolder.GetToken("GoogleAccessToken"); }
            set { TokenHolder.AddToken("GoogleAccessToken", value); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //NOTE: removed. so every page will provide google auth dialog
                if (AccessToken != null)
                {
                    //Previously stored token. clean it
                    ImportConfiguration.GoogleTokenManager.ExpireToken(AccessToken);
                }

                //Authenticating when loading pages
                if (!IsPostBack)
                {
                    using (
                        var google = new WebConsumer(GoogleConsumer.ServiceDescription,
                                                     ImportConfiguration.GoogleTokenManager))
                    {
                        // Is Google calling back with authorization?
                        var accessTokenResponse = google.ProcessUserAuthorization();
                        if (accessTokenResponse != null)
                        {
                            AccessToken = accessTokenResponse.AccessToken;
                            //Redirecting to result page
                            SubmitToken(AccessToken, Source);
                        }
                        else
                        {
                           GoogleConsumer.RequestAuthorization(google, "https://docs.google.com/feeds/ https://spreadsheets.google.com/feeds/ https://docs.googleusercontent.com/");

                        }
                    }
                }
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception ex)
            {
                SubmitError(ex.Message, Source);
            }
        }
    }
}