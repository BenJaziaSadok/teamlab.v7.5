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

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ASC.Core.Common.Caching
{
    class AzRecordStore : IEnumerable<AzRecord>
    {
        private readonly Dictionary<string, List<AzRecord>> byObjectId = new Dictionary<string, List<AzRecord>>();


        public AzRecordStore(IEnumerable<AzRecord> aces)
        {
            foreach (var a in aces)
            {
                Add(a);
            }
        }


        public IEnumerable<AzRecord> Get(string objectId)
        {
            List<AzRecord> aces;
            byObjectId.TryGetValue(objectId ?? string.Empty, out aces);
            return aces ?? new List<AzRecord>();
        }

        public void Add(AzRecord r)
        {
            if (r == null) return;

            var id = r.ObjectId ?? string.Empty;
            if (!byObjectId.ContainsKey(id))
            {
                byObjectId[id] = new List<AzRecord>();
            }
            byObjectId[id].RemoveAll(a => a.SubjectId == r.SubjectId && a.ActionId == r.ActionId); // remove escape, see DbAzService
            byObjectId[id].Add(r);
        }

        public void Remove(AzRecord r)
        {
            if (r == null) return;

            var id = r.ObjectId ?? string.Empty;
            if (byObjectId.ContainsKey(id))
            {
                byObjectId[id].RemoveAll(a => a.SubjectId == r.SubjectId && a.ActionId == r.ActionId && a.Reaction == r.Reaction);
            }
        }

        public IEnumerator<AzRecord> GetEnumerator()
        {
            return byObjectId.Values.SelectMany(v => v).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}