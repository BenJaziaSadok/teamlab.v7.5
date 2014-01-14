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
using System.Linq;
using ASC.Api.Attributes;
using ASC.Core;
using ASC.Mail.Aggregator;
using ASC.Mail.Aggregator.DbSchema;
using ASC.Web.Core;

namespace ASC.Api.Mail
{
    public partial class MailApi
    {
        /// <summary>
        ///    Returns the list of the contacts for auto complete feature.
        /// </summary>
        /// <param name="term">string part of contact name, lastname or email.</param>
        /// <returns>Strings list.  Strings format: "Name Lastname"</returns>
        /// <short>Get contact list for auto complete</short> 
        /// <category>Contacts</category>
        ///<exception cref="ArgumentException">Exception happens when in parameters is invalid. Text description contains parameter name and text description.</exception>
        [Read(@"contacts")]
        public IEnumerable<string> GetContacts(string term)
        {
            if (string.IsNullOrEmpty(term))
                throw new ArgumentException("term parameter empty.", "term");

            var equality = new ContactEqualityComparer();
            var contacts = mailBoxManager.SearchMailContacts(TenantId, Username, term).Distinct(equality).ToList();
            if (WebItemSecurity.IsAvailableForUser(WebItemManager.CRMProductID.ToString(), SecurityContext.CurrentAccount.ID))
                contacts = contacts.Concat(mailBoxManager.SearchCRMContacts(TenantId, Username, term)).Distinct(equality).ToList();
            if (WebItemSecurity.IsAvailableForUser(WebItemManager.PeopleProductID.ToString(), SecurityContext.CurrentAccount.ID))
                contacts = contacts.Concat(mailBoxManager.SearchTeamLabContacts(TenantId, term)).Distinct(equality).ToList();
            return contacts;
        }

        /// <summary>
        ///    Returns list of crm entities linked with chain. Entity: contact, case or opportunity.
        /// </summary>
        /// <param name="message_id">Id of message included in the chain. It may be id any of messages included in the chain.</param>
        /// <returns>List of structures: {entity_id, entity_type, avatar_link, title}</returns>
        /// <short>Get crm linked entities</short> 
        /// <category>Contacts</category>
        ///<exception cref="ArgumentException">Exception happens when in parameters is invalid. Text description contains parameter name and text description.</exception>
        [Read(@"crm/linked/entities")]
        public IEnumerable<CrmContactEntity> GetLinkedCrmEntitiesInfo(int message_id)
        {
            if(message_id < 0)
                throw new ArgumentException("meesage_id must be positive integer", "message_id");

            return mailBoxManager.GetLinkedCrmEntitiesId(message_id, TenantId, Username);
        }
    }
}
