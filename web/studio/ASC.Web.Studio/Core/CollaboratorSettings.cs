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
    public class CollaboratorSettings: ISettings
    {
        [DataMember(Name = "FirstVisit")]
        public bool FirstVisit { get; set; }

        public Guid ID
        {
            get { return new Guid("{73537E08-17F6-4706-BFDA-1414108AA7D2}"); }
        }


        public ISettings GetDefault()
        {
            return new CollaboratorSettings()
            {
                FirstVisit = true
            };
        }
    }
}
