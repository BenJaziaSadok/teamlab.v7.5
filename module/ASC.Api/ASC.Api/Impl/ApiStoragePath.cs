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
using System.IO;
using System.Web;
using ASC.Api.Interfaces;

namespace ASC.Api.Impl
{
    internal class ApiStoragePath : IApiStoragePath
    {
        #region IApiStoragePath Members

        public string GetDataDirectory(IApiEntryPoint entryPoint)
        {
            string basePath;
            if (HttpContext.Current != null)
            {
                basePath = HttpContext.Current.Server.MapPath("~/EntryPointData");
            }
            else
            {
                basePath = AppDomain.CurrentDomain.GetData("Data Directory") as string;
                if (string.IsNullOrEmpty(basePath))
                {
                    basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EntryPointData");
                }
            }
            if (string.IsNullOrEmpty(basePath))
            {
                throw new InvalidOperationException("failed to resolve data directory");
            }
            string apidata = Path.Combine(basePath, entryPoint.Name);
            if (!Directory.Exists(apidata))
            {
                Directory.CreateDirectory(apidata);
            }
            return apidata;
        }

        #endregion
    }
}