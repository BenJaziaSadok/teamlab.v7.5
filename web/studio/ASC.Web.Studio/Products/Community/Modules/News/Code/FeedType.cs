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
	[Flags]
	public enum FeedType
	{
		News = 1,
		Order = 2,
		Advert = 4,
		Poll = 8,

		None = 0,
		AllNews = News | Advert | Order,
		All = AllNews | Poll,
	}
}