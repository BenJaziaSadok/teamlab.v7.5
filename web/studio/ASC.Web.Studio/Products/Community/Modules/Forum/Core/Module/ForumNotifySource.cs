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

using ASC.Core.Notify;
using ASC.Notify;
using ASC.Notify.Model;
using ASC.Notify.Patterns;

namespace ASC.Forum.Module
{
    class ForumNotifyClient
    {
        public static INotifyClient NotifyClient { get; private set; }

        static ForumNotifyClient()
        {
            NotifyClient = ASC.Core.WorkContext.NotifyContext.NotifyService.RegisterClient(ForumNotifySource.Instance);
        }
    }

    class ForumNotifySource : NotifySource
    {
        public static ForumNotifySource Instance
        {
            get;
            private set;
        }


        static ForumNotifySource()
        {
            Instance = new ForumNotifySource();
        }


        private ForumNotifySource()
            : base(ASC.Web.Community.Forum.ForumManager.ModuleID)
        {

        }


        protected override IActionProvider CreateActionProvider()
        {
            return new ConstActionProvider(
                    Constants.NewPostByTag,
                    Constants.NewPostInThread,
                    Constants.NewPostInTopic,
                    Constants.NewTopicInForum
                );
        }

        protected override IPatternProvider CreatePatternsProvider()
        {
            return new XmlPatternProvider2(ASC.Forum.Module.Patterns.forum_patterns);
        }
    }
}
