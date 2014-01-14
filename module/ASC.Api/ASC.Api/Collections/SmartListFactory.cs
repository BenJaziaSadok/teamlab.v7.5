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
using System.Collections.Generic;

namespace ASC.Api.Collections
{
    public static class SmartListFactory
    {
         public static SmartList<T> Create<T>(IEnumerable<T> elements)
         {
             if (elements!=null)
             {
                 return new CompiledSmartList<T>(elements);
             }
             return null;
         }

        public static Type GetSmartListType()
        {
            return typeof (CompiledSmartList<>);
        }
    }
}