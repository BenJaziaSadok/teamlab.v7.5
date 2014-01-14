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

namespace ASC.Common.Security.Authorizing
{
    [Serializable]
    public sealed class Role : IRole
    {
        public const string Everyone = "Everyone";
        public const string Visitors = "Visitors";
        public const string Users = "Users";
        public const string Administrators = "Administrators";
        public const string System = "System";


        public Guid ID { get; internal set; }

        public string Name { get; internal set; }

        public string AuthenticationType
        {
            get { return "ASC"; }
        }

        public bool IsAuthenticated
        {
            get { return false; }
        }


        public Role(Guid id, string name)
        {
            if (id == Guid.Empty) throw new ArgumentException("id");
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            ID = id;
            Name = name;
        }


        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var r = obj as Role;
            return r != null && r.ID == ID;
        }

        public override string ToString()
        {
            return string.Format("Role: {0}", Name);
        }
    }
}