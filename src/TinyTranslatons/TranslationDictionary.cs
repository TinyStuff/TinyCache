using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace TinyTranslations
{
    public class NotTranslatedWord
    {
        public NotTranslatedWord(string key)
        {
            Original = key;
            UsageCount = 1;
            Translation = Original;
        }

        public string Original { get; set; }
        public string Translation { get; set; }
        public int UsageCount { get; set; }
    }

    public class TranslationDictionary : IDictionary<string, string>, INotifyCollectionChanged
    {
        public TranslationDictionary(string locale)
        {
            Locale = locale;
        }

        private Dictionary<string, string> dict = new Dictionary<string, string>();
        private Dictionary<string, NotTranslatedWord> newWords = new Dictionary<string, NotTranslatedWord>();

        public static bool AddNewWordsToDictionary { get; set; } = true;

        public EventHandler<KeyValuePair<string, string>> OnAdd;
        public EventHandler<KeyValuePair<string, string>> OnUpdate;

        public string Locale { get; internal set; }

        public string this[string key]
        {
            get
            {
                if (dict.ContainsKey(key))
                    return dict[key];
                return AddNotFound(key);
            }
            set
            {
                if (dict.ContainsKey(key))
                {
                    dict[key] = value;
                }
                Add(key, value);
            }
        }

        public void Populate(string locale, Dictionary<string, string> data)
        {
            Locale = locale;
            foreach (var kv in data)
            {
                Add(kv);
            }
        }

        public Dictionary<string, string> GetAllTranslations() => dict;

        private KeyValuePair<string, string> GetKV(string key, string value = null)
        {
            return new KeyValuePair<string, string>(key, value ?? dict[key]);
        }

        private void ChangeKeyValue(string key, string newValue)
        {
            //KeyValuePair<string, string> old = KeyValuePair;
            var oldKv = GetKV(key);
            dict[key] = newValue;
            var newKv = GetKV(key, newValue);
            OnUpdate?.Invoke(this, newKv);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, oldKv, newKv));
        }

        private void AddValue(string key, string value)
        {
            if (dict.ContainsKey(key))
            {
                ChangeKeyValue(key, value);
            }
            else
            {
                dict.Add(key, value);
                var newKv = GetKV(key, value);
                OnAdd?.Invoke(this, newKv);
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newKv));
            }
        }

        private bool RemoveKey(string key)
        {
            if (dict.ContainsKey(key))
            {
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, GetKV(key)));
                dict.Remove(key);
                return true;
            }
            return false;
        }

        private string AddNotFound(string key)
        {
            if (newWords.ContainsKey(key))
            {
                var word = newWords[key];
                word.UsageCount++;
                return word.Translation;
            }
            var newword = new NotTranslatedWord(key);
            newWords.Add(key, newword);
            if (AddNewWordsToDictionary)
            {
                Add(key, newword.Translation);

            }
            return newword.Translation;

        }

        public ICollection<string> Keys => dict.Keys;

        public ICollection<string> Values => dict.Values;

        public int Count => dict.Count;

        public bool IsReadOnly => false;

        public bool IsPrimaryLanguage { get; internal set; }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void Add(string key, string value)
        {
            if (dict.ContainsKey(key))
            {
                ChangeKeyValue(key, value);
            }
            else
            {
                AddValue(key, value);
            }
        }

        public void Add(KeyValuePair<string, string> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            dict.Clear();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public bool Contains(KeyValuePair<string, string> item) => dict.ContainsKey(item.Key);

        public bool ContainsKey(string key) => dict.ContainsKey(key);

        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            foreach (var kv in array)
            {
                Add(kv.Key, kv.Value);
            }
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => dict.GetEnumerator();

        public bool Remove(string key)
        {
            return RemoveKey(key);
        }

        public bool Remove(KeyValuePair<string, string> item)
        {
            return Remove(item.Key);
        }

        public bool TryGetValue(string key, out string value)
        {
            return TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator() => dict.GetEnumerator();

    }
}
