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
	public class ReviewPost
    {
        private Guid _ReviewID;
        private Post _Post;
        private Guid _UserID;
        private DateTime _Timestamp;
        private int _Count;

        public virtual Guid ReviewID
        {
            get { return _ReviewID; }
            set { _ReviewID = value; }
        }
        public virtual Post Post
        {
            get { return _Post; }
            set { _Post = value; }
        }
        public virtual Guid UserID
        {
            get { return _UserID; }
            set { _UserID = value; }
        }
        public virtual DateTime Timestamp
        {
            get { return _Timestamp; }
            set { _Timestamp = value; }
        }
        public virtual int Count
        {
            get { return _Count; }
            set { _Count = value; }
        }
    }
}
