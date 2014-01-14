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
using System.Collections;

#endregion

namespace ASC.Notify.Cron
{

    #region

    #endregion

    [Serializable]
    public class TreeSet : ArrayList, ISortedSet
    {
        #region Members

        private readonly IComparer comparator = Comparer.Default;

        #endregion

        #region Properties

        public IComparer Comparator
        {
            get { return comparator; }
        }

        #endregion

        #region Constructor

        public TreeSet()
        {
        }

        public TreeSet(ICollection c)
        {
            AddAll(c);
        }

        public TreeSet(IComparer c)
        {
            comparator = c;
        }

        #endregion

        #region Methods

        public new bool Add(object obj)
        {
            bool inserted = AddWithoutSorting(obj);
            Sort(comparator);
            return inserted;
        }

        public bool AddAll(ICollection c)
        {
            IEnumerator e = new ArrayList(c).GetEnumerator();
            bool added = false;
            while (e.MoveNext())
            {
                if (AddWithoutSorting(e.Current))
                {
                    added = true;
                }
            }
            Sort(comparator);
            return added;
        }

        public object First()
        {
            return this[0];
        }

        public override bool Contains(object item)
        {
            IEnumerator tempEnumerator = GetEnumerator();
            while (tempEnumerator.MoveNext())
            {
                if (comparator.Compare(tempEnumerator.Current, item) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        public ISortedSet TailSet(object limit)
        {
            ISortedSet newList = new TreeSet();
            int i = 0;
            while ((i < Count) && (comparator.Compare(this[i], limit) < 0))
            {
                i++;
            }
            for (; i < Count; i++)
            {
                newList.Add(this[i]);
            }
            return newList;
        }

        public static TreeSet UnmodifiableTreeSet(ICollection collection)
        {
            var items = new ArrayList(collection);
            items = ReadOnly(items);
            return new TreeSet(items);
        }

        #endregion

        #region Utility methods

        private bool AddWithoutSorting(object obj)
        {
            bool inserted;
            if (!(inserted = Contains(obj)))
            {
                base.Add(obj);
            }
            return !inserted;
        }

        #endregion
    }
}