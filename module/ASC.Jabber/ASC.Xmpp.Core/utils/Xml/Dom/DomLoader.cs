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

#region using

using System.IO;
using System.Text;

#endregion

namespace ASC.Xmpp.Core.utils.Xml.Dom
{

    #region usings

    #endregion

    /// <summary>
    ///   internal class that loads a xml document from a string or stream
    /// </summary>
    public class DomLoader
    {
        #region Members

        /// <summary>
        /// </summary>
        private readonly Document doc;

        /// <summary>
        /// </summary>
        private readonly StreamParser sp;

        #endregion

        #region Constructor

        /// <summary>
        /// </summary>
        /// <param name="xml"> </param>
        /// <param name="d"> </param>
        public DomLoader(string xml, Document d)
        {
            doc = d;
            sp = new StreamParser();

            sp.OnStreamStart += sp_OnStreamStart;
            sp.OnStreamElement += sp_OnStreamElement;
            sp.OnStreamEnd += sp_OnStreamEnd;

            byte[] b = Encoding.UTF8.GetBytes(xml);
            sp.Push(b, 0, b.Length);
        }

        /// <summary>
        /// </summary>
        /// <param name="sr"> </param>
        /// <param name="d"> </param>
        public DomLoader(StreamReader sr, Document d) : this(sr.ReadToEnd(), d)
        {
        }

        #endregion

        #region Utility methods

        /// <summary>
        /// </summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>
        private void sp_OnStreamStart(object sender, Node e, string streamNamespace)
        {
            doc.ChildNodes.Add(e);
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>
        private void sp_OnStreamElement(object sender, Node e)
        {
            doc.RootElement.ChildNodes.Add(e);
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>
        private void sp_OnStreamEnd(object sender, Node e)
        {
        }

        #endregion

        // ya, the Streamparser is only usable for parsing xmpp stream.
        // it also does a very good job here
    }
}