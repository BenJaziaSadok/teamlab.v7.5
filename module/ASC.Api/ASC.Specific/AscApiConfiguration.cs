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

using System.Linq;
using ASC.Api.Interfaces;

namespace ASC.Specific
{
    public class AscApiConfiguration : IApiConfiguration
    {
        public const uint DefaultItemsPerPage = 25;

        public string ApiPrefix
        {
            get;
            set;
        }

        public string ApiVersion
        {
            get;
            set;
        }

        public char ApiSeparator
        {
            get;
            set;
        }

        public AscApiConfiguration(string version)
            : this(string.Empty, version, DefaultItemsPerPage)
        {
        }

        public AscApiConfiguration(string prefix, string version)
            : this(prefix, version, DefaultItemsPerPage)
        {
        }

        public AscApiConfiguration(string prefix, string version, uint maxPage)
        {
            ApiSeparator = '/';
            ApiPrefix = prefix??string.Empty;
            ApiVersion = version;
            ItemsPerPage = maxPage;
        }

        public string GetBasePath()
        {
            return (ApiPrefix + ApiSeparator + ApiVersion + ApiSeparator).TrimStart('/', '~');
        }

        public uint ItemsPerPage { get; private set; }
    }
}