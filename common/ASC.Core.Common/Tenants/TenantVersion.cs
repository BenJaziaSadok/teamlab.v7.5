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

namespace ASC.Core
{
    [Serializable]
    public class TenantVersion
    {
        public int Id
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }


        public TenantVersion(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var v = obj as TenantVersion;
            return v != null && v.Id == Id;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
