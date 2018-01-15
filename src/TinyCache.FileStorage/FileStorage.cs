using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace TinyCacheLib.FileStorage
{
    public class FileStorage : ICacheStorage
    {
        private string cacheFolder;

        public void Initialize(string cacheFolder)
        {
            this.cacheFolder = cacheFolder;
        }

        public object Get(string key, Type t)
        {
            var path = GetPath(key);

            if(File.Exists(path))
            {
                var json = File.ReadAllText(path);

                var result = JsonConvert.DeserializeObject(json, t);

                return result;
            }

            return null;
        }

        public void Remove(string key)
        {
            var path = GetPath(key);

            if(File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public bool Store(string key, object value)
        {
            var hasChanged = true;

            var path = GetPath(key);

            var json = JsonConvert.SerializeObject(value);

            if(File.Exists(path))
            {
                var content = File.ReadAllText(path);

                if(content == json)
                {
                    hasChanged = false;
                }
            }

            if (hasChanged)
            {
 
                File.WriteAllText(path, json, Encoding.UTF8);
            }

            return hasChanged;
        }

        private string GetPath(string key)
        {
            var encoded = WebUtility.UrlEncode(key);

            return string.Format("TinyCache_{0}.cache", encoded);
        }
    }
}
