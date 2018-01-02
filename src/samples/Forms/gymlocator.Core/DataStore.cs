using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using gymlocator.Rest;
using gymlocator.Rest.Models;
using Microsoft.Rest;
using TinyCache;

namespace gymlocator.Core
{
    public class NoClientCredentials : ServiceClientCredentials
    {

    }

    public class DataStore
    {
        static Uri apiEndPoint = new Uri("http://f24s-gym-api03.azurewebsites.net/v1/");
        GymAPI api = new GymAPI(apiEndPoint, new NoClientCredentials(), new TinyCache.TinyCacheDelegationHandler());
        private string locale = System.Globalization.CultureInfo.CurrentCulture.Name;

        XamarinPropertyStorage store = new XamarinPropertyStorage();

        public DataStore()
        {
            api.BaseUri = apiEndPoint;
            store.LoadFromString(CacheResources.PreloadData.JsonData);

            var preloadString = store.GetAllAsLoadableString();

            TinyCache.TinyCache.SetCacheStore(store);
            TinyCache.TinyCache.OnError += (sender, e) =>
            {
                var i = 3;
            };

            TinyCache.TinyCache.SetBasePolicy(
                new TinyCachePolicy()
                    .SetMode(TinyCacheModeEnum.CacheFirst)
                    .SetFetchTimeout(6000));
        }

        public async Task<IList<Gym>> GetGymsAsync()
        {
            var result = await api.GetGymsAsync(locale) as IList<Gym>;
            //var result = await TinyCache.TinyCache.RunAsync<List<Gym>>("gyms", () => { return api.GetGymListAsync(locale); });
            return result;
        }
    }
}
