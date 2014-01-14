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

namespace ASC.Web.Core.WebZones
{
    [Flags]
    public enum WebZoneType
    {
        Nowhere = 1,
        StartProductList = 2,
        TopNavigationProductList = 4,
        CustomProductList = 8,

        All = Nowhere | StartProductList | TopNavigationProductList | CustomProductList
    }

    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class WebZoneAttribute : Attribute
    {
        public WebZoneType Type { get; private set; }

        public WebZoneAttribute(WebZoneType type)
        {
            Type = type;
        }
    }

    public interface IRenderWebItem
    {
    }
}