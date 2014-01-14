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

namespace ASC.Files.Core.Security
{
    public class FileShareRecord
    {
        public int Tenant { get; set; }

        public object EntryId { get; set; }

        public FileEntryType EntryType { get; set; }

        public Guid Subject { get; set; }

        public Guid Owner { get; set; }

        public FileShare Share { get; set; }

        public int Level { get; set; }
    }

    public class SmallShareRecord
    {
        public Guid ShareTo { get; set; }
        public Guid ShareParentTo { get; set; }
        public Guid ShareBy { get; set; }
        public DateTime ShareOn { get; set; }
        public FileShare Share { get; set; }
    }
}
