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

namespace ASC.SocialMedia
{
    /// <summary>
    /// Represents an user activity message
    /// </summary>
    public class Message
    {
        /// <summary>
        /// User name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Message text
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The date of message post
        /// </summary>
        public DateTime PostedOn { get; set; }

        /// <summary>
        /// Social network
        /// </summary>
        public SocialNetworks Source { get; set; }

    }
}
