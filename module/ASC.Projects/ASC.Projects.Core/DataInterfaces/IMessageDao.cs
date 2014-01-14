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

using System.Collections.Generic;
using ASC.Projects.Core.Domain;

#endregion

namespace ASC.Projects.Core.DataInterfaces
{
    public interface IMessageDao
    {
        List<Message> GetAll();

        List<Message> GetByProject(int projectId);

        List<Message> GetMessages(int startIndex, int maxResult);

        List<Message> GetRecentMessages(int offset, int maxResult, params int[] projects);

        List<Message> GetByFilter(TaskFilter filter, bool isAdmin);

        int GetByFilterCount(TaskFilter filter, bool isAdmin);

        Message GetById(int id);

        bool IsExists(int id);

        Message Save(Message message);

        void Delete(int id);
    }
}
