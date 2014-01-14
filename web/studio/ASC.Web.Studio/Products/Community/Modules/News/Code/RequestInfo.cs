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
using System.Globalization;
using System.Web;
using ASC.Core;
using ASC.Core.Users;

namespace ASC.Web.Community.News.Code
{
	public class RequestInfo
	{
		public Guid UserId = Guid.Empty;

		public string UserIdAttribute = string.Empty;

		public UserInfo User = null;

		public bool HasUser = false;

		public RequestInfo(HttpRequest request)
		{
			try
			{
				if (!string.IsNullOrEmpty(request["uid"]))
				{
					UserId = new Guid(request["uid"]);
					if (UserId != Guid.Empty)
					{
						HasUser = true;
						UserIdAttribute = string.Format(CultureInfo.CurrentCulture, "&uid={0}", UserId);
						User = CoreContext.UserManager.GetUsers(UserId);
					}
				}
			}
			catch { }
		}
	}
}
