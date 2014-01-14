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

using ASC.Common.Data;
using System;

namespace ASC.Blogs.Core.Data
{
    public class BlogsStorage : IDisposable
    {
        readonly DbManager _db;
        readonly int _tenant;
        readonly IPostDao _postDao;


        public BlogsStorage(string dbId, int tenant)
        {
            _db = new DbManager(dbId);
            _tenant = tenant;
            _postDao = new DbPostDao(_db, _tenant);
        }


        public IPostDao GetPostDao()
        {
            return _postDao;
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
