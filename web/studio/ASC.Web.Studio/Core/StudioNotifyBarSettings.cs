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

namespace ASC.Web.Studio.Core
{
    [Serializable]
    [DataContract]
    public class StudioNotifyBarSettings : ISettings
    {
        [DataMember(Name = "ShowPromotions")]
        public bool ShowPromotions { get; set; }

        #region ISettings Members

        public Guid ID
        {
            get { return new Guid("{D291A4C1-179D-4ced-895A-E094E809C859}"); }
        }

        public ISettings GetDefault()
        {
            return new StudioNotifyBarSettings() { ShowPromotions = true };
        }

        #endregion
    }
}