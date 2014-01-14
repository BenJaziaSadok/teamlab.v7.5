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

#endregion

namespace ASC.Projects.Core.DataInterfaces
{
    public interface ITagDao
    {
        Dictionary<int, string> GetTags();

        Dictionary<int, string> GetTags(string prefix);

        string GetById(int id);

        int[] GetTagProjects(string tagName);

        int[] GetTagProjects(int tagID);

        Dictionary<int, string> GetProjectTags(int projectId);

        void SetProjectTags(int projectId, string[] tags);
    }
}
