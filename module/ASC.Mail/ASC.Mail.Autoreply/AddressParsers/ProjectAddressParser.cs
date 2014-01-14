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
    internal class ProjectAddressParser : AddressParser
    {
        protected override Regex GetRouteRegex()
        {
            return new Regex(@"^(?'type'task|message)_(?'projectId'\d+)$", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
        }

        protected override ApiRequest ParseRequestInfo(IDictionary<string, string> groups, Tenant t)
        {
            var type = groups["type"];
            if (type == "task")
            {
                return new ApiRequest(string.Format("project/{0}/task", groups["projectId"]))
                {
                    Parameters = new List<RequestParameter>
                        {
                            new RequestParameter("description", new PlainTextContentResolver()),
                            new RequestParameter("deadline", new TaskDeadlineResolver()),
                            new RequestParameter("priority", new TaskPriorityResolver()),
                            new RequestParameter("milestoneid", new TaskMilestoneResolver()),
                            new RequestParameter("responsibles", new TaskResponsiblesResolver()),
                            new RequestParameter("responsible", new TaskResponsibleResolver()),
                            new RequestParameter("title", new TitleResolver(TaskDeadlineResolver.Pattern, TaskPriorityResolver.Pattern, TaskMilestoneResolver.Pattern, TaskResponsiblesResolver.Pattern))
                        }
                };
            }

            return new ApiRequest(string.Format("project/{0}/message", groups["projectId"]))
                {
                    Parameters = new List<RequestParameter>
                        {
                            new RequestParameter("title", new TitleResolver()),
                            new RequestParameter("content", new HtmlContentResolver())
                        }
                };
        }
    }
}