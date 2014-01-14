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

#region Usings

using System;

#endregion

namespace ASC.Projects.Core.Domain
{
    public abstract class DomainObject<TID> where TID : struct
    {
        public TID ID { get; set; }

        public virtual string UniqID
        {
            get { return DoUniqId(GetType(), ID); }
        }

        internal static string DoUniqId(Type type, TID id)
        {
            return string.Format("{0}_{1}", type.Name, id);
        }

        public override int GetHashCode()
        {
            return (GetType().FullName + "|" + ID.GetHashCode()).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var compareTo = obj as DomainObject<TID>;
            return compareTo != null && (
                (!IsTransient() && !compareTo.IsTransient() && ID.Equals(compareTo.ID)) ||
                ((IsTransient() || compareTo.IsTransient()) && GetHashCode().Equals(compareTo.GetHashCode())));
        }

        private bool IsTransient()
        {
            return ID.Equals(default(TID));
        }
    }
}
