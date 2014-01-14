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
using ASC.Api.Exceptions;

namespace ASC.Api.Utils
{
    public static class Validate
    {
        public static T If<T>(this T item,Func<T,bool> @if, Func<T> then) where T : class
        {
            return @if(item) ? then() : item;
        }

        public static T IfNull<T>(this T item, Func<T> func) where T:class
        {
            return item.If((x)=>x==default(T),func);
        }

        public static T ThrowIfNull<T>(this T item, Exception e) where T : class
        {
            return item.IfNull(() => { throw e; });
        }

        public static T NotFoundIfNull<T>(this T item) where T : class
        {
            return NotFoundIfNull<T>(item, "Item not found");
        }

        public static T NotFoundIfNull<T>(this T item, string message) where T : class
        {
            return item.IfNull(() => { throw new ItemNotFoundException(message); });
        }
    }
}