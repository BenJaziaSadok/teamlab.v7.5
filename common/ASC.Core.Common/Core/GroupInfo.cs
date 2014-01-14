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

using ASC.Common.Security.Authorizing;
using ASC.Notify.Recipients;
using System;

namespace ASC.Core.Users
{
    [Serializable]
    public class GroupInfo : IRole, IRecipientsGroup
    {
        public Guid ID { get; internal set; }

        public string Name { get; set; }

        public Guid CategoryID { get; set; }

        public GroupInfo Parent { get; internal set; }
        

        public GroupInfo()
        {
        }

        public GroupInfo(Guid categoryID)
        {
            CategoryID = categoryID;
        }


        public override string ToString()
        {
            return Name;
        }

        public override int GetHashCode()
        {
            return ID != Guid.Empty ? ID.GetHashCode() : base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var g = obj as GroupInfo;
            if (g == null) return false;
            if (ID == Guid.Empty && g.ID == Guid.Empty) return ReferenceEquals(this, g);
            return g.ID == ID;
        }


        string IRecipient.ID
        {
            get { return ID.ToString(); }
        }

        string IRecipient.Name
        {
            get { return Name; }
        }

        public string AuthenticationType
        {
            get { return "ASC"; }
        }

        public bool IsAuthenticated
        {
            get { return false; }
        }
    }
}