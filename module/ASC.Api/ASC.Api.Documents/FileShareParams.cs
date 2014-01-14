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
using ASC.Files.Core.Security;
using ASC.Web.Files.Services.WCFService;

namespace ASC.Api.Documents
{
    /// <summary>
    /// </summary>
    public class FileShareParams
    {
        /// <summary>
        /// </summary>
        public Guid ShareTo { get; set; }
        
        /// <summary>
        /// </summary>
        public FileShare Access { get; set; }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public AceWrapper ToAceObject()
        {
            return new AceWrapper
                          {
                              Share = Access,
                              SubjectId = ShareTo,
                              SubjectGroup = !Core.CoreContext.UserManager.UserExists(ShareTo)
                          };
        }
    }
}