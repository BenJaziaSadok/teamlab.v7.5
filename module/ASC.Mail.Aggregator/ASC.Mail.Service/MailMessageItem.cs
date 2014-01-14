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

namespace ASC.Mail.Service
{
    public class MailMessageItem
    {
        public string Subject { get; set; }
        public DateTime Date { get; set; }
        public string To { get; set; }
        public string From { get; set; }
        public int Size { get; set; }
        public bool HasAttachments { get; set; }
        public bool Important { get; set; }
        public string Priority { get; set; }
        public string Link { get; set; }
    }
}