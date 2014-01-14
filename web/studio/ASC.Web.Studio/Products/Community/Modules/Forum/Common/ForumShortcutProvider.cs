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

using System.Web;
using ASC.Forum;

namespace ASC.Web.Community.Forum
{
    public static class ForumShortcutProvider
    {
        public static string GetCreateContentPageUrl()
        {
            return ValidateCreateTopicOrPoll(false) ? VirtualPathUtility.ToAbsolute(ForumManager.BaseVirtualPath + "/newpost.aspx") + "?m=0" : null;
        }

        private static bool ValidateCreateTopicOrPoll(bool isPool)
        {
            return ForumManager.Instance.ValidateAccessSecurityAction((isPool ? ForumAction.PollCreate : ForumAction.TopicCreate), null);
        }
    }
}
