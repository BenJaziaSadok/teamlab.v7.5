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
using System.Linq;
using ASC.Api.Interfaces;
using ASC.Api.Interfaces.Cache;

#endregion

namespace ASC.Api.Impl
{
    public class ApiCacheKeyBuilder : IApiCacheMethodKeyBuilder
    {
        #region IApiCacheMethodKeyBuilder Members

        public virtual string BuildCacheKeyForMethodCall(IApiMethodCall apiMethodCall, IEnumerable<object> callArgs, ApiContext context)
        {
            return string.Format("{0}.{1}({2}),{3}:{4}",
                                 apiMethodCall.MethodCall.DeclaringType.FullName,
                                 apiMethodCall.MethodCall.Name,
                                 string.Join(",", callArgs.Select(x => x.GetHashCode().ToString()).ToArray()),
                                 apiMethodCall.MethodCall.DeclaringType.Assembly.FullName,context);
        }

        #endregion
    }
}