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
using ASC.Core;
using ASC.Core.Users;
using ASC.Common.Security.Authentication;
using ASC.Common.Security.Authorizing;

namespace ASC.Api.Calendar.Wrappers
{
    [DataContract(Name ="publicItem")]
    public class PublicItemWrapper : ASC.Web.Core.Calendars.SharingOptions.PublicItem
    {
        private Guid _owner;
        private string _calendarId;
        private string _eventId;
        private bool _isCalendar;


        public PublicItemWrapper( ASC.Web.Core.Calendars.SharingOptions.PublicItem publicItem, string calendartId, Guid owner)
        {
            base.Id = publicItem.Id;
            base.IsGroup = publicItem.IsGroup;

            _owner = owner;
            _calendarId = calendartId;
            _isCalendar = true;
        }

        public PublicItemWrapper(ASC.Web.Core.Calendars.SharingOptions.PublicItem publicItem, string calendarId, string eventId, Guid owner)
        {
            base.Id = publicItem.Id;
            base.IsGroup = publicItem.IsGroup;

            _owner = owner;
            _calendarId = calendarId;
            _eventId = eventId;
            _isCalendar = false;
        }

        [DataMember(Name = "id", Order = 10)]
        public string ItemId
        {
            get
            {
                return base.Id.ToString();
            }
            set{}
        }

        [DataMember(Name = "name", Order = 20)]
        public string ItemName
        {
            get
            {
                if(this.IsGroup)                
                    return CoreContext.GroupManager.GetGroupInfo(base.Id).Name;                
                else
                    return CoreContext.UserManager.GetUsers(base.Id).DisplayUserName();                
            }
            set{}
        }

        [DataMember(Name = "isGroup", Order = 30)]
        public new bool IsGroup
        {
            get
            {
                return base.IsGroup;
            }
            set { }
        }

        [DataMember(Name = "canEdit", Order = 40)]
        public bool CanEdit
        {
            get
            {
                return !base.Id.Equals(_owner); 
            }
            set { }
        }

        [DataMember(Name = "selectedAction", Order = 50)]
        public AccessOption SharingOption
        {
            get {
                if (base.Id.Equals(_owner))
                {
                    return AccessOption.OwnerOption;
                }
                var subject = IsGroup ? (ISubject)CoreContext.GroupManager.GetGroupInfo(base.Id) : (ISubject)CoreContext.Authentication.GetAccountByID(base.Id);
                int calId;
                if (_isCalendar && int.TryParse(_calendarId,out calId))
                {
                    var obj = new ASC.Api.Calendar.BusinessObjects.Calendar() { Id = _calendarId };
                    if (SecurityContext.PermissionResolver.Check(subject, obj, null, CalendarAccessRights.FullAccessAction))
                        return AccessOption.FullAccessOption;
                }
                else if(!_isCalendar)
                {
                    var obj = new ASC.Api.Calendar.BusinessObjects.Event() { Id = _eventId, CalendarId = _calendarId};
                    if (SecurityContext.PermissionResolver.Check(subject, obj, null, CalendarAccessRights.FullAccessAction))
                        return AccessOption.FullAccessOption;
                }

                return AccessOption.ReadOption;
            }
            set { }
        }

        public static object GetSample()
        {
            return new { selectedAction = AccessOption.GetSample(), canEdit = true, isGroup = true, 
                         name = "Everyone", id = "2fdfe577-3c26-4736-9df9-b5a683bb8520" };
        }
    }
}
