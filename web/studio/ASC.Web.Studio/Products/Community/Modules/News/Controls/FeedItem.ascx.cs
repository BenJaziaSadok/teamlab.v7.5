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
using System.Web.UI;
using AjaxPro;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Community.News.Code;
using ASC.Web.Community.News.Code.DAO;
using ASC.Web.Community.News.Code.Module;
using ASC.Web.Community.News.Resources;
using ASC.Web.Community.Product;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Utility;
using FeedNS = ASC.Web.Community.News.Code;

namespace ASC.Web.Community.News.Controls
{
	[AjaxNamespace("FeedItem")]
	public partial class FeedItem : UserControl
	{
		public FeedNS.Feed Feed { get; set; }
		public bool IsEditVisible { get; set; }
		public string FeedLink { get; set; }
		public Uri RemoveUrlWithParam { get; set; }
		public Uri EditUrlWithParam { get; set; }
		public string FeedType { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public DateTime ExpirationTime { get; set; }
		public int PollVotes { get; set; }

		protected Guid RequestedUserId
		{
			get
			{
				Guid result = Guid.Empty;
				try
				{
					result = new Guid(Request["uid"]);
				}
				catch { }

				return result;
			}
		}

		protected string UserIdAttribute
		{
			get
			{
				if (!RequestedUserId.Equals(Guid.Empty))
				{
					return string.Format(CultureInfo.CurrentCulture, "?uid={0}", RequestedUserId);
				}
				return string.Empty;
			}

		}


		protected void Page_Load(object sender, EventArgs e)
		{
			Utility.RegisterTypeForAjax(typeof(FeedItem), this.Page);
		}

		public override void DataBind()
		{
			base.DataBind();

			if (Feed != null)
			{
				Date.Text = Feed.Date.ToShortDateString();
				NewsLink.NavigateUrl = FeedLink;
				NewsLink.Text = Feed.Caption.HtmlEncode();
                Type.Text = FeedTypeInfo.FromFeedType(Feed.FeedType).TypeName;
                profileLink.Text = CoreContext.UserManager.GetUsers(new Guid(Feed.Creator)).RenderCustomProfileLink(CommunityProduct.ID, "", "linkMedium");
			}
		}

		private string GetEditUrl()
		{
			return (Feed is FeedPoll ? FeedUrls.EditPollUrl : FeedUrls.EditNewsUrl) + UserIdAttribute;
		}

		[AjaxPro.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
		public AjaxResponse Remove(string id)
		{
			AjaxResponse resp = new AjaxResponse();
			resp.rs1 = "0";
			if (!string.IsNullOrEmpty(id))
			{
                CommunitySecurity.DemandPermissions(NewsConst.Action_Edit);

				var storage = FeedStorageFactory.Create();
				storage.RemoveFeed(Convert.ToInt64(id, CultureInfo.CurrentCulture));

                CommonControlsConfigurer.FCKUploadsRemoveForItem("news", id);

				resp.rs1 = id;
				resp.rs2 = NewsResource.FeedDeleted;
			}
			return resp;
		}
	}
}