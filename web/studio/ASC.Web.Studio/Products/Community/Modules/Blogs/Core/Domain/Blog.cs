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
using System.Text;
using System.Reflection;

namespace ASC.Blogs.Core.Domain
{
	public class Blog
    {
        private long blogID;
        private string name;
        private Guid userID;
        private Guid groupID;
        private IList<Guid> memberList = new List<Guid>();
        private IList<Post> posts = new List<Post>();

        public virtual long BlogID
        {
            get { return blogID; }
            set { blogID = value; }
        }
        public virtual BlogType BlogType
        {
            get { return this.GroupID.Equals(Guid.Empty) ? BlogType.Corporate : BlogType.Personal; }
        }
        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }
        public virtual Guid UserID
        {
            get { return userID; }
            set { userID = value; }
        }
        public virtual Guid GroupID
        {
            get { return groupID; }
            set { groupID = value; }
        }
        public virtual IList<Post> Posts
        {
            get { return posts; }
            set { posts = value; }
        }

        public virtual IList<Guid> MemberList
        {
            get { return memberList; }
            set { memberList = value; }
        }
    }
}
