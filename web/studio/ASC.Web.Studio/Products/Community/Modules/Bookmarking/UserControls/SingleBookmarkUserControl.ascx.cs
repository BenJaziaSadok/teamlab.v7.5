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

using System.Collections.Generic;
using AjaxPro;
using ASC.Bookmarking.Pojo;
using ASC.Web.UserControls.Bookmarking.Common.Presentation;

namespace ASC.Web.UserControls.Bookmarking
{
	[AjaxNamespace("SingleBookmarkUserControl")]
	public partial class SingleBookmarkUserControl : BookmarkInfoBase
	{

		/// <summary>
		/// Init tags and added by list
		/// </summary>
		public override void InitUserControl()
		{
			if (Bookmark != null)
			{
				var tags = Bookmark.Tags;
				if (tags != null && tags.Count > 0)
				{
					InitTagsRepeater(tags);
				}
			}			
		}

		private void InitTagsRepeater(IList<Tag> tags)
		{
			if (tags != null && tags.Count > 0)
			{
				TagsRepeater.DataSource = tags;
				TagsRepeater.DataBind();

				UserTagsString = BookmarkingServiceHelper.ConvertTagsToString(tags);
			}
		}

	}
}