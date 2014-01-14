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

namespace ASC.Mail.Service
{
    public class UriTemplates
    {

        //Sending
        public const string SendMessage             = "send_message/{id}";
        public const string SaveMessage             = "save_message/{id}";

        //filter
        public const string FilteredIds             = "filtered_ids";
        public const string SearchContacts          = "search_contacts";
    }
}