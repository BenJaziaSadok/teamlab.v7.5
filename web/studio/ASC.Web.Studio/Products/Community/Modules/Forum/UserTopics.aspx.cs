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
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Controls.Common;
using AjaxPro;
using ASC.Core;
using ASC.Core.Users;
using ASC.Forum;
using ASC.Web.Studio;
using ASC.Web.Studio.Utility;
using ASC.Web.UserControls.Forum;
using ASC.Web.UserControls.Forum.Common;

namespace ASC.Web.Community.Forum
{
    [AjaxNamespace("SearchResult")]
    public partial class UserTopics : MainPage
    {
        protected bool _isFind;
        private Guid _userID;

        protected void Page_Load(object sender, EventArgs e)
        {
            ForumManager.Instance.SetCurrentPage(ForumPage.Search);

            int currentPageNumber;
            if (!int.TryParse(Request["p"], out currentPageNumber))
                currentPageNumber = 1;

            if (currentPageNumber <= 0)
                currentPageNumber = 1;

            var findTopicList = new List<Topic>();
            var topicCount = 0;

            if (!String.IsNullOrEmpty(Request["uid"]))
            {
                try
                {
                    _userID = new Guid(Request["uid"]);
                }
                catch
                {
                    _userID = Guid.Empty;
                }

                if (_userID != Guid.Empty)
                {
                    findTopicList = ForumDataProvider.SearchTopicsByUser(TenantProvider.CurrentTenantID, _userID, currentPageNumber, ForumManager.Settings.TopicCountOnPage, out topicCount);
                }
            }

            if (findTopicList.Count > 0)
            {
                _isFind = true;

                var i = 0;
                foreach (var topic in findTopicList)
                {
                    var topicControl = (TopicControl)LoadControl(ForumManager.Settings.UserControlsVirtualPath + "/TopicControl.ascx");
                    topicControl.SettingsID = ForumManager.Settings.ID;
                    topicControl.Topic = topic;
                    topicControl.IsShowThreadName = true;
                    topicControl.IsEven = (i%2 == 0);
                    topicListHolder.Controls.Add(topicControl);
                    i++;
                }

                #region navigators

                var pageNavigator = new PageNavigator
                    {
                        CurrentPageNumber = currentPageNumber,
                        EntryCountOnPage = ForumManager.Settings.TopicCountOnPage,
                        VisiblePageCount = 5,
                        EntryCount = topicCount,
                        PageUrl = "usertopics.aspx?uid=" + _userID.ToString()
                    };

                bottomPageNavigatorHolder.Controls.Add(pageNavigator);

                #endregion
            }
            else
            {
                var emptyScreenControl = new EmptyScreenControl
                    {
                        ImgSrc = WebImageSupplier.GetAbsoluteWebPath("forums_icon.png", ForumManager.Settings.ModuleID),
                        Header = Resources.ForumResource.EmptyScreenSearchCaption,
                        Describe = Resources.ForumResource.EmptyScreenSearchText,
                    };

                topicListHolder.Controls.Add(emptyScreenControl);
            }

            //bread crumbs
            (Master as ForumMasterPage).CurrentPageCaption = CoreContext.UserManager.GetUsers(_userID).DisplayUserName(false);

            Title = HeaderStringHelper.GetPageTitle((Master as ForumMasterPage).CurrentPageCaption);
        }
    }
}