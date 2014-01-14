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
using System.Linq;
using ASC.Api.Attributes;
using ASC.Api.Collections;
using ASC.Api.Projects.Wrappers;

#endregion

namespace ASC.Api.Projects
{
    public partial class ProjectApi
    {
        #region tags

		 ///<summary>
		 ///Returns the list of all available project tags
		 ///</summary>
		 ///<short>
		 ///Project tags
		 ///</short>
		 /// <category>Tags</category>
        ///<returns>List of tags</returns>
        [Read(@"tag")]
        public IEnumerable<ObjectWrapperBase> GetAllTags()
        {
            return EngineFactory.GetTagEngine().GetTags().Select(x => new ObjectWrapperBase{Id = x.Key, Title = x.Value}).ToSmartList();

        }

		  ///<summary>
		  ///Returns the detailed list of all projects with the specified tag
		  ///</summary>
		  ///<short>
		  ///Project by tag
		  ///</short>
		  /// <category>Tags</category>
        ///<param name="tag">Tag name</param>
        ///<returns>List of projects</returns>
        [Read(@"tag/{tag}")]
        public IEnumerable<ProjectWrapper> GetProjectsByTags(string tag)
        {
            var projectsTagged = EngineFactory.GetTagEngine().GetTagProjects(tag);
            return EngineFactory.GetProjectEngine().GetByID(projectsTagged).Select(x => new ProjectWrapper(x)).ToSmartList();
        }


          ///<summary>
          ///Returns the list of all tags like the specified tag name
          ///</summary>
          ///<short>
          ///Tags by tag name
          ///</short>
          /// <category>Tags</category>
        ///<param name="tagName">Tag name</param>
        ///<returns>List of tags</returns>
        [Read(@"tag/search")]
        public string[] GetTagsByName(string tagName)
        {
            return !string.IsNullOrEmpty(tagName) && tagName.Trim() != string.Empty
                ? EngineFactory.GetTagEngine().GetTags(tagName.Trim()).Select(r => r.Value).ToArray()
                : new string[0];
        }

        #endregion
    }
}
