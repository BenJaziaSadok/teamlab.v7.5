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
using ASC.Common.Security;
using ASC.Common.Security.Authorizing;

namespace ASC.Core
{
    [Serializable]
    public class AzRecord
    {
        public Guid SubjectId { get; private set; }

        public Guid ActionId { get; private set; }

        public string ObjectId { get; private set; }

        public AceType Reaction { get; private set; }

        public int Tenant { get; set; }


        public AzRecord(Guid subjectId, Guid actionId, AceType reaction)
            : this(subjectId, actionId, reaction, default(string))
        {
        }

        public AzRecord(Guid subjectId, Guid actionId, AceType reaction, ISecurityObjectId objectId)
            : this(subjectId, actionId, reaction, AzObjectIdHelper.GetFullObjectId(objectId))
        {
        }


        internal AzRecord(Guid subjectId, Guid actionId, AceType reaction, string objectId)
        {
            SubjectId = subjectId;
            ActionId = actionId;
            Reaction = reaction;
            ObjectId = objectId;
        }

        public override bool Equals(object obj)
        {
            var r = obj as AzRecord;
            return r != null &&
                r.Tenant == Tenant &&
                r.SubjectId == SubjectId &&
                r.ActionId == ActionId &&
                r.ObjectId == ObjectId &&
                r.Reaction == Reaction;
        }

        public override int GetHashCode()
        {
            return Tenant.GetHashCode() ^
                SubjectId.GetHashCode() ^
                ActionId.GetHashCode() ^
                (ObjectId ?? string.Empty).GetHashCode() ^
                Reaction.GetHashCode();
        }
    }
}
