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

namespace ASC.Notify.Model
{
    [Serializable]
    public class NotifyAction : INotifyAction
    {
        public string ID { get; private set; }

        public string Name { get; private set; }


        public NotifyAction(string id)
            : this(id, null)
        {
        }

        public NotifyAction(string id, string name)
        {
            if (id == null) throw new ArgumentNullException("id");

            ID = id;
            Name = name;
        }

        public override bool Equals(object obj)
        {
            var a = obj as INotifyAction;
            return a != null && a.ID == ID;
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("action: {0}", ID);
        }
    }
}