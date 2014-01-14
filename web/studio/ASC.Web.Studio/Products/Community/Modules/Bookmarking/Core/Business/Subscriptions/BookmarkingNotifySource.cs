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

using ASC.Bookmarking.Common;
using ASC.Bookmarking.Resources;
using ASC.Core.Notify;
using ASC.Notify.Model;
using ASC.Notify.Patterns;
using ASC.Web.UserControls.Bookmarking.Common;

namespace ASC.Bookmarking.Business.Subscriptions
{
    internal class BookmarkingNotifySource : NotifySource
    {
        public static BookmarkingNotifySource Instance { get; private set; }

        static BookmarkingNotifySource()
        {
            Instance = new BookmarkingNotifySource();
        }

        private BookmarkingNotifySource() : base(BookmarkingSettings.ModuleId)
        {
        }

        protected override IActionProvider CreateActionProvider()
        {
            return new ConstActionProvider(BookmarkingBusinessConstants.NotifyActionNewBookmark, BookmarkingBusinessConstants.NotifyActionNewComment);
        }

        protected override IPatternProvider CreatePatternsProvider()
        {
            return new XmlPatternProvider2(BookmarkingSubscriptionPatterns.BookmarkingPatterns);
        }
    }
}