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

using System.Runtime.Serialization;

namespace ASC.Api.Events
{
    [DataContract(Name = "vote", Namespace = "")]
    public class VoteWrapper
    {
        [DataMember(Order = 1, EmitDefaultValue = true)]
        public long Id { get; set; }

        [DataMember(Order = 10, EmitDefaultValue = true)]
        public string Name { get; set; }

        [DataMember(Order = 20, EmitDefaultValue = true)]
        public int Votes { get; set; }

        public static VoteWrapper GetSample()
        {
            return new VoteWrapper()
                       {
                           Votes = 100,
                           Name = "Variant 1",
                           Id = 133
                       };
        }
    }
}