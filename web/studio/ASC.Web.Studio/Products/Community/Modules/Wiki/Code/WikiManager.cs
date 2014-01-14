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
using ASC.Web.UserControls.Wiki.Resources;

namespace ASC.Web.UserControls.Wiki
{
    public sealed class WikiManager
    {
        WikiManager()
        {

        }

        public static Guid ModuleId
        {
            get { return new Guid("742CF945-CBBC-4a57-82D6-1600A12CF8CA"); }
        }

        public static int MaxPageSearchResult = 10;
        public static int MaxPageSearchInLinkDlgResult = 3;

        public static string BaseVirtualPath
        {
            get
            {
                return "~/Products/Community/Modules/Wiki".ToLower();
            }
        }

        public static string ViewVirtualPath
        {
            get
            {
                return "~/Products/Community/Modules/Wiki/Default.aspx".ToLower();
            }
        }

        public static string SearchDefaultString
        {
            get
            {
                return WikiResource.SearchDefaultString;
            }
        }

        public static string WikiSectionConfig
        {
            get
            {
                return "/Products/Community/Modules/Wiki";
            }
        }

    }
}
