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
using System.Runtime.Serialization;
using ASC.Core;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Studio.Utility;
using System.Globalization;

namespace ASC.Web.Files.Classes
{
    [Serializable]
    [DataContract]
    public class FilesSettings : ISettings
    {
        private static readonly CultureInfo CultureInfo = CultureInfo.CreateSpecificCulture("en-US");

        [DataMember(Name = "EnableThirdpartySettings")]
        public bool EnableThirdpartySetting { get; set; }

        [DataMember(Name = "StoreOriginalFiles")]
        public bool StoreOriginalFilesSetting { get; set; }

        [DataMember(Name = "UpdateIfExist")]
        public bool UpdateIfExistSetting { get; set; }

        [DataMember(Name = "ExternalIP")]
        public KeyValuePair<bool, string> CheckExternalIPSetting { get; set; }

        public ISettings GetDefault()
        {
            return new FilesSettings
                {
                    EnableThirdpartySetting = true,
                    StoreOriginalFilesSetting = true,
                    UpdateIfExistSetting = false,
                    CheckExternalIPSetting = new KeyValuePair<bool, string>(true, DateTime.MinValue.ToString(CultureInfo))
                };
        }

        public Guid ID
        {
            get { return new Guid("{03B382BD-3C20-4f03-8AB9-5A33F016316E}"); }
        }

        public static bool EnableThirdParty
        {
            set
            {
                var setting = new FilesSettings
                    {
                        EnableThirdpartySetting = value
                    };
                SettingsManager.Instance.SaveSettings(setting, TenantProvider.CurrentTenantID);
            }
            get { return SettingsManager.Instance.LoadSettings<FilesSettings>(TenantProvider.CurrentTenantID).EnableThirdpartySetting; }
        }

        public static bool StoreOriginalFiles
        {
            set
            {
                var setting = SettingsManager.Instance.LoadSettingsFor<FilesSettings>(SecurityContext.CurrentAccount.ID);
                setting.StoreOriginalFilesSetting = value;

                SettingsManager.Instance.SaveSettingsFor(setting, SecurityContext.CurrentAccount.ID);
            }
            get { return SettingsManager.Instance.LoadSettingsFor<FilesSettings>(SecurityContext.CurrentAccount.ID).StoreOriginalFilesSetting; }
        }

        public static bool UpdateIfExist
        {
            set
            {
                var setting = SettingsManager.Instance.LoadSettingsFor<FilesSettings>(SecurityContext.CurrentAccount.ID);
                setting.UpdateIfExistSetting = value;

                SettingsManager.Instance.SaveSettingsFor(setting, SecurityContext.CurrentAccount.ID);
            }
            get { return SettingsManager.Instance.LoadSettingsFor<FilesSettings>(SecurityContext.CurrentAccount.ID).UpdateIfExistSetting; }
        }

        public static KeyValuePair<bool, DateTime> CheckHaveExternalIP
        {
            set
            {
                var setting = new FilesSettings
                    {
                        CheckExternalIPSetting = new KeyValuePair<bool, string>(value.Key, DateTime.UtcNow.ToString(CultureInfo))
                    };
                SettingsManager.Instance.SaveSettings(setting, -1);
            }
            get
            {
                var pair = SettingsManager.Instance.LoadSettings<FilesSettings>(-1).CheckExternalIPSetting;
                return new KeyValuePair<bool, DateTime>(pair.Key, Convert.ToDateTime(pair.Value, CultureInfo));
            }
        }
    }
}