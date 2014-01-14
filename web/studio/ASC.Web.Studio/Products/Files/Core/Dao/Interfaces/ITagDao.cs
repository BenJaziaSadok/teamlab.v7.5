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
using System.Collections.Generic;

namespace ASC.Files.Core
{
    public interface ITagDao : IDisposable
    {
        IEnumerable<Tag> GetTags(Guid owner, TagType tagType);

        IEnumerable<Tag> GetTags(string name, TagType tagType);

        IEnumerable<Tag> GetTags(String[] names, TagType tagType);

        IEnumerable<Tag> GetNewTags(Guid subject, Folder parentFolder, bool deepSearch);

        IEnumerable<Tag> GetNewTags(Guid subject,  params FileEntry[] fileEntries);

        IEnumerable<Tag> SaveTags(params Tag[] tag);

        void UpdateNewTags(params Tag[] tag);

        void RemoveTags(params Tag[] tag);

        void RemoveTags(params int[] tagIds);

        IEnumerable<Tag> GetTags(object entryID, FileEntryType entryType, TagType tagType);
    }
}