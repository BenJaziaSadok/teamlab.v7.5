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
using System.Web;
using ASC.Web.CRM.Resources;
using ASC.Web.Core.Client.HttpHandlers;

namespace ASC.Web.CRM.Masters.ClientScripts
{
    public class ClientLocalizationResources : ClientScriptLocalization
    {
        protected override string BaseNamespace
        {
            get { return "ASC.CRM.Resources"; }
        }

        protected override IEnumerable<KeyValuePair<string, object>> GetClientVariables(HttpContext context)
        {
            yield return RegisterResourceSet("CRMJSResource", CRMJSResource.ResourceManager);
            yield return RegisterResourceSet("CRMCommonResource", CRMCommonResource.ResourceManager);
            yield return RegisterResourceSet("CRMContactResource", CRMContactResource.ResourceManager);
            yield return RegisterResourceSet("CRMDealResource", CRMDealResource.ResourceManager);
            yield return RegisterResourceSet("CRMTaskResource", CRMTaskResource.ResourceManager);
            yield return RegisterResourceSet("CRMCasesResource", CRMCasesResource.ResourceManager);
            yield return RegisterResourceSet("CRMEnumResource", CRMEnumResource.ResourceManager);
        }
    }

    public class ClientCustomResources : ClientScriptCustom
    {
        protected override string BaseNamespace
        {
            get { return "ASC.CRM.Resources"; }
        }

        protected override IEnumerable<KeyValuePair<string, object>> GetClientVariables(HttpContext context)
        {
            yield return RegisterObject("AddUser", Studio.Core.Users.CustomNamingPeople.Substitute<CRMCommonResource>("AddUser").HtmlEncode());
            yield return RegisterObject("AddGroup", Studio.Core.Users.CustomNamingPeople.Substitute<CRMCommonResource>("AddGroup").HtmlEncode());
            yield return RegisterObject("CurrentUser", Studio.Core.Users.CustomNamingPeople.Substitute<CRMCommonResource>("CurrentUser").HtmlEncode());
        }
    }
}