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

namespace ASC.Common.Security.Authentication
{
    [Serializable]
    public class Account : IAccount
    {
        public Account(Guid id, string name, bool authenticated)
        {
            ID = id;
            Name = name;
            IsAuthenticated = authenticated;
        }

        #region IAccount Members

        public Guid ID { get; private set; }

        public string Name { get; private set; }


        public object Clone()
        {
            return MemberwiseClone();
        }

        public string AuthenticationType
        {
            get { return "ASC"; }
        }

        public virtual bool IsAuthenticated
        {
            get;
            private set;
        }

        #endregion

        public override bool Equals(object obj)
        {
            var a = obj as IAccount;
            return a != null && ID.Equals(a.ID);
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}