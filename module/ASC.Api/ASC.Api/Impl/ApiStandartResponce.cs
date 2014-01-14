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

using System.Runtime.Serialization;
using ASC.Api.Enums;
using ASC.Api.Interfaces;

#endregion

namespace ASC.Api.Impl
{
    [DataContract(Name = "result", Namespace = "")]
    internal class ApiStandartResponce : IApiStandartResponce
    {
        #region IApiStandartResponce Members

        [DataMember(Name = "response", EmitDefaultValue = false, Order = 200)]
        public object Response { get; set; }

        [DataMember(Name = "error", EmitDefaultValue = false, Order = 210)]
        public ErrorWrapper Error { get; set; }

        [DataMember(Name = "status", EmitDefaultValue = true, Order = 100)]
        public ApiStatus Status { get; set; }

        [DataMember(Name = "statusCode", EmitDefaultValue = false, Order = 101)]
        public long Code { get; set; }

        [DataMember(Name = "count", EmitDefaultValue = false, Order = 10)]
        public long Count { get; set; }

        [DataMember(Name = "startIndex", EmitDefaultValue = false, Order = 11)]
        public long StartIndex { get; set; }

        [DataMember(Name = "nextIndex", EmitDefaultValue = false, Order = 12)]
        public long? NextPage { get; set; }

        [DataMember(Name = "total", EmitDefaultValue = false, Order = 13)]
        public long? TotalCount { get; set; }


        public ApiContext ApiContext { get; set; }
        #endregion
    }
}