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

namespace ASC.FederatedLogin.Helpers
{
    public static class DictionaryExtensions
    {
         public static TValue Get<T,TValue>(this IDictionary<T, TValue> disct, T key)
         {
             TValue def;
             disct.TryGetValue(key, out def);
             return def;
         }
    }
}