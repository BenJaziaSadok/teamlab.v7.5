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

namespace ASC.Web.Studio.Core.Import
{
    public class ContactInfo
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

		public override bool Equals(object obj)
		{
			try
			{
				if (obj is ContactInfo)
				{
					var o = obj as ContactInfo;
					return Email.Equals(o.Email);
				}
			}
			catch { }
			return false;
		}

		public override int GetHashCode()
		{
			return Email.GetHashCode();
		}
    }
}