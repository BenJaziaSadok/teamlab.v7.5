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
using System.Linq;

namespace ASC.Data.Backup.Extensions
{
    public class TreeNode<TEntry>
    {
        public TEntry Entry { get; set; }
        public TreeNode<TEntry> Parent { get; set; }
        public List<TreeNode<TEntry>> Children { get; private set; }

        public TreeNode()
        {
            Children = new List<TreeNode<TEntry>>();
        }

        public TreeNode(TEntry entry)
            :this()
        {
            Entry = entry;
            Parent = null;
        }
    }

    public static class EnumerableExtensions
    {
        public static IEnumerable<TreeNode<TEntry>> ToTree<TEntry, TKey>(this IEnumerable<TEntry> elements,
                                                                         Func<TEntry, TKey> keySelector,
                                                                         Func<TEntry, TKey> parentKeySelector)
        {
            if (elements == null)
                throw new ArgumentNullException("elements");

            if (keySelector == null)
                throw new ArgumentNullException("keySelector");

            if (parentKeySelector == null)
                throw new ArgumentNullException("parentKeySelector");

            var dic = elements.ToDictionary(keySelector, x => new TreeNode<TEntry>(x));
            foreach (var keyValue in dic)
            {
                var parentKey = parentKeySelector(keyValue.Value.Entry);
                TreeNode<TEntry> parent;
                if (parentKey != null && dic.TryGetValue(parentKeySelector(keyValue.Value.Entry), out parent))
                {
                    parent.Children.Add(keyValue.Value);
                    keyValue.Value.Parent = parent;
                }
            }

            return dic.Values.Where(x => x.Parent == null);
        }

        public static IEnumerable<IEnumerable<TEntry>> MakeParts<TEntry>(this IEnumerable<TEntry> collection, int partLength)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            if (partLength <= 0)
                throw new ArgumentOutOfRangeException("partLength", partLength, "Length must be positive integer");

            var part = new List<TEntry>(partLength);

            foreach (var entry in collection)
            {
                part.Add(entry);

                if (part.Count == partLength)
                {
                    yield return part.AsEnumerable();
                    part = new List<TEntry>(partLength);
                }
            }

            if (part.Count > 0)
            {
                yield return part.AsEnumerable();
            }
        }
    }
}
