﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace TinyCacheLib
{
    public class XamarinPropertyStorage : IPreloadableCache
    {
        private IDictionary<string, object> AppStorage
        {
            get
            {
                return Application.Current.Properties;
            }
        }

        public object Get(string key, Type t)
        {
            if (AppStorage != null && AppStorage.ContainsKey(key))
            {
                var stringValue = AppStorage[key] as string;
                return JsonConvert.DeserializeObject(stringValue, t);
            }

            return null;
        }

        private Object thisLock = new Object(); 

        public bool Store(string key, object value)
        {
            var ret = true;

            if (value == null)
            {
                ret = false;
            }

            var stringValue = JsonConvert.SerializeObject(value);

            if (AppStorage != null && !string.IsNullOrEmpty(stringValue))
            {
                lock (thisLock)
                {
                    if (AppStorage.ContainsKey(key))
                    {
                        if (AppStorage[key] as string != stringValue)
                        {
                            AppStorage[key] = stringValue;
                            ret = false;
                        }
                    }
                    else
                    {
                        AppStorage.Add(key, stringValue);
                        ret = true;
                    }
                }
            }

            if (ret)
            {
                Application.Current.SavePropertiesAsync();
            }

            return ret;
        }

        public void Remove(string key)
        {
            if (AppStorage != null && AppStorage.ContainsKey(key))
            {
                AppStorage.Remove(key);
            }
        }

        public string GetAllAsLoadableString()
        {
            return JsonConvert.SerializeObject(AppStorage);
        }

        public void LoadFromString(string fillData)
        {
            var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(fillData);

            foreach (var key in dict.Keys)
            {
                if (!AppStorage.ContainsKey(key))
                {
                    AppStorage.Add(key, dict[key]);
                }
            }
        }
    }
}
