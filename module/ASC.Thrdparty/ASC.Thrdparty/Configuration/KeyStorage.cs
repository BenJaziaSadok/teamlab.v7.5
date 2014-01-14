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
using System.Configuration;
using System.IO;
using System.Web.Configuration;

namespace ASC.Thrdparty.Configuration
{
    public static class KeyStorage
    {
        public static string Get(string keyName)
        {
            var section = ConsumerConfigurationSection.GetSection();
            return section!=null ? section.Keys.GetKeyValue(keyName) : string.Empty;
        }

    }
}