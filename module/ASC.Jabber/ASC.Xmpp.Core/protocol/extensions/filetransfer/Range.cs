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

using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.extensions.filetransfer
{
    /// <summary>
    ///   When range is sent in the offer, it should have no attributes. This signifies that the sender can do ranged transfers. When no range element is sent in the Stream Initiation result, the Sender MUST send the complete file starting at offset 0. More generally, data is sent over the stream byte for byte starting at the offset position for the length specified.
    /// </summary>
    public class Range : Element
    {
        /*
		<range offset='252' length='179'/>		    	
		*/

        public Range()
        {
            TagName = "range";
            Namespace = Uri.SI_FILE_TRANSFER;
        }

        public Range(long offset, long length) : this()
        {
            Offset = offset;
            Length = length;
        }

        /// <summary>
        ///   Specifies the position, in bytes, to start transferring the file data from. This defaults to zero (0) if not specified.
        /// </summary>
        public long Offset
        {
            get { return GetAttributeLong("offset"); }
            set { SetAttribute("offset", value.ToString()); }
        }

        /// <summary>
        ///   Specifies the number of bytes to retrieve starting at offset. This defaults to the length of the file from offset to the end.
        /// </summary>
        public long Length
        {
            get { return GetAttributeLong("length"); }
            set { SetAttribute("length", value.ToString()); }
        }
    }
}