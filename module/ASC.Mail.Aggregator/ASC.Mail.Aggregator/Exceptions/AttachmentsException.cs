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
using System.Text;

namespace ASC.Mail.Aggregator.Exceptions
{
    public class AttachmentsException : Exception
    {
        public enum Types
        {
            UNKNOWN = 0,
            BAD_PARAMS = 1,
            EMPTY_FILE = 2,
            MESSAGE_NOT_FOUND = 3,
            TOTAL_SIZE_EXCEEDED = 4,
            DOCUMENT_NOT_FOUND = 5,
            DOCUMENT_ACCESS_DENIED = 6
        }
        public Types ErrorType { get; set; }

        public AttachmentsException(Types type, string message) : base(message) 
        {
            ErrorType = type;
        }
    }
}
