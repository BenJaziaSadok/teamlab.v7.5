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

#region Import

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ASC.Projects.Engine;

#endregion

namespace ASC.Api.Projects.Wrappers
{
    [DataContract(Name = "common_security", Namespace = "")]
    public  class CommonSecurityInfo
    {
        [DataMember]
        public bool CanCreateProject { get; set; }

        public CommonSecurityInfo()
        {
            CanCreateProject = ProjectSecurity.CanCreateProject();
        }
    }
}
