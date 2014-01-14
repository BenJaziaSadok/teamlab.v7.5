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
using System.Resources;
using System.Globalization;

namespace TMResourceData
{
    public class DBResourceSet : ResourceSet
    {
        internal DBResourceSet(string fileName, CultureInfo culture)
            : base(new DBResourceReader(fileName, culture)) { }


        public override Type GetDefaultReader()
        {
            return typeof(DBResourceReader);
        }

        public void SetString(object name, object newvalue)
        {
            if (Table[name] != null)
                Table[name] = newvalue;
            else
                Table.Add(name, newvalue);
        }

        public int TableCount
        {
            get
            {
                return Table.Count;
            }
        }
    }
}
