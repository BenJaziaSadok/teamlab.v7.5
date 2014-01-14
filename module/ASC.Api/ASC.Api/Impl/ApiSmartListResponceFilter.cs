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

using System;
using System.Collections;
using ASC.Api.Collections;
using ASC.Api.Interfaces;

#endregion

namespace ASC.Api.Impl
{
    public class ApiSmartListResponceFilter : IApiResponceFilter
    {

        #region IApiResponceFilter Members

        public object FilterResponce(object responce, ApiContext context)
        {
            if (responce != null && !context.FromCache)
            {
                ISmartList smartList = null;
                var type = responce.GetType();
                if (responce is ISmartList)
                {
                    smartList = responce as ISmartList;
                }
                else if (Utils.Binder.IsCollection(type) && !typeof(IDictionary).IsAssignableFrom(type))
                {
                    try
                    {
                        var elementType = Utils.Binder.GetCollectionType(type);
                        var smartListType = SmartListFactory.GetSmartListType().MakeGenericType(elementType);
                        smartList = Activator.CreateInstance(smartListType, (IEnumerable)responce) as ISmartList;
                    }
                    catch (Exception)
                    {
                        
                    }
                }
                if (smartList != null)
                {
                    return TransformList(context, smartList);
                }
            }
            return responce;
        }

        private static object TransformList(ApiContext context, ISmartList smartList)
        {
            bool getTotalCount = context.SpecifiedCount < smartList.Count && !context.TotalCount.HasValue;/*We have already more items than needed and no one set totalcount*/

            smartList.TakeCount = context.SpecifiedCount;
            smartList.StartIndex = context.StartIndex;
            smartList.IsDescending = context.SortDescending;
            smartList.SortBy = context.SortBy;
            smartList.FilterBy = context.FilterBy;
            smartList.FilterOp = context.FilterOp;
            smartList.FilterValue = context.FilterValues;
            smartList.UpdatedSince = context.UpdatedSince;
            smartList.FilterType = context.FilterToType;
            var list= smartList.Transform(getTotalCount);
            if (getTotalCount)
            {
                context.TotalCount = smartList.TotalCount;
            }
            return list;
        }

        #endregion
    }
}