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
using System.Web.Configuration;
using ASC.Thrdparty;
using ASC.Thrdparty.Configuration;
using ASC.Thrdparty.TokenManagers;

namespace ASC.Web.Files.Import
{
    public static class ImportConfiguration
    {
        private static string[] importProviders = (WebConfigurationManager.AppSettings["files.import.enable"] ?? "").Split(new char[] { '|', ',' }, StringSplitOptions.RemoveEmptyEntries);
        private static string[] thirdPartyProviders = (WebConfigurationManager.AppSettings["files.thirdparty.enable"] ?? "").Split(new char[] {'|', ','}, StringSplitOptions.RemoveEmptyEntries);

        public static bool SupportImport
        {
            get { return SupportBoxNetImport || SupportGoogleImport || SupportZohoImport; }
        }

        public static bool SupportBoxNetImport
        {
            get { return importProviders.Contains("boxnet") && !string.IsNullOrEmpty(BoxNetApiKey); }
        }

        public static bool SupportGoogleImport
        {
            get { return importProviders.Contains("google") && !string.IsNullOrEmpty(KeyStorage.Get("googleConsumerKey")); }
        }

        public static bool SupportZohoImport
        {
            get { return importProviders.Contains("zoho") && !string.IsNullOrEmpty(ZohoApiKey); }
        }

        public static bool SupportInclusion
        {
            get { return SupportBoxNetInclusion || SupportDropboxInclusion || SupportGoogleInclusion || SupportSkyDriveInclusion; }
        }

        public static bool SupportBoxNetInclusion
        {
            get { return thirdPartyProviders.Contains("boxnet") && !string.IsNullOrEmpty(BoxNetApiKey); }
        }

        public static bool SupportDropboxInclusion
        {
            get { return thirdPartyProviders.Contains("dropbox") && !string.IsNullOrEmpty(DropboxAppKey) && !string.IsNullOrEmpty(DropboxAppSecret); }
        }

        public static bool SupportGoogleInclusion
        {
            get { return thirdPartyProviders.Contains("google") && !string.IsNullOrEmpty(KeyStorage.Get("googleConsumerKey")); }
        }

        public static bool SupportSkyDriveInclusion
        {
            get { return thirdPartyProviders.Contains("skydrive") && !string.IsNullOrEmpty(SkyDriveAppKey) && !string.IsNullOrEmpty(SkyDriveAppSecret); }
        }

        public static string BoxNetApiKey
        {
            get { return KeyStorage.Get("box.net"); }
        }

        public static string BoxNetIFrameAddress
        {
            get { return KeyStorage.Get("box.net.framehandler"); }
        }

        public static IAssociatedTokenManager GoogleTokenManager 
        {
            get { return TokenManagerHolder.Get("google", "googleConsumerKey", "googleConsumerSecret"); }
        }

        public static string ZohoApiKey
        {
            get { return KeyStorage.Get("zoho"); }
        }

        public static string DropboxAppKey
        {
            get { return KeyStorage.Get("dropboxappkey"); }
        }

        public static string DropboxAppSecret
        {
            get { return KeyStorage.Get("dropboxappsecret"); }
        }

        public static string SkyDriveAppKey
        {
            get { return KeyStorage.Get("skydriveappkey"); }
        }

        public static string SkyDriveAppSecret
        {
            get { return KeyStorage.Get("skydriveappsecret"); }
        }
    }
}