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
using System.Web;
using System.Web.UI.WebControls;
using ASC.Core.Users;
using ASC.Forum;
using ASC.Web.Studio.Utility;
using ASC.Web.UserControls.Forum.Common;

namespace ASC.Web.UserControls.Forum
{   
    public class RecentActivityControl: WebControl
    {   
        public Guid SettingsID { get; set; }
        public int MaxTopicCount { get; set; }
        public bool AutoInitView { get; set; }
        
        private Settings _settings = null;

        public List<Topic> TopicList { get; set; }

        public RecentActivityControl()
        {
            this.MaxTopicCount = 8;
            AutoInitView = true;
            TopicList = new List<Topic>();
        }
        
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (AutoInitView)
                InitView();
            
        }

        public void InitView()
        {
            _settings = ForumManager.GetSettings(SettingsID);
            TopicList = ForumDataProvider.GetLastUpdateTopics(TenantProvider.CurrentTenantID, MaxTopicCount);
        }    

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var topic in TopicList)
            {
                if (topic.RecentPostID != 0)
                {
                    //poster
                    sb.Append("<div class='clearFix' style='margin-bottom:20px;'>");
                    sb.Append(topic.RecentPostAuthor.RenderCustomProfileLink(_settings.ProductID, "describe-text", "link gray"));

                    //topic
                    sb.Append("<div style='margin-top:5px;'>");
                    sb.Append("<a href='" + _settings.LinkProvider.RecentPost(topic.RecentPostID, topic.ID, topic.PostCount) + "'>" + HttpUtility.HtmlEncode(topic.Title) + "</a>");
                    sb.Append("</div>");


                    //date
                    sb.Append("<div style='margin-top:5px;'>");
                    sb.Append(DateTimeService.DateTime2StringPostStyle(topic.RecentPostCreateDate));
                    sb.Append("</div>");

                    sb.Append("</div>");
                }

            }
            
            writer.Write(sb.ToString());
        }
    }
}
