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
using System.Web;
using ASC.Web.Core.Client.HttpHandlers;

namespace ASC.Web.CRM.Masters.ClientScripts
{
    public class ClientTemplateResources : ClientScriptLocalization
    {
        protected override string BaseNamespace
        {
            get { return "ASC.CRM.Master"; }
        }

        protected override IEnumerable<KeyValuePair<string, object>> GetClientVariables(HttpContext context)
        {
            yield return RegisterClientTemplatesPath("~/products/crm/templates/CasesTemplates.ascx", context);
            yield return RegisterClientTemplatesPath("~/products/crm/templates/CommonCustomFieldsTemplates.ascx", context);
            yield return RegisterClientTemplatesPath("~/products/crm/templates/CommonTemplates.ascx", context);
            yield return RegisterClientTemplatesPath("~/products/crm/templates/ContactsTemplates.ascx", context);
            yield return RegisterClientTemplatesPath("~/products/crm/templates/DealsTemplates.ascx", context);
            yield return RegisterClientTemplatesPath("~/products/crm/templates/SimpleContactListTemplate.ascx", context);
            yield return RegisterClientTemplatesPath("~/products/crm/templates/TasksTemplates.ascx", context);
            yield return RegisterClientTemplatesPath("~/products/crm/templates/ContactSelectorTemplates.ascx", context);
            yield return RegisterClientTemplatesPath("~/products/crm/templates/ContactInfoCardTemplate.ascx", context);
            yield return RegisterClientTemplatesPath("~/products/crm/templates/SettingsTemplates.ascx", context);

            yield return RegisterClientTemplatesPath("~/products/projects/ProjectsTemplates/ListProjectsTemplates.ascx", context);
        }
    }
}