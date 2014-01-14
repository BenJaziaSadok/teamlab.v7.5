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
using ASC.Core;
using ASC.Core.Users;

namespace ASC.Web.Studio.Core.Statistic
{
    class UserVisit
    {
        public virtual int TenantID { get; set; }

        public virtual DateTime VisitDate { get; set; }

        public virtual DateTime? FirstVisitTime { get; set; }

        public virtual DateTime? LastVisitTime { get; set; }

        public virtual Guid UserID { get; set; }

        public virtual UserInfo User { get { return CoreContext.UserManager.GetUsers(UserID); } }

        public virtual Guid ProductID { get; set; }

        public virtual int VisitCount { get; set; }
    }
}
