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

using System.Linq;
using System.Runtime.Serialization;
using ASC.Api.Collections;

namespace ASC.Api.Batch
{
    [DataContract(Name = "batch_response", Namespace = "")]
    public class ApiBatchResponse
    {
        public ApiBatchResponse(ApiBatchRequest apiBatchRequest)
        {
            Name = apiBatchRequest.Name;
        }

        [DataMember(Order = 100)]
        public string Data { get; set; }

        [DataMember(Order = 10,EmitDefaultValue = false)]
        public ItemDictionary<string, string> Headers { get; set; }

        [DataMember(Order = 5)]
        public int Status { get; set; }

        [DataMember(Order = 1,EmitDefaultValue = false)]
        public string Name { get; set; }
    }
}