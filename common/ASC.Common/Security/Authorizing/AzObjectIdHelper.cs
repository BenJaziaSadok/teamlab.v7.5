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

namespace ASC.Common.Security.Authorizing
{
    public static class AzObjectIdHelper
    {
        private static readonly string separator = "|";

        public static string GetFullObjectId(ISecurityObjectId objectId)
        {
            if (objectId == null) return null;
            return string.Format("{0}{1}{2}", objectId.ObjectType.FullName, separator, objectId.SecurityId);
        }
    }
}