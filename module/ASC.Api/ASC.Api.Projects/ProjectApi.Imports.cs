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
using System.Linq;
using System.Security;
using System.Text.RegularExpressions;
using ASC.Api.Attributes;
using ASC.Api.Collections;
using ASC.Api.Projects.Wrappers;
using ASC.Core;
using ASC.Web.Projects.Configuration;
using ASC.Web.Projects.Resources;
using SecurityContext = ASC.Core.SecurityContext;

#endregion

namespace ASC.Api.Projects
{
    public partial class ProjectApi
    {
		///<summary>
		///Adds the company URL for importing to the queue
		///</summary>
		///<short>
		///Add importing URL to queue
		///</short>
		/// <category>Import</category>        
        ///<param name="url">The company URL </param>
        ///<param name="userName">User Name </param>
        ///<param name="password">Password </param>
        ///<param name="importClosed">Import closed</param>
        ///<param name="disableNotifications">Disable notifications</param>
        ///<param name="importUsersAsCollaborators">Flag for add users as guests</param>
        ///<param name="projects" optional="true">Projects for importing</param>
        ///<returns>Import status</returns>
        [Create(@"import")]
        public ImportStatus Add(string url, string userName, string password, bool importClosed, bool disableNotifications, bool importUsersAsCollaborators, IEnumerable<int> projects)
        {
            if(!CoreContext.UserManager.IsUserInGroup(SecurityContext.CurrentAccount.ID, Core.Users.Constants.GroupAdmin.ID))
                throw new SecurityException();

            //Validate all data
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException(ImportResource.EmptyURL);

            if (string.IsNullOrEmpty(userName))
                throw new ArgumentException(ImportResource.EmptyEmail);

            if (string.IsNullOrEmpty(password))
                throw new ArgumentException(ImportResource.EmptyPassword);

            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
                throw new ArgumentException(ImportResource.MalformedUrl);

            ImportQueue.Add(url, userName, password, importClosed, disableNotifications, importUsersAsCollaborators, projects);

            return GetStatus();
        }

		///<summary>
		///Returns the list of the projects to be imported
		///</summary>
		///<short>
		///Get projects for import
		///</short>
		/// <category>Import</category>
        ///<returns>List of projects</returns>
        ///<param name="url">The company URL </param>
        ///<param name="userName">User Name </param>
        ///<param name="password">Password </param>
        [Create(@"import/projects")]
        public IEnumerable<ObjectWrapperBase> GetProjectsForImport(string url, string userName, string password)
        {
            return ImportQueue.GetProjects(url, userName, password).Select(x => new ObjectWrapperBase{Id = x.ID, Title = x.Title, Status = (int)x.Status}).ToSmartList();
        }

        ///<summary>
        ///Returns the number of users that can be added to the import. This number can be negative, if the number of imported users exceeds the quota.
        ///</summary>
        ///<short>
        ///Returns the number of users that can be added to the import
        ///</short>
        /// <category>Import</category>
        ///<returns>Number of users</returns>
        ///<param name="url">The company URL </param>
        ///<param name="userName">User Name </param>
        ///<param name="password">Password </param>
        ///<visible>false</visible>
        [Create(@"import/quota")]
        public int CheckUsersQuota(string url, string userName, string password)
        {
            return ImportQueue.CheckUsersQuota(url, userName, password);
        }

		///<summary>
		///Returns the project importing status
		///</summary>
		///<short>
		///Get import status
		///</short>
		/// <category>Import</category>
        ///<returns>Importing Status</returns>
        [Read(@"import")]
        public ImportStatus GetStatus()
        {
            if (!CoreContext.UserManager.IsUserInGroup(SecurityContext.CurrentAccount.ID, Core.Users.Constants.GroupAdmin.ID))
                throw new SecurityException();

            return ImportQueue.GetStatus();
        }
    }
}
