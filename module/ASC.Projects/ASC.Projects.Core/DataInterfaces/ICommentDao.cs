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

#region Usings

using System;
using System.Collections.Generic;
using ASC.Projects.Core.Domain;

#endregion

namespace ASC.Projects.Core.DataInterfaces
{
    public interface ICommentDao
    {
        List<Comment> GetAll(DomainObject<int> target);

        Comment GetById(Guid id);

        Comment GetLast(DomainObject<int> target);

        int Count(DomainObject<int> target);

        List<int> GetCommentsCount(List<ProjectEntity> targets);

        Comment Save(Comment comment);

        void Delete(Guid id);
    }
}
