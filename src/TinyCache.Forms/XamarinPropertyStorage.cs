using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace TinyCache
{
    public class XamarinPropertyStorage : IPreloadableCache
    {
        public object Get(string key, Type t)
        {
            if (Application.Current.Properties != null && Application.Current.Properties.ContainsKey(key))
            {
                var stringValue = Application.Current.Properties[key] as string;
                return JsonConvert.DeserializeObject(stringValue, t);
            }

            return null;
        }

        public bool Store(string key, object value)
        {
            var ret = true;

            if (value == null)
            {
                ret = false;
            }

            var stringValue = JsonConvert.SerializeObject(value);

            if (Application.Current.Properties != null && !string.IsNullOrEmpty(stringValue))
            {
                if (Application.Current.Properties.ContainsKey(key))
                {
                    if (Application.Current.Properties[key] as string != stringValue)
                    {
                        Application.Current.Properties[key] = stringValue;
                        ret = false;
                    }
                }
                else
                {
                    Application.Current.Properties.Add(key, stringValue);
                    ret = true;
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
            if (Application.Current.Properties != null && Application.Current.Properties.ContainsKey(key))
            {
                Application.Current.Properties.Remove(key);
            }
        }

        public string GetAllAsLoadableString()
        {
            return JsonConvert.SerializeObject(Application.Current.Properties);
        }

        public void LoadFromString(string fillData)
        {
            var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(fillData);

            foreach (var key in dict.Keys)
            {
                if (!Application.Current.Properties.ContainsKey(key))
                {
					Application.Current.Properties.Add(key,dict[key]);
                }
            }
        }
    }
}
