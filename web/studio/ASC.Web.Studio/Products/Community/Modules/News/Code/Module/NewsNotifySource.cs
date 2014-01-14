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
using ASC.Core;
using ASC.Core.Notify;
using ASC.Notify;
using ASC.Notify.Model;
using ASC.Notify.Patterns;
using ASC.Web.Community.News.Resources;

namespace ASC.Web.Community.News.Code.Module
{
    public enum FeedSubscriptionType
    {
        NewFeed = 0,
        NewOrder = 1
    }

    public class NewsNotifyClient
    {
        public static INotifyClient NotifyClient { get; private set; }

        static NewsNotifyClient()
        {
            NotifyClient = WorkContext.NotifyContext.NotifyService.RegisterClient(NewsNotifySource.Instance);
        }
    }

    public class NewsNotifySource : NotifySource
    {
        public static NewsNotifySource Instance
        {
            get;
            private set;
        }

        private NewsNotifySource()
            : base(new Guid("{6504977C-75AF-4691-9099-084D3DDEEA04}"))
        {

        }

        static NewsNotifySource()
        {
            Instance = new NewsNotifySource();
        }

        protected override IActionProvider CreateActionProvider()
        {
            return new ConstActionProvider(NewsConst.NewFeed, NewsConst.NewComment);
        }

        protected override IPatternProvider CreatePatternsProvider()
        {
            return new XmlPatternProvider2(NewsPatternsResource.news_patterns);
        }
    }
}