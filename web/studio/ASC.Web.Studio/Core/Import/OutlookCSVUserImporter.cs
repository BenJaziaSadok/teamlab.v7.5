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

using System.Collections.Generic;
using System.IO;

namespace ASC.Web.Studio.Core.Import
{
    public class OutlookCSVUserImporter : TextFileUserImporter
    {
        public OutlookCSVUserImporter(Stream fileStream)
            : base(fileStream)
        {
            HasHeader = true;
            Separators = new[] { ';', ',' };
            NameMapping = new Dictionary<string, string>()
                              {
                                  {"First Name", "FirstName"},
                                  {"Last Name", "LastName"},
                                  {"Middle Name", ""},
                                  {"Name", ""},
                                  {"Nickname", ""},
                                  {"E-mail Address", "Email"},
								  {"Email", "Email"},
                                  {"Home Street", "PrimaryAddress"},
                                  {"Home City", ""},
                                  {"Home Postal Code", "PostalCode"},
                                  {"Home State", ""},
                                  {"Home Country/Region", ""},
                                  {"Home Phone", "PhoneHome"},
                                  {"Home Fax", ""},
                                  {"Mobile Phone", "PhoneMobile"},
                                  {"Personal Web Page", ""},
                                  {"Business Street", ""},
                                  {"Business City", ""},
                                  {"Business Postal Code", ""},
                                  {"Business State", ""},
                                  {"Business Country/Region", ""},
                                  {"Business Web Page", ""},
                                  {"Business Phone", "PhoneOffice"},
                                  {"Business Fax", ""},
                                  {"Pager", ""},
                                  {"Job Title", "Title"},
                                  {"Department", "Department"},
                                  {"Office Location", ""},
                                  {"Notes", "Notes"}
                              };
        }
    }
}