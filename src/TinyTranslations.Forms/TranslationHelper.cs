using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using TinyTranslations;
using TinyWebSockets;
using TinyWebSockets.Interfaces;
using TranslationMessages.SocketMessages;
using Xamarin.Forms;

namespace TinyTranslations.Forms
{
    public class TranslationHelper : INotifyPropertyChanged, IMessageReceiver
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

        private Uri WsUrl { get; }

        public TranslationHelper(Uri serverUri, Uri wsUrl) : this(serverUri)
        {
            WsUrl = wsUrl;
            socket = new WebSocketClientService();

            msgHandler = new MessageHandler(socket);

            msgHandler.RegisterActionReceiver(this);

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
        private readonly MessageHandler msgHandler;
        private readonly WebSocketClientService socket;

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

        public bool IsActive => throw new NotImplementedException();

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
            SendLanguageChange();
            var newdict = await FetchLanguageMethod(locale);
            newdict.OnAdd += TranslationAdded;
            if (_translations != null)
                _translations.OnAdd -= TranslationAdded;
            _translations = newdict;
            propertyChanged(nameof(Translations));
        }

        void TranslationAdded(object sender, System.Collections.Generic.KeyValuePair<string, string> e)
        {
            Task.Run(() =>
            {
                httpClient.AddTranslation(e);
                msgHandler.SendMessage(new TranslationAdded()
                {
                    Key = e.Key,
                    Word = e.Value
                });
            });
        }

        public void Init(string locale = "")
        {
            if (string.IsNullOrEmpty(locale))
                locale = DeviceSpecificLanguage;
            Task.Run(() => ChangeCultureAsync(locale).RunSynchronously());
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Translate(string key)
        {
            return Translations[key];
        }

        public void SetStateService(MessageHandler stateService)
        {
            Task.Run(async () =>
            {
                await socket.StartListening(WsUrl);
                await socket.StartReceivingMessages();
                SendLanguageChange();
            });
        }

        private void SendLanguageChange()
        {
            msgHandler.SendMessage(new LanguageChange()
            {
                Locale = CurrentLocale
            });
        }

        public void HandleAction(IMessage action)
        {
            switch (action)
            {
                case TranslationAdded added:
                    Translations.Add(added.Key, added.Word);
                    break;
                case LanguageChanged langchange:

                    break;
                case TranslationUpdated updated:
                    Translations.Add(updated.Key, updated.Word);
                    break;
            }
        }
    }
}
