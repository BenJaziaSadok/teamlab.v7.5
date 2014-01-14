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
using System.IO;
using ASC.Data.Storage;
using ASC.Web.Core.WebZones;

namespace ASC.Web.Core
{
    [WebZoneAttribute(WebZoneType.TopNavigationProductList | WebZoneType.StartProductList)]
    public abstract class Product : IProduct
    {
        public abstract Guid ProductID { get; }

        public abstract string Name { get; }

        public abstract string Description { get; }

        public abstract string StartURL { get; }

        public abstract string ProductClassName { get; }

        public abstract void Init();

        public abstract ProductContext Context { get; }

        public virtual void Shutdown() { }

        public virtual string ExtendedDescription { get { return Description; } }

        WebItemContext IWebItem.Context { get { return ((IProduct)this).Context; } }

        Guid IWebItem.ID { get { return ProductID; } }

        public string GetResourcePath(string relativePath)
        {
            return WebPath.GetPath(Path.Combine(StartURL, relativePath));
        }
    }
}
