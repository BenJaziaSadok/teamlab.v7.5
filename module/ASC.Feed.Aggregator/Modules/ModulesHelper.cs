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
using ASC.Web.Core;

namespace ASC.Feed.Aggregator.Modules
{
    public static class ModulesHelper
    {
        public static Guid CommunityProductID
        {
            get { return WebItemManager.CommunityProductID; }
        }

        public static string CommunityProductName
        {
            get { return "community"; }
        }

        public static Guid CRMProductID
        {
            get { return WebItemManager.CRMProductID; }
        }

        public static string CRMProductName
        {
            get { return "crm"; }
        }

        public static Guid ProjectsProductID
        {
            get { return WebItemManager.ProjectsProductID; }
        }

        public static string ProjectsProductName
        {
            get { return "projects"; }
        }

        public static Guid DocumentsProductID
        {
            get { return WebItemManager.DocumentsProductID; }
        }

        public static string DocumentsProductName
        {
            get { return "documents"; }
        }
    }
}