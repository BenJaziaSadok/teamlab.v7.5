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

#region usings

using System.Collections.Generic;
using ASC.Common.Security.Authorizing;

#endregion

namespace ASC.Common.Security
{
    public class SecurityCallContext
    {
        public SecurityCallContext()
        {
            ObjectsStack = new List<ISecurityObjectId>();
            RolesList = new List<IRole>();
        }

        public List<ISecurityObjectId> ObjectsStack { get; private set; }

        public List<IRole> RolesList { get; private set; }

        public object UserData { get; set; }
    }
}