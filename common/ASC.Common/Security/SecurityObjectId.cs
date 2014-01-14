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
using ASC.Common.Security.Authorizing;

namespace ASC.Common.Security
{
    [DebuggerDisplay("ObjectType: {ObjectType.Name}, SecurityId: {SecurityId}")]
    public class SecurityObjectId : ISecurityObjectId
    {
        public object SecurityId { get; private set; }

        public Type ObjectType { get; private set; }


        public SecurityObjectId(object id, Type objType)
        {
            if (objType == null) throw new ArgumentNullException("objType");

            SecurityId = id;
            ObjectType = objType;
        }

        public override int GetHashCode()
        {
            return AzObjectIdHelper.GetFullObjectId(this).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as SecurityObjectId;
            return other != null &&
                   Equals(AzObjectIdHelper.GetFullObjectId(other), AzObjectIdHelper.GetFullObjectId(this));
        }
    }
}