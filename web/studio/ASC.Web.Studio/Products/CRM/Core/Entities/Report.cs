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
using ASC.Common.Security;

#endregion

namespace ASC.CRM.Core.Entities
{
    [DataContract]
    public class Report
    {
        [DataMember]
        public String ReportTitle { get; set; }

        [DataMember]
        public String ReportDescription { get; set; }

        [DataMember]
        public IEnumerable<String> Lables { get; set; }

        [DataMember]
        public Object Data { get; set; }
    }
}