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
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using ASC.Api.Web.Help.DocumentGenerator;

namespace ASC.Api.Web.Help.Helpers
{
    public class BreadCrumbsBuilder
    {
        private readonly Controller _context;

        public class BreadCrumb
         {
             public string Text { get; set; }
             public string Link { get; set; }

             public override bool Equals(object obj)
             {
                 if (ReferenceEquals(null, obj)) return false;
                 if (ReferenceEquals(this, obj)) return true;
                 if (obj.GetType() != typeof (BreadCrumb)) return false;
                 return Equals((BreadCrumb) obj);
             }

            public bool Equals(BreadCrumb other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(other.Text, Text) && Equals(other.Link, Link);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Text != null ? Text.GetHashCode() : 0)*397) ^ (Link != null ? Link.GetHashCode() : 0);
                }
            }
         }

         public List<BreadCrumb> BreadCrumbs { get; set; } 
         
         public BreadCrumbsBuilder(Controller context)
         {
             _context = context;
         }

        public void Add(string text, string routeName, string action, string contorller, object routeValues)
        {
            if (_context.ViewData["breadcrumbs"]==null)
            {
                _context.ViewData["breadcrumbs"] = BreadCrumbs = new List<BreadCrumb>();
            }
            var breadCrumb = new BreadCrumb()
                                 {
                                     Link =
                                         UrlHelper.GenerateUrl(routeName, action, contorller,
                                                               new RouteValueDictionary(routeValues), RouteTable.Routes,
                                                               _context.ControllerContext.RequestContext, false),
                                     Text = text
                                 };
            if (!BreadCrumbs.Contains(breadCrumb))
            {
                BreadCrumbs.Add(breadCrumb);
            }
        }

        public void Add(MsDocEntryPoint section)
        {
            Add(section, null);
        }

        public void Add(MsDocEntryPoint section, MsDocEntryPointMethod method)
        {
            if (section != null)
            {
                Add(section.Name, "Docs", "Index", "Documentation", Url.GetRouteValues(section.Name, null, null));
            }
            if (method != null && section!=null)
            {
                Add(string.IsNullOrEmpty(method.ShortName) ? (string.IsNullOrEmpty(method.Summary) ? method.FunctionName : method.Summary) : method.ShortName, "Docs", "Index", "Documentation", Url.GetRouteValues(section.Name, method.HttpMethod, method.Path));
            }
        }
    }
}