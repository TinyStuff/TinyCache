using System;

namespace TinyCache
{
    public interface ICacheStorage
    {
        bool Store(string key, object value);
        object Get(string key, Type t);
        void Remove(string key);
    }
}
