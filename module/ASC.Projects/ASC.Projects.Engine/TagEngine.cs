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

using System.Collections.Generic;
using System.Linq;
using ASC.Projects.Core.DataInterfaces;

namespace ASC.Projects.Engine
{
    public class TagEngine
    {
        private readonly ITagDao _tagDao;


        public TagEngine(IDaoFactory daoFactory)
        {
            _tagDao = daoFactory.GetTagDao();
        }


        public Dictionary<int, string> GetTags()
        {
            return _tagDao.GetTags();
        }

        public Dictionary<int, string> GetTags(string prefix)
        {
            return _tagDao.GetTags(prefix);
        }

        public string GetById(int id)
        {
            return _tagDao.GetById(id);
        }

        public int[] GetTagProjects(string tagName)
        {
            return _tagDao.GetTagProjects(tagName);
        }

        public int[] GetTagProjects(int tagID)
        {
            return _tagDao.GetTagProjects(tagID);
        }

        public Dictionary<int, string> GetProjectTags(int projectId)
        {
            return _tagDao.GetProjectTags(projectId);
        }

        public void SetProjectTags(int projectId, string tags)
        {
            _tagDao.SetProjectTags(projectId, FromString(tags));
        }

        private string[] FromString(string tags)
        {
            return (tags ?? string.Empty)
                .Split(',', ';')
                .Select(t => t.Trim())
                .Where(t => !string.IsNullOrEmpty(t))
                .ToArray();
        }
    }
}
