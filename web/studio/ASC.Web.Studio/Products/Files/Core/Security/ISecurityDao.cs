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

namespace ASC.Files.Core.Security
{
    public interface ISecurityDao : IDisposable
    {
        void SetShare(FileShareRecord r);

        IEnumerable<FileShareRecord> GetShares(IEnumerable<Guid> subjects);

        IEnumerable<FileShareRecord> GetShares(params FileEntry[] entry);

        void RemoveSubject(Guid subject);

        IEnumerable<FileShareRecord> GetPureShareRecords(params FileEntry[] entries);

        void DeleteShareRecords(params FileShareRecord[] records);
    }
}