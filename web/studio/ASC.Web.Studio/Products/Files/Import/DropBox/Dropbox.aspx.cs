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
using AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox;

namespace ASC.Web.Files.Import.DropBox
{
    public partial class Dropbox : OAuthBase
    {
        public static string Location
        {
            get { return CommonLinkUtility.FilesBaseAbsolutePath + "import/dropbox/dropbox.aspx"; }
        }

        private const string Source = "dropbox";

        private const string RequestTokenSessionKey = "requestToken";
        private const string AuthorizationUrlKey = "authorization";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session.IsReadOnly)
            {
                SubmitError("No session is availible.", Source);
                return;
            }

            var config = CloudStorage.GetCloudConfigurationEasy(nSupportedCloudConfigurations.DropBox) as DropBoxConfiguration;
            var callbackUri = new UriBuilder(Request.GetUrlRewriter());
            if (!string.IsNullOrEmpty(Request.QueryString[AuthorizationUrlKey]) && Session[RequestTokenSessionKey] != null)
            {
                //Authorization callback
                try
                {
                    var accessToken = DropBoxStorageProviderTools.ExchangeDropBoxRequestTokenIntoAccessToken(config,
                                                                                                             ImportConfiguration.DropboxAppKey,
                                                                                                             ImportConfiguration.DropboxAppSecret,
                                                                                                             Session[RequestTokenSessionKey] as DropBoxRequestToken);

                    Session[RequestTokenSessionKey] = null; //Exchanged
                    var storage = new CloudStorage();
                    var base64token = storage.SerializeSecurityTokenToBase64Ex(accessToken, config.GetType(), new Dictionary<string, string>());
                    storage.Open(config, accessToken); //Try open storage!
                    var root = storage.GetRoot();
                    if (root == null) throw new Exception();

                    SubmitToken(base64token, Source);
                }
                catch
                {
                    SubmitError("Failed to open storage with token", Source);
                }
            }
            else
            {
                callbackUri.Query += string.Format("&{0}=1", AuthorizationUrlKey);
                config.AuthorizationCallBack = callbackUri.Uri;
                // create a request token
                var requestToken = DropBoxStorageProviderTools.GetDropBoxRequestToken(config, ImportConfiguration.DropboxAppKey,
                                                                                      ImportConfiguration.DropboxAppSecret);
                var authorizationUrl = DropBoxStorageProviderTools.GetDropBoxAuthorizationUrl(config, requestToken);
                Session[RequestTokenSessionKey] = requestToken; //Store token into session!!!
                Response.Redirect(authorizationUrl);
            }
        }
    }
}