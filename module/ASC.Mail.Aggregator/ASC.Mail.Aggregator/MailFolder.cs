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
using System.Runtime.Serialization;

namespace ASC.Mail.Aggregator
{
    public class MailFolder
	{
        public static class Ids
        {
            public const int inbox = 1;
            public const int sent = 2;
            public const int drafts = 3;
            public const int trash = 4;
            public const int spam = 5;
        }

        public static bool IsIdOk(int folder_id)
        {
            return folder_id >= 0 && folder_id <= 5;
        }
	}
}
