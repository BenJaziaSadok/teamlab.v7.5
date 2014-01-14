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
using System.Text;
using AjaxPro;
using ASC.Forum;
using ASC.Web.Studio;
using ASC.Web.Studio.Utility;
using ASC.Web.UserControls.Forum;
using ASC.Web.UserControls.Forum.Common;
using ASC.Web.Community.Resources;

namespace ASC.Web.Community.Forum
{
    public partial class Posts : MainPage
    {
        protected string ForumPageTitle { get; set; }
        protected string ForumPageParentTitle { get; set; }
        protected string ForumPageParentURL { get; set; }
        protected string ForumPageParentIn { get; set; }

        private UserControls.Forum.Common.ForumManager _forumManager;
        public Topic Topic { get; set; }
        public Guid SettingsID { get; set; }
        protected string SubscribeStatus { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            _forumManager = UserControls.Forum.Common.ForumManager.GetSettings(ForumManager.Settings.ID).ForumManager;

            ForumManager.Instance.SetCurrentPage(ForumPage.PostList);

            var idTopic = 0;
            if (!String.IsNullOrEmpty(Request["t"]))
            {
                try
                {
                    idTopic = Convert.ToInt32(Request["t"]);
                }
                catch
                {
                    idTopic = 0;
                }
            }
            if (idTopic == 0)
                Response.Redirect("default.aspx");


            var topic = ForumDataProvider.GetTopicByID(TenantProvider.CurrentTenantID, idTopic);
            if (topic == null)
                Response.Redirect("default.aspx");

            Topic = topic;

            var postListControl = LoadControl(ForumManager.Settings.UserControlsVirtualPath + "/PostListControl.ascx") as PostListControl;
            postListControl.SettingsID = ForumManager.Settings.ID;
            postListControl.Topic = topic;
            postListHolder.Controls.Add(postListControl);
            Utility.RegisterTypeForAjax(typeof (TopicControl), Page);
            Utility.RegisterTypeForAjax(typeof (Subscriber));
            var subscriber = new Subscriber();

            var isTopicSubscribe = subscriber.IsTopicSubscribe(topic.ID);
            var SubscribeTopicLink = subscriber.RenderTopicSubscription(!isTopicSubscribe, topic.ID);

            //master.ActionsPlaceHolder.Controls.Add(new HtmlMenuItem(subscriber.RenderThreadSubscription(!isThreadSubscribe, topic.ThreadID)));
            //master.ActionsPlaceHolder.Controls.Add(new HtmlMenuItem(subscriber.RenderTopicSubscription(!isTopicSubscribe, topic.ID)));

            ForumPageParentTitle = topic.ThreadTitle;
            ForumPageParentIn = CommunityResource.InForParentPage;
            ForumPageParentURL = "topics.aspx?f=" + topic.ThreadID.ToString();
            ForumPageTitle = topic.Title;
            Title = HeaderStringHelper.GetPageTitle((Master as ForumMasterPage).CurrentPageCaption ?? Resources.ForumResource.AddonName);
            SubscribeStatus = isTopicSubscribe ? "subscribed" : "unsubscribed";

            RenderModeratorFunctionsHeader();

            SubscribeLinkBlock.Text = SubscribeTopicLink;
        }

        protected void RenderModeratorFunctionsHeader()
        {
            var sb = new StringBuilder();
            var bitMask = 0;

            if (!Topic.IsApproved && _forumManager.ValidateAccessSecurityAction(ForumAction.ApprovePost, Topic))
                bitMask += 1;

            if (_forumManager.ValidateAccessSecurityAction(ForumAction.TopicDelete, Topic))
                bitMask += 2;

            if (_forumManager.ValidateAccessSecurityAction(ForumAction.TopicSticky, Topic))
                bitMask += 4;

            if (_forumManager.ValidateAccessSecurityAction(ForumAction.TopicClose, Topic))
                bitMask += 8;

            if (_forumManager.ValidateAccessSecurityAction(ForumAction.TopicEdit, Topic))
                bitMask += 32;

            var _settings = UserControls.Forum.Common.ForumManager.GetSettings(ForumManager.Settings.ID).LinkProvider;

            if (bitMask > 0)
            {
                sb.Append("<span class=\"menu-small\" onclick=\"javascript:ForumManager.ShowModeratorFunctions('" + Topic.ID + "'," + bitMask + "," + (int) Topic.Status + ",'" + _settings.EditTopic(Topic.ID) + "');\"></span>");
            }

            menuToggle.Text = sb.ToString();
        }
    }
}