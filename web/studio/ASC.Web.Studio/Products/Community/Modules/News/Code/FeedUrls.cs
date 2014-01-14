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

namespace ASC.Web.Community.News.Code
{
    internal static class FeedUrls
    {
        public static string BaseVirtualPath
        {
            get { return "~/products/community/modules/news/"; }
        }

        public static string MainPageUrl
        {
            get { return VirtualPathUtility.ToAbsolute(BaseVirtualPath); }
        }

        public static string EditNewsName
        {
            get { return "editnews.aspx"; }
        }

        public static string EditPollName
        {
            get { return "editpoll.aspx"; }
        }

        public static string EditNewsUrl
        {
            get { return VirtualPathUtility.ToAbsolute(string.Format("{0}{1}", BaseVirtualPath, EditNewsName)); }
        }

        public static string EditOrderUrl
        {
            get { return VirtualPathUtility.ToAbsolute(string.Format("{0}{1}", BaseVirtualPath, EditNewsName)) + "?type=" + FeedType.Order.ToString(); }
        }

        public static string EditAdvertUrl
        {
            get { return VirtualPathUtility.ToAbsolute(string.Format("{0}{1}", BaseVirtualPath, EditNewsName)) + "?type=" + FeedType.Advert.ToString(); }
        }

        public static string EditPollUrl
        {
            get { return VirtualPathUtility.ToAbsolute(string.Format("{0}{1}", BaseVirtualPath, EditPollName)); }
        }

        public static string GetFeedAbsolutePath(long feedId)
        {
            return VirtualPathUtility.ToAbsolute(BaseVirtualPath) + "?docid=" + feedId.ToString();
        }

        public static string GetFeedVirtualPath(long feedId)
        {
            return string.Format("{0}?docid={1}", BaseVirtualPath, feedId);
        }

        public static string GetFeedUrl(long feedId)
        {
            return GetFeedUrl(feedId, Guid.Empty);
        }

        public static string GetFeedUrl(long feedId, Guid userId)
        {
            var url = string.Format("{0}?docid={1}", MainPageUrl, feedId);
            if (userId != Guid.Empty) url += "&uid=" + userId.ToString();
            return url;
        }

        public static string GetFeedListUrl(FeedType feedType)
        {
            return GetFeedListUrl(feedType, Guid.Empty);
        }

        public static string GetFeedListUrl(Guid userId)
        {
            return GetFeedListUrl(FeedType.All, userId);
        }

        public static string GetFeedListUrl(FeedType feedType, Guid userId)
        {
            var url = MainPageUrl;
            string p1 = null;
            string p2 = null;

            if (feedType != FeedType.All) p1 = "type=" + feedType.ToString();
            if (userId != Guid.Empty) p2 = "uid=" + userId.ToString();
            if (p1 == null && p2 == null) return url;

            url += "?";
            if (p1 != null) url += p1 + "&";
            if (p2 != null) url += p2 + "&";
            return url.Substring(0, url.Length - 1);
        }
    }
}