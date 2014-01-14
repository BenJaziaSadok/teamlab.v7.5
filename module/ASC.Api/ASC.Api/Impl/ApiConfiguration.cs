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

#region usings

using ASC.Api.Interfaces;

#endregion

namespace ASC.Api.Impl
{
    internal class ApiConfiguration : IApiConfiguration
    {
        public ApiConfiguration()
        {
            ApiPrefix = "api";
            ApiVersion = "1.0";
            ApiSeparator = '/';
        }

        #region IApiConfiguration Members

        public string ApiPrefix { get; set; }
        public string ApiVersion { get; set; }
        public char ApiSeparator { get; set; }

        public string GetBasePath()
        {
            return (ApiPrefix + ApiSeparator + ApiVersion + ApiSeparator).TrimStart('/', '~');
        }

        public uint ItemsPerPage
        {
            get { return 25; }
        }

        #endregion
    }
}