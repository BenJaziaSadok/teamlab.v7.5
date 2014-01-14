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

#endregion

namespace ASC.Xmpp.Core.utils.Xml.Dom
{

    #region usings

    #endregion

    /// <summary>
    /// </summary>
    public class Document : Node
    {
        #region Members

        #endregion

        #region Constructor

        /// <summary>
        /// </summary>
        public Document()
        {
            NodeType = NodeType.Document;
        }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public string Encoding { get; set; }

        /// <summary>
        /// </summary>
        public Element RootElement
        {
            get
            {
                foreach (Node n in ChildNodes)
                {
                    if (n.NodeType == NodeType.Element)
                    {
                        return n as Element;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// </summary>
        public string Version { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///   Clears the Document
        /// </summary>
        public void Clear()
        {
            ChildNodes.Clear();
        }

        /// <summary>
        /// </summary>
        /// <param name="xml"> </param>
        public void LoadXml(string xml)
        {
            if (xml != string.Empty && xml != null)
            {
                var l = new DomLoader(xml, this);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="filename"> </param>
        /// <returns> </returns>
        public bool LoadFile(string filename)
        {
            if (File.Exists(filename))
            {
                try
                {
                    var sr = new StreamReader(filename);
                    var l = new DomLoader(sr, this);
                    sr.Close();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="stream"> </param>
        /// <returns> </returns>
        public bool LoadStream(Stream stream)
        {
            try
            {
                var sr = new StreamReader(stream);
                var l = new DomLoader(sr, this);
                sr.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="filename"> </param>
        public void Save(string filename)
        {
            var w = new StreamWriter(filename);

            w.Write(ToString(System.Text.Encoding.UTF8));
            w.Flush();
            w.Close();
        }

        #endregion
    }
}