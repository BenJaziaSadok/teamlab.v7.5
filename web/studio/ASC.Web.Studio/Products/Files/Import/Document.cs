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

namespace ASC.Web.Files.Import
{
    internal class Document
    {
        public string Id { get; set; }

        public string Parent { get; set; }

        public string Title { get; set; }

        public string ContentLink { get; set; }

        public string CreateBy { get; set; }

        public DateTime CreateOn { get; set; }

        public bool IsFolder { get; set; }
    }
}