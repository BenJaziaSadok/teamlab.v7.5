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

#region usings

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

#endregion

namespace ASC.Api.Collections
{
    public static class Extensions
    {
        public static ItemList<T> ToItemList<T>(this IEnumerable<T> enumerable)
        {
            return new ItemList<T>(enumerable);
        }

        public static SmartList<T> ToSmartList<T>(this IEnumerable<T> enumerable)
        {
            return SmartListFactory.Create(enumerable);
        }

        public static ItemDictionary<TKey, TValue> ToItemDictionary<TKey, TValue>(
            this IDictionary<TKey, TValue> enumerable)
        {
            return new ItemDictionary<TKey, TValue>(enumerable);
        }

        public static NameValueCollection ToNameValueCollection(
            this IEnumerable<KeyValuePair<string, object>> enumerable)
        {
            var collection = new NameValueCollection();
            foreach (var keyValuePair in enumerable.Where(keyValuePair => keyValuePair.Value is string))
            {
                collection.Add(keyValuePair.Key,(string)keyValuePair.Value);
            }
            return collection;
        }
    }
}