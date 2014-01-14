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
    public interface IFileSecurity
    {
        bool CanRead(FileEntry file, Guid userId);

        bool CanCreate(FileEntry file, Guid userId);

        bool CanEdit(FileEntry file, Guid userId);

        bool CanDelete(FileEntry file, Guid userId);
    }
}