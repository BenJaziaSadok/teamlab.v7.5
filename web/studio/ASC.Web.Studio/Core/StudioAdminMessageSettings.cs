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
    public class StudioAdminMessageSettings : ISettings
    {
        [DataMember(Name = "Enable")]
        public bool Enable { get; set; }

        #region ISettings Members

        public Guid ID
        {
            get { return new Guid("{28902650-58A9-11E1-B6A9-0F194924019B}"); }
        }

        public ISettings GetDefault()
        {
            return new StudioAdminMessageSettings { Enable = false };
        }

        #endregion
    }
}
