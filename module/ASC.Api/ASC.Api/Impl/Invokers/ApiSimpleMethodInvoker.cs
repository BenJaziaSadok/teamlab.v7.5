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
using ASC.Api.Interfaces;

namespace ASC.Api.Impl.Invokers
{
    internal class ApiSimpleMethodInvoker : IApiMethodInvoker
    {
        #region IApiMethodInvoker Members

        public virtual object InvokeMethod(IApiMethodCall methodToCall, object instance, IEnumerable<object> callArg, ApiContext apicontext)
        {
            return methodToCall.Invoke(instance, callArg.ToArray());
        }

        #endregion
    }
}