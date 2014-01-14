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
using ASC.Common.Security.Authentication;
using ASC.Core.Users;

namespace ASC.Core.Security.Authentication
{
    [Serializable]
    class UserAccount : MarshalByRefObject, IUserAccount
    {
        public Guid ID { get; private set; }

        public string Name { get; private set; }

        public string AuthenticationType { get { return "ASC"; } }

        public bool IsAuthenticated { get { return true; } }

        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        public string Title { get; private set; }

        public string Department { get; private set; }

        public int Tenant { get; private set; }

        public string Email { get; private set; }


        public UserAccount(UserInfo info, int tenant)
        {
            ID = info.ID;
            Name = UserFormatter.GetUserName(info);
            FirstName = info.FirstName;
            LastName = info.LastName;
            Title = info.Title;
            Department = info.Department;
            Tenant = tenant;
            Email = info.Email;            
        }


        public object Clone()
        {
            return MemberwiseClone();
        }

        public override bool Equals(object obj)
        {
            var a = obj as IUserAccount;
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