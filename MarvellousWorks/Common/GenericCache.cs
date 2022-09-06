using System;
using System.Collections.Generic;
using System.Threading;

namespace Common
{
    public class GenericCache<TKey,TValue>
    {
        private Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
        private ReaderWriterLock rwLock = new ReaderWriterLock();
        private readonly TimeSpan lockTimeOut = TimeSpan.FromMilliseconds(100);
        public void Add(TKey key,TValue value)
        {
            bool isExisting = false;
            
            rwLock.AcquireWriterLock(lockTimeOut);
            try
            {
                if (!dictionary.ContainsKey(key))
                {
                    dictionary.Add(key, value);
                }
                else
                {
                    isExisting = true;
                }
            }
            catch
            {

            }
            finally
            {
                rwLock.ReleaseWriterLock();
            }
            if (isExisting) throw new ArgumentException();
        }

        public bool TryGetValue(TKey key,TValue value)
        {
            bool result;
            rwLock.AcquireReaderLock(lockTimeOut);
            try
            {
                result = dictionary.TryGetValue(key, out value);
            }
            finally
            {
                rwLock.ReleaseReaderLock();
            }
            return result;
        }

        public void Clear()
        {
            if (dictionary.Count > 0)
            {
                rwLock.AcquireWriterLock(lockTimeOut);
                try
                {
                    dictionary.Clear();
                }
                finally
                {
                    rwLock.ReleaseWriterLock();
                }
            }
        }

        public bool ContainsKey(TKey key)
        {
            if (dictionary.Count == 0) return false;
            bool result;
            rwLock.AcquireReaderLock(lockTimeOut);
            try
            {
                result = dictionary.ContainsKey(key);
            }
            finally
            {
                rwLock.ReleaseReaderLock();
            }
            return result;
        }

        public int Count => dictionary.Count;
    }
}
