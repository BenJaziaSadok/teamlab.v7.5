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
using ASC.Web.Core.Utility.Skins;

namespace ASC.Web.Core
{
    public static class ProductModuleExtension
    {
        
        
        public static string GetSmallIconAbsoluteURL(this IModule module)
        {
            if (module == null || module.Context == null || String.IsNullOrEmpty(module.Context.SmallIconFileName))
                return "";

            return WebImageSupplier.GetAbsoluteWebPath(module.Context.SmallIconFileName, module.ID);
        }

        public static string GetSmallIconAbsoluteURL(this IProduct product)
        {
            if (product == null || product.Context == null || String.IsNullOrEmpty(product.Context.SmallIconFileName))
                return "";

            return WebImageSupplier.GetAbsoluteWebPath(product.Context.SmallIconFileName, product.ID);
        }

        public static string GetIconAbsoluteURL(this IModule module)
        {
            if (module == null || module.Context == null || String.IsNullOrEmpty(module.Context.IconFileName))
                return "";

            return WebImageSupplier.GetAbsoluteWebPath(module.Context.IconFileName, module.ID);
        }

        public static string GetIconAbsoluteURL(this IProduct product)
        {
            if (product == null || product.Context == null || String.IsNullOrEmpty(product.Context.IconFileName))
                return "";

            return WebImageSupplier.GetAbsoluteWebPath(product.Context.IconFileName, product.ID);
        }
    }
}
