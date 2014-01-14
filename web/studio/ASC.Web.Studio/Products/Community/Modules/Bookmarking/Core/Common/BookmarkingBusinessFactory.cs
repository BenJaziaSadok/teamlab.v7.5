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
using System.Web;

namespace ASC.Bookmarking.Common
{

	public static class BookmarkingBusinessFactory
	{
		public static T GetObjectFromSession<T>() where T : class, new()
		{
		    T obj;
			var key = typeof(T).ToString();
            if (HttpContext.Current.Session != null)
            {
                obj = (T) HttpContext.Current.Session[key];
                if (obj == null)
                {
                    obj = Activator.CreateInstance<T>();
                    HttpContext.Current.Session[key] = obj;
                }
            }
            else
            {
                obj = (T)HttpContext.Current.Items[key];
                if (obj == null)
                {
                    obj = Activator.CreateInstance<T>();
                    HttpContext.Current.Items[key] = obj;
                }
            }
		    return obj;
		}
	}
}
