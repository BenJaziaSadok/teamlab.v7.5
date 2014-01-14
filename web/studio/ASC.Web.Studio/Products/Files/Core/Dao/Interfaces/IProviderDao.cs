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
    public interface IProviderDao : IDisposable
    {
        IProviderInfo GetProviderInfo(int linkId);
        List<IProviderInfo> GetProvidersInfo();
        List<IProviderInfo> GetProvidersInfo(FolderType folderType);
        int SaveProviderInfo(string providerKey, string customerTitle, AuthData authData, FolderType folderType);
        int UpdateProviderInfo(int linkId, string customerTitle, FolderType folderType);
        void RemoveProviderInfo(int linkId);
    }
}