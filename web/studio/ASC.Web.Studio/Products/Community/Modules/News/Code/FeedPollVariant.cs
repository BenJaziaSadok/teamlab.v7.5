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

namespace ASC.Web.Community.News.Code
{
	[Serializable]
	public class FeedPollVariant
	{
		public long ID
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

	    public override string ToString()
		{
			return Name;
		}

		public override int GetHashCode()
		{
			return ID.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			var v = obj as FeedPollVariant;
			return obj != null && ID == v.ID;
		}
	}
}
