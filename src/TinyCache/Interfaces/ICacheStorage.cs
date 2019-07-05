using System;

namespace TinyCacheLib
{
    public interface ICacheStorage
    {
        bool Store(string key, object value, bool checkChange = true);
        object Get(string key, Type t);
        void Remove(string key);
    }
}
