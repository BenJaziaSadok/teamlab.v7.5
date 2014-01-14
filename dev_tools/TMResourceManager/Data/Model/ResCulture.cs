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

namespace TMResourceData.Model
{
    public class ResCulture
    {
        public string Title { get; set; }
        public string Value { get; set; }
        public bool Available { get; set; }

        public override bool Equals(object obj)
        {
            return Title.Equals(((ResCulture) obj).Title);
        }
        public override int GetHashCode()
        {
            return Title.GetHashCode();
        }
    }
}
