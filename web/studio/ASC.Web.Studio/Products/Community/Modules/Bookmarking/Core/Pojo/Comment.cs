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

#region Usings

using System;

#endregion

namespace ASC.Bookmarking.Pojo
{
	public class Comment
	{
	    public virtual Guid ID { get; set; }		

		public virtual Guid UserID { get; set; }

		public virtual string Content { get; set; }

		public virtual DateTime Datetime { get; set; }

		public virtual string Parent { get; set; }

		public virtual long BookmarkID { get; set; }

	    public virtual Bookmark Bookmark { get; set; }

        public Comment()
        {
        }

	    public Comment(Bookmark bookmark)
        {
            Bookmark = bookmark;
        }

		public virtual bool Inactive { get; set; }

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

			var c = obj as Comment;
			return c != null && ID.Equals(c.ID);
		}

		public override int GetHashCode()
		{
			return (GetType().FullName + "|" + ID).GetHashCode();
		}						
	}
}
