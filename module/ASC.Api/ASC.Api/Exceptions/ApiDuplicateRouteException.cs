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
using System.Runtime.Serialization;
using ASC.Api.Interfaces;

namespace ASC.Api.Exceptions
{
    [Serializable]
    public class ApiDuplicateRouteException : Exception
    {
        public ApiDuplicateRouteException(IApiMethodCall currentMethod, IApiMethodCall registeredMethod)
            : base(string.Format("route '{0}' is already registered to '{1}'", currentMethod, registeredMethod))
        {
        }

        public ApiDuplicateRouteException()
        {
        }


        public ApiDuplicateRouteException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected ApiDuplicateRouteException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}