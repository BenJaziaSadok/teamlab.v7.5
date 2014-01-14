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
    public class ChunkedUploadSession
    {
        public string Id { get; set; }

        public DateTime Created { get; set; }

        public DateTime Expired { get; set; }

        public string Location { get; set; }

        public long BytesUploaded { get; set; }

        public long BytesTotal { get; set; }

        public File File { get; set; }

        public int TenantId { get; set; }

        public Guid UserId { get; set; }

        public bool UseChunks { get; set; }

        public readonly Dictionary<string, object> Items = new Dictionary<string, object>();

        public ChunkedUploadSession(File file, long bytesTotal)
        {
            Id = Guid.NewGuid().ToString("N");
            Created = DateTime.UtcNow;
            BytesUploaded = 0;
            BytesTotal = bytesTotal;
            File = file;
            UseChunks = true;
        }

        public T GetItemOrDefault<T>(string key)
        {
            return Items.ContainsKey(key) && Items[key] is T ? (T)Items[key] : default(T);
        }
    }
}
