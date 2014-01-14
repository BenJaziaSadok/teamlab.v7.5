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
using ASC.Common.Security;
using ASC.Common.Security.Authorizing;
using ASC.Core.Users;
using System.Collections.Generic;
using ASC.Blogs.Core.Domain;

namespace ASC.Blogs.Core.Security
{
    public class PersonalBlogSecObject : SecurityObjectId, ISecurityObject
    {
        private UserInfo blogOwner;

        public PersonalBlogSecObject()
            : base((int)BlogType.Personal, typeof(BlogType))
        {

        }

        public PersonalBlogSecObject(UserInfo blogOwner)
            : this()
        {

            this.blogOwner = blogOwner;
        }

        public override string ToString()
        {
            return "personal blog";
        }

        #region ISecurityObjectProvider Members

        public bool InheritSupported
        {
            get { return false; }
        }

        public bool ObjectRolesSupported
        {
            get { return blogOwner != null; }
        }

        public ISecurityObjectId InheritFrom(ISecurityObjectId objectId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IRole> GetObjectRoles(ISubject account, ISecurityObjectId objectId, SecurityCallContext callContext)
        {
            var roles = new List<IRole>();
            if (blogOwner != null && blogOwner.ID.Equals(account.ID))
            {
                roles.Add(ASC.Common.Security.Authorizing.Constants.Owner);
            }
            return roles;
        }

        #endregion
    }
}
