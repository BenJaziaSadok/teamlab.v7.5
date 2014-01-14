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

using System.Diagnostics;

namespace ASC.Web.Studio.UserControls.Management
{
    public class AuthService
    {
        public string Name { get; private set; }

        public string Title { get; private set; }

        public AuthKey Id { get; private set; }
        
        public AuthKey Key { get; private set; }

        public AuthService(string name)
        {
            Name = name;
            Title = GetResourceString(name) ?? name;
        }

        public AuthService WithId(string keyName, string keyValue)
        {
            if (keyName != null)
                Id = new AuthKey(keyName, keyValue) {Title = GetResourceString(Name + "Key") ?? keyName};
            return this;
        }

        public AuthService WithKey(string keyName, string keyValue)
        {
            if (keyName != null)
                Key = new AuthKey(keyName, keyValue) {Title = GetResourceString(Name + "Secret") ?? keyName};
            return this;
        }

        private static string GetResourceString(string resourceKey)
        {
            try
            {
                return Resources.Resource.ResourceManager.GetString("Consumers" + resourceKey);
            }
            catch
            {
                return null;
            }
        }
    }

    [DebuggerDisplay("({Name},{Value})")]
    public class AuthKey
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public string Title { get; set; }

        public AuthKey()
        {

        }

        public AuthKey(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}