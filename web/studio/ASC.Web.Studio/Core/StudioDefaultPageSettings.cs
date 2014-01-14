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
    public class StudioDefaultPageSettings : ISettings
    {
        [DataMember(Name = "DefaultProductID")]
        public Guid DefaultProductID { get; set; }

        #region ISettings Members

        public Guid ID
        {
            get { return new Guid("{F3FF27C5-BDE3-43ae-8DD0-2E8E0D7044F1}"); }
        }

        public Guid FeedModuleID
        {
            get { return new Guid("{48328C27-4C85-4987-BA0E-D6BB17356B10}"); }
        }

        public ISettings GetDefault()
        {
            return new StudioDefaultPageSettings { DefaultProductID = Guid.Empty };
        }

        #endregion
    }
}
