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

namespace ASC.Web.Mail.Masters.ClientScripts
{
    public class ClientTemplateResources : ClientScriptLocalization
    {
        protected override string BaseNamespace
        {
            get { return "ASC.Mail.Master"; }
        }

        protected override IEnumerable<KeyValuePair<string, object>> GetClientVariables(HttpContext context)
        {
            yield return RegisterClientTemplatesPath("~/addons/mail/templates/foldersTmpl.ascx", context);
            yield return RegisterClientTemplatesPath("~/addons/mail/templates/messagesTmpl.ascx", context);
            yield return RegisterClientTemplatesPath("~/addons/mail/templates/messageTmpl.ascx", context);
            yield return RegisterClientTemplatesPath("~/addons/mail/templates/editMessageTmpl.ascx", context);
            yield return RegisterClientTemplatesPath("~/addons/mail/templates/accountTmpl.ascx", context);
            yield return RegisterClientTemplatesPath("~/addons/mail/templates/accountErrorTmpl.ascx", context);
            yield return RegisterClientTemplatesPath("~/addons/mail/templates/accountWizardTmpl.ascx", context);
            yield return RegisterClientTemplatesPath("~/addons/mail/templates/accountsPanelTmpl.ascx", context);
            yield return RegisterClientTemplatesPath("~/addons/mail/templates/contactsTmpl.ascx", context);
            yield return RegisterClientTemplatesPath("~/addons/mail/templates/accountsTmpl.ascx", context);
            yield return RegisterClientTemplatesPath("~/addons/mail/templates/tagsTmpl.ascx", context);
            yield return RegisterClientTemplatesPath("~/addons/mail/templates/crmLinkPopup.ascx", context);
            yield return RegisterClientTemplatesPath("~/addons/mail/templates/crmExportPopup.ascx", context);
            yield return RegisterClientTemplatesPath("~/addons/mail/templates/crmLinkedContactsTmpl.ascx", context);
            yield return RegisterClientTemplatesPath("~/addons/mail/templates/addFirstAccountTmpl.ascx", context);
            yield return RegisterClientTemplatesPath("~/addons/mail/templates/composeMessageBodyTmpl.ascx", context);
        }
    }
}