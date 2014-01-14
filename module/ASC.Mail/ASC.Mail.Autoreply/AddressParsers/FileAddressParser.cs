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

using System.Collections.Generic;
using System.Text.RegularExpressions;
using ASC.Core.Tenants;

namespace ASC.Mail.Autoreply.AddressParsers
{
    internal class FileAddressParser : AddressParser
    {
        protected override Regex GetRouteRegex()
        {
            return new Regex(@"^file_(?'folder'my|common|share|\d+)$", RegexOptions.Compiled);
        }

        protected override ApiRequest ParseRequestInfo(IDictionary<string,string> groups, Tenant t)
        {
            var folder = groups["folder"];
            
            int id;
            if (!int.TryParse(folder, out id))
            {
                folder = '@' + folder;
            }

            return new ApiRequest(string.Format("files/{0}/upload", folder)) {FilesToPost = new List<RequestFileInfo>()};
        }
    }
}