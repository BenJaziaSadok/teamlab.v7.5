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

namespace ASC.CRM.Core.Entities
{
    [DataContract]
    public class ListItem : DomainObject
    {
        [DataMember(Name = "title")]
        public String Title { get; set; }

        [DataMember(Name = "description")]
        public String Description { get; set; }

        [DataMember(Name = "color")]
        public String Color { get; set; }

        [DataMember(Name = "sort_order")]
        public int SortOrder { get; set; }

        [DataMember(Name = "additional_params")]
        public String AdditionalParams { get; set; }


        public ListItem()
        {
        }

        public ListItem(string title, string addparams)
        {
            Title = title;
            AdditionalParams = addparams;
        }
    }
}
