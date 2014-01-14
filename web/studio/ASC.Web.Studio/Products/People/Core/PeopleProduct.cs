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

#region Import

using System;
using ASC.Web.Core;
using ASC.Web.Core.Utility;

#endregion

namespace ASC.Web.People.Core
{
    public class PeopleProduct : Product
    {
        internal const string ProductPath = "~/products/people/";

        private ProductContext _context;

        public static Guid ID
        {
            get { return new Guid("{F4D98AFD-D336-4332-8778-3C6945C81EA0}"); }
        }

        public override ProductContext Context
        {
            get { return this._context; }
        }

        public override string Name
        {
            get { return Resources.PeopleResource.ProductName; }
        }

        public override string Description
        {
            get { return Resources.PeopleResource.ProductDescription; }
        }

        public override string ExtendedDescription
        {
            get { return Resources.PeopleResource.ProductDescription; }
        }

        public override Guid ProductID
        {
            get { return ID; }
        }

        public override string StartURL
        {
            get { return ProductPath; }
        }
        public override string ProductClassName
        {
            get { return "people"; }
        }

        public override void Init()
        {
            _context = new ProductContext
            {
                MasterPageFile = "~/products/people/PeopleBaseTemplate.Master",
                DisabledIconFileName = "product_disabled_logo.png",
                IconFileName = "product_logo.png",
                LargeIconFileName = "product_logolarge.png",
                DefaultSortOrder = 50,
            };

            SearchHandlerManager.Registry(new SearchHandler());
        }
    }
}
