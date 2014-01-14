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
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Projects.Core.DataInterfaces;
using ASC.Projects.Core.Domain;

namespace ASC.Projects.Engine
{
    public class CommentEngine
    {
        private readonly ICommentDao _commentDao;


        public CommentEngine(IDaoFactory daoFactory)
        {
            _commentDao = daoFactory.GetCommentDao();
        }

        public List<Comment> GetComments(DomainObject<Int32> targetObject)
        {
            return targetObject != null ? _commentDao.GetAll(targetObject) : new List<Comment>();
        }

        public Comment GetByID(Guid id)
        {
            return _commentDao.GetById(id);
        }

        public Comment GetLast(DomainObject<Int32> targetObject)
        {
            return targetObject != null ? _commentDao.GetLast(targetObject) : null;
        }

        public int Count(DomainObject<Int32> targetObject)
        {
            return targetObject == null ? 0 : _commentDao.Count(targetObject);
        }

        public List<int> GetCommentsCount(List<ProjectEntity> targets)
        {
            return _commentDao.GetCommentsCount(targets);
        }

        public Comment SaveOrUpdate(Comment comment)
        {
            if (comment == null) throw new ArgumentNullException("comment");

            ProjectSecurity.DemandCreateComment();

            if (comment.CreateBy == default(Guid)) comment.CreateBy = SecurityContext.CurrentAccount.ID;

            var now = TenantUtil.DateTimeNow();
            if (comment.CreateOn == default(DateTime)) comment.CreateOn = now;

            var newComment = _commentDao.Save(comment);
            //mark entity as jast readed

            return newComment;
        }
    }
}