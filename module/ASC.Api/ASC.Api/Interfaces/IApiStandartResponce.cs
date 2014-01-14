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
using System.Runtime.Serialization;
using ASC.Api.Enums;
using ASC.Api.Impl;

#endregion

namespace ASC.Api.Interfaces
{
    public interface IApiStandartResponce
    {
        object Response { get; set; }
        ErrorWrapper Error { get; set; }
        ApiStatus Status { get; set; }
        long Code { get; set; }
        long Count { get; set; }
        long StartIndex { get; set; }
        long? NextPage { get; set; }
        long? TotalCount { get; set; }
        ApiContext ApiContext { get; set; }
    }

    [DataContract(Name = "error", Namespace = "")]
    public class ErrorWrapper
    {
        public ErrorWrapper()
        {
        }

        public ErrorWrapper(Exception exception)
        {
            //Unwrap
            while (exception.InnerException != null)
            {
                exception = exception.InnerException;
            }
            Message = exception.Message;
#if (DEBUG)
            Type = exception.GetType().ToString();
            Stack = exception.StackTrace;
#endif
        }

        [DataMember(Name = "message", EmitDefaultValue = false, Order = 2)]
        public string Message { get; set; }

        [DataMember(Name = "type", EmitDefaultValue = false, Order = 3)]
        public string Type { get; set; }

        [DataMember(Name = "stack", EmitDefaultValue = false, Order = 3)]
        public string Stack { get; set; }
    }
}