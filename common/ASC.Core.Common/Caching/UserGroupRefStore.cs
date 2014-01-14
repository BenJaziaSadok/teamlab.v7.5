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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ASC.Core.Caching
{
    class UserGroupRefStore : IDictionary<string, UserGroupRef>
    {
        private readonly IDictionary<string, UserGroupRef> refs;
        private ILookup<Guid, UserGroupRef> index;
        private bool changed;


        public UserGroupRefStore(IDictionary<string, UserGroupRef> refs)
        {
            this.refs = refs;
            changed = true;
        }


        public void Add(string key, UserGroupRef value)
        {
            refs.Add(key, value);
            RebuildIndex();
        }

        public bool ContainsKey(string key)
        {
            return refs.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get { return refs.Keys; }
        }

        public bool Remove(string key)
        {
            var result = refs.Remove(key);
            RebuildIndex();
            return result;
        }

        public bool TryGetValue(string key, out UserGroupRef value)
        {
            return refs.TryGetValue(key, out value);
        }

        public ICollection<UserGroupRef> Values
        {
            get { return refs.Values; }
        }

        public UserGroupRef this[string key]
        {
            get
            {
                return refs[key];
            }
            set
            {
                refs[key] = value;
                RebuildIndex();
            }
        }

        public void Add(KeyValuePair<string, UserGroupRef> item)
        {
            refs.Add(item);
            RebuildIndex();
        }

        public void Clear()
        {
            refs.Clear();
            RebuildIndex();
        }

        public bool Contains(KeyValuePair<string, UserGroupRef> item)
        {
            return refs.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, UserGroupRef>[] array, int arrayIndex)
        {
            refs.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return refs.Count; }
        }

        public bool IsReadOnly
        {
            get { return refs.IsReadOnly; }
        }

        public bool Remove(KeyValuePair<string, UserGroupRef> item)
        {
            var result = refs.Remove(item);
            RebuildIndex();
            return result;
        }

        public IEnumerator<KeyValuePair<string, UserGroupRef>> GetEnumerator()
        {
            return refs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return refs.GetEnumerator();
        }

        public IEnumerable<UserGroupRef> GetRefsByUser(Guid userId)
        {
            if (changed)
            {
                index = refs.Values.ToLookup(r => r.UserId);
                changed = false;
            }
            return index[userId];
        }

        private void RebuildIndex()
        {
            changed = true;
        }
    }
}
