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
using ASC.Web.Core.Calendars;
using ASC.Api.Calendar.BusinessObjects;
using System.Collections.Generic;

namespace ASC.Api.Calendar.Wrappers
{
    [DataContract(Name = "subscription", Namespace = "")]
    public class SubscriptionWrapper : CalendarWrapper
    {
        public SubscriptionWrapper(BaseCalendar calendar)
            : base(calendar) { }
        public SubscriptionWrapper(BaseCalendar calendar, UserViewSettings userViewSettings)
            : base(calendar, userViewSettings) { }


        [DataMember(Name = "isSubscribed", Order = 100)]
        public bool IsAccepted
        {
            get
            {
                if(UserCalendar is Calendar.BusinessObjects.Calendar)
                    return _userViewSettings != null && _userViewSettings.IsAccepted;

                return this.IsAcceptedSubscription;
            }
            set { }
        }


        [DataMember(Name = "isNew", Order = 140)]
        public bool IsNew
        {
            get
            {
                return _userViewSettings==null;
            }
            set { }
        }

        [DataMember(Name = "group", Order = 130)]
        public string Group 
        {
            get {

                if(UserCalendar.IsiCalStream())
                    return Resources.CalendarApiResource.iCalCalendarsGroup;

                return String.IsNullOrEmpty(UserCalendar.Context.Group) ? Resources.CalendarApiResource.SharedCalendarsGroup : UserCalendar.Context.Group;
            }
            set { }
        }

        [DataMember(IsRequired=false)]
        public override CalendarPermissions Permissions{get; set;}

        public new static object GetSample()
        {
            return new
            {
                canEditTimeZone = false,
                timeZone = TimeZoneWrapper.GetSample(),
                defaultAlert = EventAlertWrapper.GetSample(),
                events = new List<object>() { EventWrapper.GetSample() },
                owner = UserParams.GetSample(),
                objectId = "1",
                title = "Calendar Name",
                description = "Calendar Description",
                backgroundColor = "#000000",
                textColor = "#ffffff",
                isEditable = true,
                permissions = CalendarPermissions.GetSample(),
                isShared = true,
                canAlertModify = true,
                isHidden = false,
                isiCalStream = false,
                isSubscription = false,
                group = "Personal Calendars",
                isNew = true,
                isSubscribed = false,

            };
        }
    }
}
