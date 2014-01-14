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
    public class StudioTrustedDomainSettings : ISettings
    {
        [DataMember(Name = "InviteUsersAsVisitors")]
        public bool InviteUsersAsVisitors { get; set; }

        #region ISettings Members

        public Guid ID
        {
            get { return new Guid("{00A2DB01-BAE3-48aa-BE32-CE768D7C874E}"); }
        }

        public ISettings GetDefault()
        {
            return new StudioTrustedDomainSettings { InviteUsersAsVisitors = false };
        }

        #endregion
    }
}
