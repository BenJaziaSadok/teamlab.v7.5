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
using System.Runtime.Serialization;
using ASC.Web.Core.Utility.Settings;


namespace ASC.Web.Community.Wiki.Common
{
    [Serializable]
    [DataContract]
    public class WikiModuleSettings: ISettings
    {

        #region static functions
        public static WikiModuleSettings GetSettings(Guid userID)
        {
            return SettingsManager.Instance.LoadSettingsFor<WikiModuleSettings>(userID);
        }

        public static bool SetSettings(WikiModuleSettings settings, Guid userID)
        {
            return SettingsManager.Instance.SaveSettingsFor<WikiModuleSettings>(settings, userID);
        }


        public static bool GetIsWysiwygDefault(Guid userID)
        {
            return GetSettings(userID).IsWysiwygDefault;
        }

        public static void SetIsWysiwygDefault(bool isWysiwygDefault, Guid userID)
        {
            WikiModuleSettings currentSettings = GetSettings(userID);
            currentSettings.IsWysiwygDefault = isWysiwygDefault;
            SetSettings(currentSettings, userID);
        }

        #endregion

        [DataMember(Name = "IsWysiwygDefault")]
        public bool IsWysiwygDefault { get; set; }

        #region ISettings Members 
        public ISettings GetDefault()
        {
            return new WikiModuleSettings() { IsWysiwygDefault = true };
        }

        public Guid ID
        {
            get { return new Guid("{9174797C-6CE3-40b8-852A-BC9D5F3AE794}"); }
        }

        #endregion
    }
}
