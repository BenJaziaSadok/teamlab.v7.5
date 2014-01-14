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
using System.Text;
using System.Web.UI.WebControls;

namespace ASC.Bookmarking.Dao
{
	internal class BookmarkingHibernateDaoHelper
	{
		private const string PERSENT_SYMBOL = "%";

		internal static string ModifySearchParam(string searchParam)
		{
			return PERSENT_SYMBOL + searchParam + PERSENT_SYMBOL;
		}

		internal static string ModifySearchParamWithSpace(string searchParam)
		{
			return PERSENT_SYMBOL + " " + searchParam + PERSENT_SYMBOL;
		}

		internal static string ModifyEndOfSearchParam(string searchParam)
		{
			return searchParam + PERSENT_SYMBOL;
		}
	}
}
