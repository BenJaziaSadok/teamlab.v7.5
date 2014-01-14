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

#region Usings

using System;
using System.Web;

#endregion

namespace ASC.Web.Projects.Classes
{
    public static class UrlParameters
    {
        public static string ProjectsFilter
        {
            get { return HttpContext.Current.Request[UrlConstant.ProjectsFilter] ?? string.Empty; }
        }

        public static string ProjectsTag
        {
            get { return HttpContext.Current.Request[UrlConstant.ProjectsTag] ?? string.Empty; }
        }

        public static string ActionType
        {
            get { return HttpContext.Current.Request[UrlConstant.Action] ?? string.Empty; }
        }

        public static string Search
        {
            get { return HttpContext.Current.Request[UrlConstant.Search] ?? string.Empty; }
        }

        public static string EntityID
        {
            get { return HttpContext.Current.Request[UrlConstant.EntityID] ?? string.Empty; }
        }

        public static string ProjectID
        {
            get { return HttpContext.Current.Request[UrlConstant.ProjectID] ?? string.Empty; }
        }

        public static int PageNumber
        {
            get
            {
                int result;
                return int.TryParse(HttpContext.Current.Request[UrlConstant.PageNumber], out result) ? result : 1;
            }
        }

        public static Guid UserID
        {
            get
            {
                var result = HttpContext.Current.Request[UrlConstant.UserID];
                if (!string.IsNullOrEmpty(result))
                {
                    try
                    {
                        return new Guid(result);
                    }
                    catch (OverflowException) { }
                    catch (FormatException) { }
                }
                return Guid.Empty;
            }
        }

        public static String ReportType
        {
            get { return HttpContext.Current.Request[UrlConstant.ReportType] ?? string.Empty; }
        }
    }
}
