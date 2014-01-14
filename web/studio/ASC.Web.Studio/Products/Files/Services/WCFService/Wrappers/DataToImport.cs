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
using System.Runtime.Serialization;

namespace ASC.Web.Files.Services.WCFService
{
    [DataContract(Name = "DataToImport", Namespace = "")]
    public class DataToImport
    {
        [DataMember(Name = "title", EmitDefaultValue = false, IsRequired = true)]
        public String Title { get; set; }

        [DataMember(Name = "content_link", EmitDefaultValue = false, IsRequired = true)]
        public String ContentLink { get; set; }

        [DataMember(Name = "create_by", EmitDefaultValue = false, IsRequired = false)]
        public string CreateBy { get; set; }

        [DataMember(Name = "create_on", EmitDefaultValue = false, IsRequired = false)]
        public String CreateOn { get; set; }
    }
}