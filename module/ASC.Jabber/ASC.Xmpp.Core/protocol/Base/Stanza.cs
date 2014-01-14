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

namespace ASC.Xmpp.Core.protocol.Base
{
    /// <summary>
    ///   Base XMPP Element This must ne used to build all other new packets
    /// </summary>
    public abstract class Stanza : DirectionalElement
    {
        #region Constructor

        /// <summary>
        /// </summary>
        public Stanza()
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="tag"> </param>
        public Stanza(string tag) : base(tag)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="tag"> </param>
        /// <param name="ns"> </param>
        public Stanza(string tag, string ns) : base(tag)
        {
            Namespace = ns;
        }

        /// <summary>
        /// </summary>
        /// <param name="tag"> </param>
        /// <param name="text"> </param>
        /// <param name="ns"> </param>
        public Stanza(string tag, string text, string ns) : base(tag, text)
        {
            Namespace = ns;
        }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public string Id
        {
            get { return GetAttribute("id"); }

            set { SetAttribute("id", value); }
        }

        /// <summary>
        ///   XML Language attribute
        /// </summary>
        /// <remarks>
        ///   The language 'xml:lang' attribute SHOULD be included by the initiating entity on the header for the initial stream to specify the default language of any human-readable XML character data it sends over that stream. If the attribute is included, the receiving entity SHOULD remember that value as the default for both the initial stream and the response stream; if the attribute is not included, the receiving entity SHOULD use a configurable default value for both streams, which it MUST communicate in the header for the response stream. For all stanzas sent over the initial stream, if the initiating entity does not include an 'xml:lang' attribute, the receiving entity SHOULD apply the default value; if the initiating entity does include an 'xml:lang' attribute, the receiving entity MUST NOT modify or delete it (see also xml:langxml:lang). The value of the 'xml:lang' attribute MUST conform to the format defined in RFC 3066 (Tags for the Identification of Languages, January 2001.[LANGTAGS]).
        /// </remarks>
        public string Language
        {
            get { return GetAttribute("xml:lang"); }

            set { SetAttribute("xml:lang", value); }
        }

        public bool HasFrom
        {
            get { return From != null; }
        }

        public bool HasTo
        {
            get { return To != null; }
        }

        #endregion

        #region Methods

        /// <summary>
        ///   Generates a automatic id for the packet. !!! Overwrites existing Ids
        /// </summary>
        public void GenerateId()
        {
            string sId = protocol.Id.GetNextId();
            Id = sId;
        }

        #endregion

        ///// <summary>
        ///// Error Child Element
        ///// </summary>
        // public agsXMPP.protocol.client.Error Error
        // {
        // get
        // {
        // return SelectSingleElement(typeof(agsXMPP.protocol.client.Error)) as agsXMPP.protocol.client.Error;

        // }
        // set
        // {
        // if (HasTag(typeof(agsXMPP.protocol.client.Error)))
        // RemoveTag(typeof(agsXMPP.protocol.client.Error));

        // if (value != null)
        // this.AddChild(value);
        // }
        // }
    }
}