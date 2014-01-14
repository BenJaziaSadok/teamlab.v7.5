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
using AjaxPro;
using ASC.Core;
using ASC.Web.Core.Utility.Settings;
using System.Linq;

namespace ASC.Web.Studio.Core.HelpCenter
{
    [AjaxNamespace("UserVideoGuideUsage")]
    [Serializable]
    [DataContract]
    public class UserVideoSettings : ISettings
    {
        public Guid ID
        {
            get { return new Guid("{CEBD4BA5-31B3-43a4-93BF-B4A110FE840F}"); }
        }

        [DataMember(Name = "VideoGuides")]
        private List<String> VideoGuides { get; set; }

        public ISettings GetDefault()
        {
            return new UserVideoSettings
                {
                    VideoGuides = new List<String>()
                };
        }

        public static List<string> GetUserVideoGuide()
        {
            return SettingsManager.Instance.LoadSettingsFor<UserVideoSettings>(SecurityContext.CurrentAccount.ID).VideoGuides ?? new List<string>();
        }

        [AjaxMethod]
        public void SaveWatchVideo(String[] video)
        {
            var watched = GetUserVideoGuide();

            watched.AddRange(video);
            watched = watched.Distinct().ToList();
            var setting = new UserVideoSettings { VideoGuides = watched };
            SettingsManager.Instance.SaveSettingsFor(setting, SecurityContext.CurrentAccount.ID);
        }
    }
}