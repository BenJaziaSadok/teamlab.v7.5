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

namespace ASC.Core.Common.Notify.Push
{
    [DataContract]
    public enum PushAction
    {
        [EnumMember]
        Unknown,

        [EnumMember]
        Created,

        [EnumMember]
        Assigned,

        [EnumMember]
        InvitedTo,

        [EnumMember]
        Closed,

        [EnumMember]
        Resumed,

        [EnumMember]
        Deleted
    }

    [DataContract]
    public enum PushItemType
    {
        [EnumMember]
        Unknown,

        [EnumMember]
        Task,

        [EnumMember]
        Subtask,

        [EnumMember]
        Milestone,

        [EnumMember]
        Project,

        [EnumMember]
        Message
    }

    [DataContract]
    public enum PushModule
    {
        [EnumMember]
        Unknown,

        [EnumMember]
        Projects
    }

    [DataContract]
    public enum DeviceType
    {
        [EnumMember]
        Ios,
        
        [EnumMember]
        Android
    }
}
