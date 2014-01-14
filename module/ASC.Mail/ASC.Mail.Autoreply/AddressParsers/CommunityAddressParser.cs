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
using System.Text.RegularExpressions;
using ASC.Core.Tenants;
using ASC.Mail.Autoreply.ParameterResolvers;

namespace ASC.Mail.Autoreply.AddressParsers
{
    internal class CommunityAddressParser : AddressParser
    {
        protected override Regex GetRouteRegex()
        {
            return new Regex(@"^(?'type'blog|event)$", RegexOptions.Compiled);
        }

        protected override ApiRequest ParseRequestInfo(IDictionary<string,string> groups, Tenant t)
        {
            var callInfo = new ApiRequest("community/" + groups["type"])
                {
                    Parameters = new List<RequestParameter>
                        {
                            new RequestParameter("content", new HtmlContentResolver()),
                            new RequestParameter("title", new TitleResolver(BlogTagsResolver.Pattern)),
                            new RequestParameter("subscribeComments", true),
                            new RequestParameter("type", 1),
                            new RequestParameter("tags", new BlogTagsResolver())
                        }
                };

            return callInfo;
        }
    }
}