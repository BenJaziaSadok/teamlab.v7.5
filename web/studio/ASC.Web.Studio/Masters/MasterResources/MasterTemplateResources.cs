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

using System.Collections.Generic;
using System.Threading;
using System.Web;
using ASC.Web.Core.Client.HttpHandlers;

namespace ASC.Web.Studio.Masters.MasterResources
{
    public class MasterTemplateResources : ClientScriptLocalization
    {
        protected override string BaseNamespace
        {
            get { return "ASC.Resources.Master"; }
        }

        protected override IEnumerable<KeyValuePair<string, object>> GetClientVariables(HttpContext context)
        {
            yield return RegisterClientTemplatesPath("~/templates/UserProfileCardTemplate.ascx", context);
            yield return RegisterClientTemplatesPath("~/templates/AdvansedFilterTemplate.ascx", context);
            yield return RegisterClientTemplatesPath("~/templates/FeedListTemplate.ascx", context);
            yield return RegisterClientTemplatesPath("~/templates/DropFeedTemplate.ascx", context);
            yield return RegisterClientTemplatesPath("~/templates/AdvUserSelectorTemplate.ascx", context);
            yield return RegisterClientTemplatesPath("~/templates/GroupSelectorTemplate.ascx", context);
            yield return RegisterClientTemplatesPath("~/templates/SharingSettingsTemplate.ascx", context);
        }
    }
}
