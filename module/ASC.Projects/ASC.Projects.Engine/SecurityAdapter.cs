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
using ASC.Core.Caching;
using ASC.Files.Core;
using ASC.Files.Core.Security;
using ASC.Projects.Core.Domain;
using ASC.Projects.Engine;

namespace ASC.Web.Projects.Classes
{
    public class SecurityAdapter : IFileSecurity
    {
        private readonly ASC.Projects.Core.DataInterfaces.IProjectDao dao;
        private readonly int projectId;

        private Project project;
        private readonly TrustInterval interval = new TrustInterval();
        private readonly TimeSpan timeout = TimeSpan.FromSeconds(10);

        private Project Project
        {
            get
            {
                if (interval.Expired)
                {
                    project = dao.GetById(projectId);
                    interval.Start(timeout);
                }
                return project;
            }
        }

        public SecurityAdapter(ASC.Projects.Core.DataInterfaces.IDaoFactory factory, int projectId)
        {
            dao = factory.GetProjectDao();
            this.projectId = projectId;
        }

        public bool CanRead(FileEntry file, Guid userId)
        {
            return Can(file, userId, SecurityAction.Read);
        }

        public bool CanCreate(FileEntry file, Guid userId)
        {
            return Can(file, userId, SecurityAction.Create);
        }

        public bool CanDelete(FileEntry file, Guid userId)
        {
            return Can(file, userId, SecurityAction.Delete);
        }

        public bool CanEdit(FileEntry file, Guid userId)
        {
            return Can(file, userId, SecurityAction.Edit);
        }

        private bool Can(FileEntry fileEntry, Guid userId, SecurityAction action)
        {
            if (!ProjectSecurity.CanReadFiles(Project)) return false;

            if (IsAdmin(userId)) return true;
            if (fileEntry == null || Project == null) return false;
            if (fileEntry is Folder && ((Folder) fileEntry).FolderType == FolderType.DEFAULT && fileEntry.CreateBy == userId) return true;
            if (fileEntry is File && fileEntry.CreateBy == userId) return true;

            switch (action)
            {
                case SecurityAction.Read:
                    return !Project.Private || dao.IsInTeam(Project.ID, userId);
                case SecurityAction.Create:
                case SecurityAction.Edit:
                    return dao.IsInTeam(Project.ID, userId)
                           && (!ProjectSecurity.IsVisitor(userId) || fileEntry is Folder && ((Folder) fileEntry).FolderType == FolderType.BUNCH);
                case SecurityAction.Delete:
                    return !ProjectSecurity.IsVisitor(userId) && Project.Responsible == userId;
                default:
                    return false;
            }
        }

        private static bool IsAdmin(Guid userId)
        {
            return ProjectSecurity.IsAdministrator(userId);
        }

        private enum SecurityAction
        {
            Read,
            Create,
            Edit,
            Delete,
        };
    }
}