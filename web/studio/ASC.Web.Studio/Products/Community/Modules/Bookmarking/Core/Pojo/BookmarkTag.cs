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
	public class BookmarkTag
	{
		public virtual long BookmarkTagID { get; set; }

		public virtual long BookmarkID { get; set; }

		public virtual long TagID { get; set; }

		// override object.Equals
		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			
			var bookmarkTag = obj as BookmarkTag;
			if (this.TagID.Equals(bookmarkTag.TagID))
			{
				return true;
			}
			return false;
		}

		// override object.GetHashCode
		public override int GetHashCode()
		{			
			var hash = TagID.GetHashCode();
			return hash;
		}

	}
}
