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

namespace ASC.Files.Core
{
    [Serializable]
    public class Tag
    {
        public string TagName { get; set; }

        public TagType TagType { get; set; }

        public Guid Owner { get; set; }

        public object EntryId { get; set; }

        public FileEntryType EntryType { get; set; }

        public int Id { get; set; }

        public int Count { get; set; }


        public Tag()
        {
        }

        public Tag(string name, TagType type, Guid owner)
            : this(name, type, owner, null, 0)
        {
        }

        public Tag(string name, TagType type, Guid owner, FileEntry entry, int count)
        {
            TagName = name;
            TagType = type;
            Owner = owner;
            Count = count;
            if (entry != null)
            {
                EntryId = entry.ID;
                EntryType = entry is File ? FileEntryType.File : FileEntryType.Folder;
            }
        }


        public static Tag New(Guid owner, FileEntry entry)
        {
            return New(owner, entry, 1);
        }

        public static Tag New(Guid owner, FileEntry entry, int count)
        {
            return new Tag("new", TagType.New, owner, entry, count);
        }

        public override bool Equals(object obj)
        {
            var f = obj as Tag;
            return f != null && f.Id == Id && f.EntryType == EntryType && Equals(f.EntryId, EntryId);
        }

        public override int GetHashCode()
        {
            return (Id + EntryType + EntryId.ToString()).GetHashCode();
        }
    }

    [Flags]
    public enum TagType
    {
        New = 1,
        //Favorite = 2,
        System = 4
    }
}