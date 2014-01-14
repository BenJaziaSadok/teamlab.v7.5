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
using ASC.Core.Tenants;

namespace ASC.Web.UserControls.Wiki.Data
{
    public class Comment : IWikiObjectOwner
    {
        public Guid Id
        {
            get;
            set;
        }

        public Guid ParentId
        {
            get;
            set;
        }

        public string PageName
        {
            get;
            set;
        }

        public string Body
        {
            get;
            set;
        }

        public Guid UserId
        {
            get;
            set;
        }

        public DateTime Date
        {
            get;
            set;
        }

        public bool Inactive
        {
            get;
            set;
        }

        public Guid OwnerID
        {
            get { return UserId; }
        }

        public object GetObjectId()
        {
            return Id;
        }
    }
}
