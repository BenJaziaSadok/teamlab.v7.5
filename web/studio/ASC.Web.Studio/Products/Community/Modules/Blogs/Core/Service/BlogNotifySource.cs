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
using ASC.Notify.Model;
using ASC.Notify.Patterns;


namespace ASC.Blogs.Core.Service
{
    public class BlogNotifySource : NotifySource
    {
        public BlogNotifySource()
            : base(Constants.ModuleId)
        {
        }

        protected override IActionProvider CreateActionProvider()
        {
            return new ConstActionProvider(
                    Constants.NewPost,
                    Constants.NewPostByAuthor,
                    Constants.NewComment
                );
        }
        protected override IPatternProvider CreatePatternsProvider()
        {
            return new XmlPatternProvider2(BlogPatternsResource.patterns);
        }
    }
}
