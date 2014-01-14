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


namespace ASC.Bookmarking.Pojo
{
	public class Tag
	{
		public virtual long TagID { get; set; }

		public virtual string Name { get; set; }

		public virtual long Populatiry { get; set; }

		public long BookmarkID { get; set; }

		// override object.Equals
		public override bool Equals(object obj)
		{
			//       
			// See the full list of guidelines at
			//   http://go.microsoft.com/fwlink/?LinkID=85237  
			// and also the guidance for operator== at
			//   http://go.microsoft.com/fwlink/?LinkId=85238
			//

			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}

			var t = obj as Tag;

			if (!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(t.Name) && Name.Equals(t.Name))
			{
				return true;
			}

			return false;
		}

		// override object.GetHashCode
		public override int GetHashCode()
		{
			return (GetType().FullName + "|" + TagID.ToString()).GetHashCode();
		}
	}
}
