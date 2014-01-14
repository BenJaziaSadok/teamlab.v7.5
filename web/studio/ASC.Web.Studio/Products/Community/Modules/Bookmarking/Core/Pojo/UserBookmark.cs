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
using System.Collections.Generic;

namespace ASC.Bookmarking.Pojo
{
	public class UserBookmark
	{
		public virtual long UserBookmarkID { get; set; }

		public virtual Guid UserID { get; set; }

		public virtual long BookmarkID { get; set; }

		public virtual int Raiting { get; set; }

		public virtual DateTime DateAdded { get; set; }

		public virtual string Name { get; set; }

		public virtual string Description { get; set; }

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

			var ub = obj as UserBookmark;
			return ub.UserBookmarkID == UserBookmarkID;
		}

		// override object.GetHashCode
		public override int GetHashCode()
		{
			return (GetType().FullName + "|" + UserBookmarkID.ToString()).GetHashCode();
		}

	}
}
