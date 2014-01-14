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
using System.Reflection;
using SmartAssembly.Attributes;

namespace ASC.Web.Core.Utility.Settings.Dao
{
	[Obfuscation(Exclude = true, ApplyToMembers = true)]
	[DoNotObfuscateType]
	internal class Settings
    {
        public virtual Guid ID { get; set; }
        public virtual Guid UserID { get; set; }
        public virtual byte[] Data { get; set; }
        public virtual int TenantID { get; set; }

        public override bool Equals(object obj)
        {
            Settings settings = obj as Settings;

            if (settings != null
                && settings.ID.Equals(this.ID)
                && settings.UserID.Equals(this.UserID)
                && settings.TenantID == this.TenantID)
                return true;

            return false;           
        }

        public override int GetHashCode()
        {
            return this.ID.GetHashCode() | this.UserID.GetHashCode() | TenantID; 
        }
    }
}
