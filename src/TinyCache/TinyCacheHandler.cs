using System;
using System.Collections.Generic;
using System.Linq;

namespace TinyCacheLib
{
    public static class TinyCacheHandler
    {
        private static readonly Dictionary<string, TinyCache> caches = new Dictionary<string, TinyCache>();

        public static TinyCache Create(string key)
        {
            var cache = new TinyCache();

            Add(key, cache);

            return cache;
        }

        public static void Add(string key, TinyCache cache)
        {
            if (caches.ContainsKey(key))
            {
                caches[key] = cache;
            }
            else
            {
                caches.Add(key, cache);
            }

            if (caches.Count == 1)
            {
                DefaultKey = key;
            }
        }

        public static TinyCache Get(string key)
        {
            return caches[key];
        }

        public static void Remove(string key)
        {
            caches.Remove(key);
        }

        public static void SetDefault(string key)
        {
            if (caches.ContainsKey(key))
            {
                DefaultKey = key;
            }
            else
            {
                throw new KeyNotFoundException();
            }
        }

        public static IEnumerable<string> AllKeys => caches.Keys;
        public static string DefaultKey { get; private set; }

        public static TinyCache Default => caches.Any() ? caches[DefaultKey] : Create();

        public static int Count => caches.Count;

        private static TinyCache Create()
        {
            var cache = new TinyCache();

            caches.Add("default", cache);

            return cache;
        }
    }
}
