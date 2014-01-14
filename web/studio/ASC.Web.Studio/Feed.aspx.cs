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
using System.Web.UI;
using ASC.Core;
using ASC.Data.Storage;
using ASC.Web.Studio.UserControls.Feed;
using ASC.Web.Studio.Utility;
using Resources;

namespace ASC.Web.Studio
{
    public partial class Feed : MainPage
    {
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            if (CoreContext.Configuration.YourDocs)
            {
                Context.Response.Redirect(CommonLinkUtility.FilesBaseAbsolutePath);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Bootstrap();
            LoadControls();
        }

        private void Bootstrap()
        {
            Title = HeaderStringHelper.GetPageTitle(UserControlsCommonResource.FeedTitle);

            var navigation = (NewNavigationPanel)LoadControl(NewNavigationPanel.Location);
            navigationHolder.Controls.Add(navigation);
        }

        private void LoadControls()
        {
            var feedList = (FeedList)LoadControl(FeedList.Location);
            controlsHolder.Controls.Add(feedList);

            var emptyScreenFilter = new Controls.Common.EmptyScreenControl
                {
                    ImgSrc = WebPath.GetPath("usercontrols/feed/images/empty_filter.png"),
                    Header = UserControlsCommonResource.FilterNoNews,
                    Describe = UserControlsCommonResource.FilterNoNewsDescription,
                    ButtonHTML =
                        string.Format("<a href='javascript:void(0)' class='baseLinkAction clearFilterButton'>{0}</a>",
                                      UserControlsCommonResource.ResetFilter)
                };
            controlsHolder.Controls.Add(emptyScreenFilter);

            var emptyScreenControl = new Controls.Common.EmptyScreenControl
                {
                    ImgSrc = WebPath.GetPath("usercontrols/feed/images/empty_screen_feed.png"),
                    Header = UserControlsCommonResource.NewsNotFound,
                    Describe = UserControlsCommonResource.NewsNotFoundDescription
                };
            controlsHolder.Controls.Add(emptyScreenControl);
        }

        #region IRenderCustomNavigation Members

        public static string RenderCustomNavigation(Page page)
        {
            if (CoreContext.Configuration.YourDocs) return string.Empty;

            return
                string.Format(@"<li class=""top-item-box feed"">
                                  <a href=""{0}"" class=""feedActiveBox inner-text {2}"" title=""{1}"" data-feedUrl=""{0}"">
                                      <span class=""inner-label"">{3}</span>
                                  </a>
                                </li>",
                              VirtualPathUtility.ToAbsolute("~/feed.aspx"),
                              UserControlsCommonResource.FeedTitle,
                              string.Empty,
                              0);
        }

        #endregion
    }
}