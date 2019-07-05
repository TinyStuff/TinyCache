using System;
using System.Collections.Generic;

namespace TinyCacheLib
{
    public class MemoryDictionaryCache : ICacheStorage
    {
        private Dictionary<string, object> cacheObj = new Dictionary<string, object>();

        public object Get(string key, Type t)
        {
            if (cacheObj.ContainsKey(key))
            {
                return cacheObj[key];
            }

            return null;
        }

        public void Remove(string key)
        {
            if (cacheObj.ContainsKey(key))
            {
                cacheObj.Remove(key);
            }
        }

        public bool Store(string key, object value, bool checkChange = true)
        {
            var hasChanged = false;

            if (checkChange && cacheObj.ContainsKey(key))
            {
                hasChanged = cacheObj[key] != value;
                cacheObj[key] = value;
            }
            else
            {
                cacheObj.Add(key, value);
                hasChanged = true;
            }

            return hasChanged;
        }
    }
}
