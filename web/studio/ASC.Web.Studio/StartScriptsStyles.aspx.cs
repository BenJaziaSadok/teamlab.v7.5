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
using System.Web.Optimization;
using System.Web.UI;

namespace ASC.Web.Studio.UserControls.FirstTime
{
    public partial class StartScriptsStyles : Page
    {
        protected List<String> ListUri { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            ListUri = new List<string>();
            var module = Request["module"];
            foreach (var bundle in BundleTable.Bundles)
            {
                if (0 <= bundle.Path.IndexOf("/common", StringComparison.InvariantCultureIgnoreCase) ||
                    (!string.IsNullOrWhiteSpace(module) && 0 <= bundle.Path.IndexOf("/" + module, StringComparison.InvariantCultureIgnoreCase)))
                {
                    ListUri.Add(BundleTable.Bundles.ResolveBundleUrl(bundle.Path));
                }
            }
        }
    }
}