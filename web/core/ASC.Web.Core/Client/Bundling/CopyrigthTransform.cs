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
using System.Linq;
using System.Web.Optimization;

namespace ASC.Web.Core.Client.Bundling
{
    class CopyrigthTransform : IBundleTransform
    {
        public static readonly string CopyrigthText = @"/*
    Copyright (c) Ascensio System SIA " + DateTime.UtcNow.Year + @". All rights reserved.
    http://www.teamlab.com
*/
";
        public void Process(BundleContext context, BundleResponse response)
        {
            if (!response.Files.Any(f => f.VirtualFile.VirtualPath.ToLowerInvariant().Contains("jquery")))
            {
                response.Content = CopyrigthText + response.Content;
            }
        }
    }
}