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

using ASC.Files.Core.Security;
using ASC.Projects.Data;
using ASC.Web.Projects.Classes;
using ASC.Web.Studio.Utility;

namespace ASC.Projects.Engine
{
    public class SecurityAdapterProvider : IFileSecurityProvider
    {
        public IFileSecurity GetFileSecurity(string data)
        {
            int id;
            return int.TryParse(data, out id) ? GetFileSecurity(id) : null;
        }

        public static IFileSecurity GetFileSecurity(int projectId)
        {
            return new SecurityAdapter(new DaoFactory("projects", TenantProvider.CurrentTenantID), projectId);
        }
    }
}
