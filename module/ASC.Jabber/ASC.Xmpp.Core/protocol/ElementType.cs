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

#region file header

#endregion

namespace ASC.Xmpp.Core.protocol
{
    /// <summary>
    /// </summary>
    public class ElementType
    {
        #region Members

        /// <summary>
        /// </summary>
        private readonly string m_Namespace;

        /// <summary>
        /// </summary>
        private readonly string m_TagName;

        #endregion

        #region Constructor

        /// <summary>
        /// </summary>
        /// <param name="TagName"> </param>
        /// <param name="Namespace"> </param>
        public ElementType(string TagName, string Namespace)
        {
            m_TagName = TagName;
            m_Namespace = Namespace;
        }

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <returns> </returns>
        public override string ToString()
        {
            if ((m_Namespace != null) && (m_Namespace != string.Empty))
            {
                return m_Namespace + ":" + m_TagName;
            }

            return m_TagName;
        }

        #endregion
    }
}