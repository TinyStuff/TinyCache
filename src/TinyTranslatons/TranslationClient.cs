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

        public HttpMessageHandler MessageHandler { get; set; }

        public virtual HttpClient GetHttpClient()
        {
            if (_client == null)
            {
                if (MessageHandler==null) {
                    _client = new HttpClient();
                }
                else {
                    _client = new HttpClient(MessageHandler);
                }
            }
            return _client;
        }

        public virtual async Task<TranslationDictionary> GetTranslations(string locale)
        {
            var ret = new TranslationDictionary(locale);
            var translationString = await GetHttpClient().GetStringAsync(baseUri.AbsoluteUri + "api/translation/" + locale);
            var dict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(translationString);
            ret.Populate(locale, dict);
            return ret;
        }

        private string GetUrl(string value) {
            var ret = System.Net.WebUtility.UrlEncode(value);
            return ret.Replace("+","%20");
        }

        public async Task<string> AddTranslationAsync(KeyValuePair<string, string> e)
        {
            var baseUrl = baseUri.AbsoluteUri + "api/translation/default/" + GetUrl(e.Key);
            if (e.Key.Equals(e.Value))
                return await GetHttpClient().GetStringAsync(baseUrl);
            else
            {
                var ret = await GetHttpClient().PutAsync(baseUrl + "/" + GetUrl(e.Value), null);
                return e.Value;
            }
        }
    }
}
