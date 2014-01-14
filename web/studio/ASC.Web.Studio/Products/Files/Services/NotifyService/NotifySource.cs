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
using ASC.Notify.Model;
using ASC.Notify.Patterns;
using NotifySourceBase = ASC.Core.Notify.NotifySource;

namespace ASC.Web.Files.Services.NotifyService
{
    public class NotifySource : NotifySourceBase
    {
        private static NotifySource instance = new NotifySource();

        public static NotifySource Instance
        {
            get { return instance; }
        }

        public NotifySource()
            : base(new Guid("6FE286A4-479E-4c25-A8D9-0156E332B0C0"))
        {
        }

        protected override IActionProvider CreateActionProvider()
        {
            return new ConstActionProvider(
                NotifyConstants.Event_ShareFolder,
                NotifyConstants.Event_ShareDocument,
                NotifyConstants.Event_LinkToEmail);
        }

        protected override IPatternProvider CreatePatternsProvider()
        {
            return new XmlPatternProvider2(FilesPatternResource.patterns);
        }
    }
}