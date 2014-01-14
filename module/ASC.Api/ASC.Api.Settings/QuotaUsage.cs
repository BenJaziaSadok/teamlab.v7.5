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

namespace ASC.Api.Settings
{
    [DataContract(Name = "quota_usage", Namespace = "")]
    public class QuotaUsage
    {
        [DataMember]
        public string Path { get; set; }

        [DataMember]
        public long Size { get; set; }

    }
}