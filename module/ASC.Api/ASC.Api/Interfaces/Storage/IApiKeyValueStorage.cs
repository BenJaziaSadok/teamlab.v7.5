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

namespace ASC.Api.Interfaces.Storage
{
    public interface IApiKeyValueStorage
    {
        object Get(IApiEntryPoint entrypoint, string key);
        void Set(IApiEntryPoint entrypoint, string key, object @object);
        bool Exists(IApiEntryPoint entrypoint, string key);
        void Remove(IApiEntryPoint entrypoint, string key);
        void Clear(IApiEntryPoint entrypoint);
    }
}