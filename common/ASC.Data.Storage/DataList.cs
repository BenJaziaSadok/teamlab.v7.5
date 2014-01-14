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

using System.Collections.Generic;
using ASC.Data.Storage.Configuration;

namespace ASC.Data.Storage
{
    public class DataList : Dictionary<string, string>
    {
        public DataList(ModuleConfigurationElement config)
        {
            Add(string.Empty, config.Data);
            foreach (DomainConfigurationElement domain in config.Domains)
            {
                Add(domain.Name, domain.Data);
            }
        }

        public string GetData(string name)
        {
            if (ContainsKey(name))
            {
                return this[name] ?? string.Empty;
            }
            return string.Empty;
        }
    }
}