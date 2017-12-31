using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using TinyTranslations;
using Xamarin.Forms;

namespace TinyTranslations.Forms
{
    public class TranslationHelper : INotifyPropertyChanged
    {
        public TranslationHelper()
        {

        }

        public TranslationHelper(Uri serverUri)
        {
            this.serverUri = serverUri;
            httpClient = new TranslationClient(serverUri);
            FetchLanguageMethod = (locale) =>
            {
                return httpClient.GetTranslations(locale);
            };
        }

        public static readonly string DeviceSpecificLanguage =
            System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

        public string Translate(string key, string text)
        {
            if (Translations.ContainsKey(key))
            {
                return Translations[key];
            }
            else
            {
                Translations.Add(key, text);
                return text;
            }
        }

        private TranslationClient httpClient;
        private Uri serverUri;
        string currentLocale = DeviceSpecificLanguage;

        public string CurrentLocale
        {
            get
            {
                return currentLocale;
            }
            set
            {
                if (currentLocale != value)
                {
                    changeCulture(value);
                    propertyChanged(nameof(CurrentLocale));
                }
            }
        }

        private TranslationDictionary _translations;
        public TranslationDictionary Translations
        {
            get
            {
                if (_translations == null)
                {
                    _translations = new TranslationDictionary(currentLocale);
                    changeCulture(currentLocale);
                }
                return _translations;
            }
        }

        public virtual Func<string, Task<TranslationDictionary>> FetchLanguageMethod { get; set; }

        private void propertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void changeCulture(string locale)
        {
            currentLocale = locale;
            Task.Run(async () =>
            {
                var newdict = await FetchLanguageMethod(locale);
                newdict.OnAdd += TranslationAdded;
                if (_translations != null)
                    _translations.OnAdd -= TranslationAdded;
                _translations = newdict;
                propertyChanged(nameof(Translations));
            });
        }

        public async Task ChangeCultureAsync(string locale)
        {
            currentLocale = locale;
            var newdict = await FetchLanguageMethod(locale);
            newdict.OnAdd += TranslationAdded;
            if (_translations != null)
                _translations.OnAdd -= TranslationAdded;
            _translations = newdict;
            propertyChanged(nameof(Translations));
        }

        void TranslationAdded(object sender, System.Collections.Generic.KeyValuePair<string, string> e)
        {
            Task.Run(() => httpClient.AddTranslation(e));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Translate(string key)
        {
            return Translations[key];
        }
    }
}
