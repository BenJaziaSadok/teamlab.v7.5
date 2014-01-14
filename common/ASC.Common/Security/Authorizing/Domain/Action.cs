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

namespace ASC.Common.Security.Authorizing
{
    [Serializable]
    public class Action : IAction
    {
        public Guid ID { get; private set; }

        public string Name { get; private set; }

        public bool AdministratorAlwaysAllow { get; private set; }

        public bool Conjunction { get; private set; }


        public Action(Guid id, string name)
            : this(id, name, true, true)
        {
        }

        public Action(Guid id, string name, bool administratorAlwaysAllow, bool conjunction)
        {
            if (id == Guid.Empty) throw new ArgumentNullException("id");

            ID = id;
            Name = name;
            AdministratorAlwaysAllow = administratorAlwaysAllow;
            Conjunction = conjunction;
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var a = obj as Action;
            return a != null && a.ID == ID;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
