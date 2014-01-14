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
using System.Collections.Generic;
using System.Runtime.Serialization;
using ASC.Common.Security;
using ASC.Common.Security.Authorizing;
using ASC.Api.Calendar.Wrappers;
using ASC.Web.Core.Calendars;
using ASC.Core.Tenants;

namespace ASC.Api.Calendar.BusinessObjects
{    
    [AllDayLongUTCAttribute]
    public class Event : BaseEvent, ISecurityObject
    {
        public Event()
        {            
        }

        public int TenantId { get; set; }

        #region ISecurityObjectId Members

        public object SecurityId
        {
            get { return this.Id; }
        }

        /// <inheritdoc/>
        public Type ObjectType
        {
            get { return typeof(Event); }
        }

        #endregion

        #region ISecurityObjectProvider Members

        public IEnumerable<ASC.Common.Security.Authorizing.IRole> GetObjectRoles(ASC.Common.Security.Authorizing.ISubject account, ISecurityObjectId objectId, SecurityCallContext callContext)
        {
            List<IRole> roles = new List<IRole>();
            if (account.ID.Equals(this.OwnerId))
                roles.Add(ASC.Common.Security.Authorizing.Constants.Owner);

            return roles;
        }

        public ISecurityObjectId InheritFrom(ISecurityObjectId objectId)
        {
            int calId;
            if (int.TryParse(this.CalendarId, out calId))
                return new Calendar() { Id = this.CalendarId };

            return null;
        }

        public bool InheritSupported
        {
            get { return true; }
        }

        public bool ObjectRolesSupported
        {
            get { return true; }
        }

        #endregion
       
    }

}
