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

using ASC.Api.Employee;
using ASC.Files.Core.Security;
using ASC.Web.Files.Services.WCFService;

namespace ASC.Api.Documents
{
    /// <summary>
    /// </summary>
    public class FileShareWrapper
    {
        private FileShareWrapper()
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="aceWrapper"></param>
        public FileShareWrapper(AceWrapper aceWrapper)
        {
            IsOwner = aceWrapper.Owner;
            IsLocked = aceWrapper.LockedRights;
            if (aceWrapper.SubjectGroup)
            {
                //Shared to group
                SharedTo = new GroupWrapperSummary(Core.CoreContext.GroupManager.GetGroupInfo(aceWrapper.SubjectId));
            }
            else
            {
                SharedTo = EmployeeWraper.Get(aceWrapper.SubjectId);    
            }
            Access = aceWrapper.Share;

        }

        /// <summary>
        /// </summary>
        public FileShare Access { get; set; }

        /// <summary>
        /// </summary>
        public object SharedTo { get; set; }

        /// <summary>
        /// </summary>
        public bool IsLocked { get; set; }

        /// <summary>
        /// </summary>
        public bool IsOwner { get; set; }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static FileShareWrapper GetSample()
        {
            return new FileShareWrapper()
                       {
                           Access = FileShare.ReadWrite,
                           IsLocked = false,
                           IsOwner = true,
                           SharedTo = EmployeeWraper.GetSample()
                       };
        }
    }
}