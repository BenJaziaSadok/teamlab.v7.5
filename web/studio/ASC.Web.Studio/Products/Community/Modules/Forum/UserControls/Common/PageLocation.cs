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

namespace ASC.Web.UserControls.Forum.Common
{   
    public enum ForumPage
    {
        Default,
        TopicList,
        PostList,
        NewPost,
        EditTopic,
        Search,
        UserProfile,
        ManagementCenter
    }

    public class PageLocation : ICloneable
    {
        public ForumPage Page{get; set;}
        public string Url { get; set;}
        
        public PageLocation(ForumPage page, string url)
        {
            this.Page = page;
            this.Url = url;
        }

        #region ICloneable Members

        public object Clone()
        {
            return new PageLocation(this.Page, this.Url);
        }

        #endregion
    }
}
