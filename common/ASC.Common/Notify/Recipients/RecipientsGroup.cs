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

#endregion

namespace ASC.Notify.Recipients
{
    [Serializable]
    public class RecipientsGroup
        : IRecipientsGroup
    {
        public RecipientsGroup(string id, string name)
        {
            ID = id;
            Name = name;
        }

        #region IRecipientsGroup Members

        public string ID { get; private set; }

        public string Name { get; private set; }

        #endregion

        public override bool Equals(object obj)
        {
            var recGr = obj as IRecipientsGroup;
            if (recGr == null) return false;
            return Equals(recGr.ID, ID);
        }

        public override int GetHashCode()
        {
            return (ID ?? "").GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("{0}", Name);
        }
    }
}