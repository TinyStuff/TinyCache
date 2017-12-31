using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TinyTranslations;

namespace TinyTranslations
{
    public class TranslationClient
    {
        public TranslationClient(Uri baseUrl)
        {
            baseUri = baseUrl;
        }

        private Uri baseUri;

        private HttpClient _client;
        public virtual HttpClient Client
        {
            get
            {
                if (_client == null)
                    _client = new HttpClient();
                return _client;
            }
            set
            {
                _client = value;
            }
        }

        public virtual async Task<TranslationDictionary> GetTranslations(string locale)
        {
            var ret = new TranslationDictionary(locale);
            var translationString = await Client.GetStringAsync(baseUri.AbsoluteUri + "api/translation/" + locale);
            var dict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(translationString);
            ret.Populate(locale, dict);
            return ret;
        }

        public async Task<string> AddTranslation(KeyValuePair<string, string> e)
        {
            var baseUrl = baseUri.AbsoluteUri + "api/translation/default/" + System.Net.WebUtility.UrlEncode(e.Key);
            if (e.Key.Equals(e.Value))
                return await Client.GetStringAsync(baseUrl);
            else
            {
                var ret = await Client.PutAsync(baseUrl + "/" + System.Net.WebUtility.UrlEncode(e.Value), null);
                return e.Value;
            }
        }
    }
}
