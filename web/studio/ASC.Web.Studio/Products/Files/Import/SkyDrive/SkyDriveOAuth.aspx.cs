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
using System.Web;
using ASC.Web.Studio.Utility;
using AppLimit.CloudComputing.SharpBox;
using AppLimit.CloudComputing.SharpBox.StorageProvider.SkyDrive.Authorization;

namespace ASC.Web.Files.Import.SkyDrive
{
    public partial class SkyDriveOAuth : OAuthBase
    {
        public static string Location { get { return CommonLinkUtility.FilesBaseAbsolutePath + "import/skydrive/skydriveoauth.aspx"; } }

        private const string Source = "skydrive";

        private const string AuthorizationCodeUrlKey = "code";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session.IsReadOnly)
            {
                SubmitError("No session is availible.", Source);
                return;
            }

            var redirectUri = Request.GetUrlRewriter().GetLeftPart(UriPartial.Path);

            if (!string.IsNullOrEmpty(Request[AuthorizationCodeUrlKey]))
            {
                //we ready to obtain and store token
                var authCode = Request[AuthorizationCodeUrlKey];
                var accessToken = SkyDriveAuthorizationHelper.GetAccessToken(ImportConfiguration.SkyDriveAppKey,
                                                                             ImportConfiguration.SkyDriveAppSecret,
                                                                             redirectUri,
                                                                             authCode);
                
                //serialize token
                var config = CloudStorage.GetCloudConfigurationEasy(nSupportedCloudConfigurations.SkyDrive);
                var storage = new CloudStorage();
                var base64AccessToken = storage.SerializeSecurityTokenToBase64Ex(accessToken, config.GetType(), new Dictionary<string, string>());

                //check and submit
                storage.Open(config, accessToken);
                var root = storage.GetRoot();
                if (root != null)
                {
                    SubmitToken(base64AccessToken, Source);
                }
                else
                {
                    SubmitError("Failed to open storage with token", Source);
                }
            }
            else
            {
                var authCodeUri = SkyDriveAuthorizationHelper.BuildAuthCodeUrl(ImportConfiguration.SkyDriveAppKey, null, redirectUri);
                Response.Redirect(authCodeUri);
            }
        }
    }
}
