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
using System.ServiceModel;
using System.ServiceModel.Web;
using ASC.Mail.Aggregator;
using ASC.Mail.Aggregator.Collection;
using ASC.Mail.Aggregator.Filter;
using ASC.Mail.Service.DAO;
using Microsoft.ServiceModel.Web;
using ASC.Mail.Aggregator.WebRequestsDataWrappers;
using System.Collections.Generic;

namespace ASC.Mail.Service
{
    [ServiceContract]
    public interface IMailService
    {
        [WebHelp(Comment = "Sends message.")]
        [WebInvoke(Method = "POST", UriTemplate = UriTemplates.SendMessage, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Xml)]
        [OperationContract]
        int SendMessage(string id, MailSendItem item);

        [WebHelp(Comment = "Saves item to_addresses draft.")]
        [WebInvoke(Method = "POST", UriTemplate = UriTemplates.SaveMessage, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Xml)]
        [OperationContract]
        int SaveMessage(string id, MailSendItem item);

        [WebHelp(Comment = "Returns ids of filtered messages.")]
        [WebInvoke(Method = "POST", UriTemplate = UriTemplates.FilteredIds, RequestFormat = WebMessageFormat.Xml, ResponseFormat = WebMessageFormat.Xml)]
        [OperationContract]
        ItemList<int> GetFilteredMessagesIds(MailFilter filter);

        [WebHelp(Comment = "Return email contacts")]
        [WebInvoke(Method = "POST", UriTemplate = UriTemplates.SearchContacts, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Xml)]
        [OperationContract]
        ItemList<string> SearchContacts(string term);
    }
}