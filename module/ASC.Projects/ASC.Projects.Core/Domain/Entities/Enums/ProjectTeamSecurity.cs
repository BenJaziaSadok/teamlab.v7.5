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

#region Usings

using System;

#endregion

namespace ASC.Projects.Core.Domain
{
    [Flags]
    public enum ProjectTeamSecurity
    {
        None = 0,
        Messages = 1,
        Tasks = 2,
        Files = 4,
        Milestone = 8,
        Contacts = 16
    }
}
