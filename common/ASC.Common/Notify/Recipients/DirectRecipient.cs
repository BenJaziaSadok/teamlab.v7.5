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

#region usings

using System;
using System.Collections.Generic;

#endregion

namespace ASC.Notify.Recipients
{
    [Serializable]
    public class DirectRecipient
        : IDirectRecipient
    {
        private readonly List<string> _Addresses = new List<string>();

        public DirectRecipient(string id, string name)
        {
            ID = id;
            Name = name;
        }

        public DirectRecipient(string id, string name, string[] addresses)
            : this(id, name, addresses, true)
        {
        }

        public DirectRecipient(string id, string name, string[] addresses, bool checkActivation)
        {
            ID = id;
            Name = name;
            CheckActivation = checkActivation;
            if (addresses != null)
                _Addresses.AddRange(addresses);
        }

        #region IDirectRecipient

        public string[] Addresses
        {
            get { return _Addresses.ToArray(); }
        }



        #endregion

        #region IRecipient

        public string ID { get; private set; }

        public string Name { get; private set; }
        public bool CheckActivation { get; set; }

        #endregion

        public override bool Equals(object obj)
        {
            var recD = obj as IDirectRecipient;
            if (recD == null) return false;
            return Equals(recD.ID, ID);
        }

        public override int GetHashCode()
        {
            return (ID ?? "").GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("{0}({1})", Name, String.Join(";", _Addresses.ToArray()));
        }
    }
}