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
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace ASC.Collections
{
    public class SynchronizedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {

        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        private readonly Dictionary<TKey, TValue> _innerDictionary;

        public SynchronizedDictionary()
        {
            _innerDictionary = new Dictionary<TKey, TValue>();
        }

        public SynchronizedDictionary(IEqualityComparer<TKey> comparer)
        {
            _innerDictionary = new Dictionary<TKey, TValue>(comparer);
        }

        public SynchronizedDictionary(int count)
        {
            _innerDictionary = new Dictionary<TKey, TValue>(count);
        }

        public SynchronizedDictionary(IDictionary<TKey, TValue> dictionary)
        {
            _innerDictionary = new Dictionary<TKey, TValue>(dictionary);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            using (GetReadLock())
            {
                return _innerDictionary.GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }



        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            using (GetWriteLock())
            {
                _innerDictionary.Clear();
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return ContainsKey(item.Key);
        }

        public IDisposable GetReadLock()
        {
            return new SlimReadLock(_lock);
        }

        public IDisposable GetWriteLock()
        {
            return new SlimWriteLock(_lock);
        }


        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        public int Count
        {
            get
            {
                using (GetReadLock())
                {
                    return _innerDictionary.Count;
                }
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool ContainsKey(TKey key)
        {
            using (GetReadLock())
            {
                return _innerDictionary.ContainsKey(key);
            }
        }

        public void Add(TKey key, TValue value)
        {
            using (GetWriteLock())
            {
                _innerDictionary.Add(key, value);
            }
        }

        public bool Remove(TKey key)
        {
            using (GetWriteLock())
            {
                return _innerDictionary.Remove(key);
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            using (GetReadLock())
            {
                return _innerDictionary.TryGetValue(key, out value);
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                TValue value;
                TryGetValue(key, out value);
                return value;
            }
            set
            {
                using (GetWriteLock())
                {
                    _innerDictionary[key] = value;
                }
            }
        }

        public ICollection<TKey> Keys
        {
            get
            {
                using (GetReadLock())
                {
                    return _innerDictionary.Keys;
                }
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                using (GetReadLock())
                {
                    return _innerDictionary.Values;
                }
            }
        }


        private class SlimReadLock : IDisposable
        {
            private readonly ReaderWriterLockSlim _locker;

            public SlimReadLock(ReaderWriterLockSlim locker)
            {
                _locker = locker;
                _locker.EnterReadLock();
            }

            public void Dispose()
            {
                _locker.ExitReadLock();
            }
        }

        private class SlimWriteLock : IDisposable
        {
            private readonly ReaderWriterLockSlim _locker;

            public SlimWriteLock(ReaderWriterLockSlim locker)
            {
                _locker = locker;
                _locker.EnterWriteLock();
            }

            public void Dispose()
            {
                _locker.ExitWriteLock();
            }
        }
    }
}