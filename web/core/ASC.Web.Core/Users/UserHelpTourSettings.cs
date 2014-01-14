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
using AjaxPro;

namespace ASC.Web.Core.Users
{
    [Serializable]
    [DataContract]
    public class UserHelpTourSettings : ISettings
    {
        public Guid ID
        {
            get { return new Guid("{DF4B94B7-42C8-4fce-AAE2-D479F3B39BDD}"); }
        }

        [DataMember(Name = "ModuleHelpTour")]
        public Dictionary<Guid, int> ModuleHelpTour { get; set; }

        [DataMember(Name = "IsNewUser")]
        public bool IsNewUser { get; set; }

        public ISettings GetDefault()
        {
            return new UserHelpTourSettings
                       {
                           ModuleHelpTour = new Dictionary<Guid, int>(),
                           IsNewUser = false
                       };
        }
    }
        
    [AjaxNamespace("UserHelpTourUsage")]
    public class UserHelpTourHelper
    {
        private static UserHelpTourSettings Settings
        {
            get { return SettingsManager.Instance.LoadSettingsFor<UserHelpTourSettings>(SecurityContext.CurrentAccount.ID); }
            set { SettingsManager.Instance.SaveSettingsFor(value, SecurityContext.CurrentAccount.ID); }
        }

        public static bool IsNewUser
        {
            get { return Settings.IsNewUser; }
            set
            {
                var settings = Settings;
                settings.IsNewUser = value;
                Settings = settings;
            }
        }

        public static int GetStep(Guid module)
        {
            var setting = Settings;

            if (setting.IsNewUser)
                return setting.ModuleHelpTour.ContainsKey(module) ? setting.ModuleHelpTour[module] : 0;

            return -1;
        }

        [AjaxMethod]
        public void SetStep(Guid module, int step)
        {
            var settings = Settings;

            if (settings.ModuleHelpTour.ContainsKey(module))
            {
                settings.ModuleHelpTour[module] = step;
            }
            else
            {
                settings.ModuleHelpTour.Add(module, step);
            }

            Settings = settings;
        }
    }
}