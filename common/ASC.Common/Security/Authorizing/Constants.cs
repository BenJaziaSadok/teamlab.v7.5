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
    public sealed class Constants
    {
        public static readonly Role Admin = new Role(new Guid("cd84e66b-b803-40fc-99f9-b2969a54a1de"), "Admin");

        public static readonly Role Everyone = new Role(new Guid("c5cc67d1-c3e8-43c0-a3ad-3928ae3e5b5e"), "Everyone");


        public static readonly Role User = new Role(new Guid("abef62db-11a8-4673-9d32-ef1d8af19dc0"), "User");

        public static readonly Role Visitor = new Role(new Guid("aced04fa-dd96-4b35-af3e-346bf1eb972d"), "Visitor");


        public static readonly Role Member = new Role(new Guid("ba74ca02-873f-43dc-8470-8620c156bc67"), "Member");

        public static readonly Role Owner = new Role(new Guid("bba32183-a14d-48ed-9d39-c6b4d8925fbf"), "Owner");

        public static readonly Role Self = new Role(new Guid("5d5b7260-f7f7-49f1-a1c9-95fbb6a12604"), "Self");
    }
}