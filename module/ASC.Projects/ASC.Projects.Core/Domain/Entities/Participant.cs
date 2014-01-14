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
using System.Diagnostics;
using ASC.Core;
using ASC.Core.Users;
using System.Collections;

namespace ASC.Projects.Core.Domain
{
    [DebuggerDisplay("{UserInfo.ToString()}")]
    public class Participant : IComparable
    {
        public Guid ID { get; private set; }

        public int ProjectID { get; set; }

        public bool CanReadFiles { get; private set; }

        public bool CanReadMilestones { get; private set; }

        public bool CanReadMessages { get; private set; }

        public bool CanReadTasks { get; private set; }

        public bool CanReadContacts { get; set; }

        public bool IsVisitor { get; private set; }

        public bool IsFullAdmin { get; private set; }

        public UserInfo UserInfo { get; private set; }

        public bool IsAdmin { get; set; }

        public Participant(Guid userID)
        {
            ID = userID;
            UserInfo = CoreContext.UserManager.GetUsers(ID);
            IsVisitor = UserInfo.IsVisitor();
            IsFullAdmin = UserInfo.IsAdmin();
        }

        public Participant(Guid userID, ProjectTeamSecurity security)
            : this(userID)
        {
            CanReadFiles = (security & ProjectTeamSecurity.Files) != ProjectTeamSecurity.Files;
            CanReadMilestones = (security & ProjectTeamSecurity.Milestone) != ProjectTeamSecurity.Milestone;
            CanReadMessages = (security & ProjectTeamSecurity.Messages) != ProjectTeamSecurity.Messages;
            CanReadTasks = (security & ProjectTeamSecurity.Tasks) != ProjectTeamSecurity.Tasks;
            CanReadContacts = (security & ProjectTeamSecurity.Contacts) != ProjectTeamSecurity.Contacts;

            if (IsVisitor)
                CanReadContacts = false;
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var p = obj as Participant;
            return p != null && p.ID == ID;
        }

        public int CompareTo(object obj)
        {
            var other = obj as Participant;
            return other == null
                       ? Comparer.Default.Compare(this, obj)
                       : UserFormatter.Compare(UserInfo, other.UserInfo);
        }
    }
}