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
using ASC.Core.Notify;
using ASC.Notify.Model;
using ASC.Notify.Patterns;

namespace ASC.Web.UserControls.Wiki
{
    public class WikiNotifySource : NotifySource
    {
        private string defPageHref;


        public static WikiNotifySource Instance
        {
            get;
            private set;
        }


        static WikiNotifySource()
        {
            Instance = new WikiNotifySource();
        }

        public WikiNotifySource()
            : base(WikiManager.ModuleId)
        {
            defPageHref = VirtualPathUtility.ToAbsolute(WikiManager.ViewVirtualPath);
        }


        public string GetDefPageHref()
        {
            return defPageHref;
        }


        protected override IActionProvider CreateActionProvider()
        {
            return new ConstActionProvider(Constants.NewPage, Constants.EditPage);
        }

        protected override IPatternProvider CreatePatternsProvider()
        {
            return new XmlPatternProvider2(Patterns.WikiPatternsResource.patterns, ChoosePattern);
        }


        private IPattern ChoosePattern(INotifyAction action, string senderName, ASC.Notify.Engine.NotifyRequest request)
        {
            if (action == Constants.EditPage)
            {
                var tag = request.Arguments.Find(t => t.Tag == "ChangeType");
                if (tag != null && tag.Value.ToString() == "new wiki page comment")
                {
                    return PatternProvider.GetPattern(new NotifyAction(tag.Value.ToString()), senderName);
                }
            }
            return null;
        }
    }
}
