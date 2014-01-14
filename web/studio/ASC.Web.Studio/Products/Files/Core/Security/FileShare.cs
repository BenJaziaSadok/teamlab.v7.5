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

namespace ASC.Files.Core.Security
{
    [DataContract(Name = "fileShare", Namespace = "")]
    public enum FileShare
    {
        [EnumMember(Value = "0")]
        None,

        [EnumMember(Value = "1")]
        ReadWrite,

        [EnumMember(Value = "2")]
        Read,

        [EnumMember(Value = "3")]
        Restrict,
    }
}