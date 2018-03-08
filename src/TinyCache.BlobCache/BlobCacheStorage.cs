using System;
using Akavache;

namespace TinyCacheLib.BlobCache
{
    public class BlobCacheStorage : ICacheStorage
    {
        private IBlobCache _internalCache;

        public BlobCacheStorage(IBlobCache storage)
        {
            _internalCache = storage;
        }

        public object Get(string key, Type t)
        {
            return _internalCache.GetObject<object>(key);
        }

        public void Remove(string key)
        {
            _internalCache.Invalidate(key);
        }

        public bool Store(string key, object value)
        {
            _internalCache.InsertObject(key, value, TimeSpan.FromDays(200));
            return true;
        }
    }
}
