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
using System.IO;
using System.Runtime.Serialization;
using System.Web;
using ASC.Core;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Core.Utility
{
    [Serializable]
    [DataContract]
    public class ColorThemesSettings : ISettings
    {
        private const string DefaultName = "default";

        [DataMember(Name = "ColorThemeName")]
        public string ColorThemeName { get; set; }

        [DataMember(Name = "FirstRequest")]
        public bool FirstRequest { get; set; }

        public ISettings GetDefault()
        {
            return new ColorThemesSettings
                {
                    ColorThemeName = DefaultName,
                    FirstRequest = true
                };
        }

        public Guid ID
        {
            get { return new Guid("{AB5B3C97-A972-475C-BB13-71936186C4E6}"); }
        }

        public static string GetThemeFolderName(string path)
        {
            var folderName = GetColorThemesSettings();
            var resolvedPath = path.ToLower().Replace("<theme_folder>", folderName);

            if (!VirtualPathUtility.IsAbsolute(resolvedPath))
                resolvedPath = VirtualPathUtility.ToAbsolute(resolvedPath);

            var filePath = HttpContext.Current.Server.MapPath(resolvedPath);

            if (!File.Exists(filePath))
            {
                resolvedPath = path.ToLower().Replace("<theme_folder>", "default");

                if (!VirtualPathUtility.IsAbsolute(resolvedPath))
                    resolvedPath = VirtualPathUtility.ToAbsolute(resolvedPath);

                filePath = HttpContext.Current.Server.MapPath(resolvedPath);

                if (!File.Exists(filePath))
                    throw new FileNotFoundException("", path);
            }

            return resolvedPath;
        }

        public static string GetColorThemesSettings()
        {
            var colorTheme = SettingsManager.Instance.LoadSettings<ColorThemesSettings>(TenantProvider.CurrentTenantID);
            var colorThemeName = colorTheme.ColorThemeName;

            if (colorTheme.FirstRequest)
            {
                if (colorTheme.ColorThemeName == DefaultName)
                {
                    var partnerId = CoreContext.TenantManager.GetCurrentTenant().PartnerId;
                    if (!string.IsNullOrEmpty(partnerId))
                    {
                        var partner = CoreContext.PaymentManager.GetPartner(partnerId);
                        if (partner != null && partner.Status == PartnerStatus.Approved && !partner.Removed)
                        {
                            colorThemeName = partner.Theme;
                        }
                    }
                }

                SaveColorTheme(colorThemeName);
            }

            return colorThemeName;
        }

        public static void SaveColorTheme(string theme)
        {
            var settings = new ColorThemesSettings { ColorThemeName = theme, FirstRequest = false };
            SettingsManager.Instance.SaveSettings(settings, TenantProvider.CurrentTenantID);
        }
    }
}