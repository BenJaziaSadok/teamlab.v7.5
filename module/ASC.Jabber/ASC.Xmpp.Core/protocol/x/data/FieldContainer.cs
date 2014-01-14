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

namespace ASC.Xmpp.Core.protocol.x.data
{

    #region usings

    #endregion

    /// <summary>
    ///   Bass class for all xdata classes that contain xData fields
    /// </summary>
    public abstract class FieldContainer : Element
    {
        #region Methods

        /// <summary>
        /// </summary>
        /// <returns> </returns>
        public Field AddField()
        {
            var f = new Field();
            AddChild(f);
            return f;
        }

        /// <summary>
        /// </summary>
        /// <param name="field"> </param>
        public Field AddField(Field field)
        {
            AddChild(field);
            return field;
        }

        /// <summary>
        ///   Retrieve a field with the given "var"
        /// </summary>
        /// <param name="var"> </param>
        /// <returns> </returns>
        public Field GetField(string var)
        {
            ElementList nl = SelectElements(typeof (Field));
            foreach (Element e in nl)
            {
                var f = e as Field;
                if (f.Var == var)
                {
                    return f;
                }
            }

            return null;
        }

        /// <summary>
        ///   Gets a list of all form fields
        /// </summary>
        /// <returns> </returns>
        public Field[] GetFields()
        {
            ElementList nl = SelectElements(typeof (Field));
            var fields = new Field[nl.Count];
            int i = 0;
            foreach (Element e in nl)
            {
                fields[i] = (Field) e;
                i++;
            }

            return fields;
        }

        #endregion
    }
}