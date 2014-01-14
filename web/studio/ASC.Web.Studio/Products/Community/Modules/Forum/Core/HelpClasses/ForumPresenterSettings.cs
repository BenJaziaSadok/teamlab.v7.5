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
using System.Reflection;

namespace ASC.Forum
{
	public class ForumPresenterSettings
    {
        public Guid ID { get; private set; }

        public ISecurityObject AdminSecurityObject { get; set; }

        public ForumPresenterSettings()
        {
            this.ID = Guid.NewGuid();
        }

    }
}
