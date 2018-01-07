using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using gymlocator.Core.Shopping;
using gymlocator.Core.Shopping.Models;
using gymlocator.Rest;
using gymlocator.Rest.Models;
using Microsoft.Rest;
using TinyCache;

namespace gymlocator.Core
{
    public class NoClientCredentials : ServiceClientCredentials
    {

    }

    public class ShoppingService
    {
        private IShoppingAPI _client;

        public ShoppingService()
        {
            //_client = new ShoppingAPI(new Uri("http://localhost:5000"), new UnsafeCredentials(), new TinyCache.TinyCacheDelegationHandler());
            _client = new ShoppingAPI(new Uri("http://localhost:5000"), new NoClientCredentials());
        }

        public async Task<IList<ShoppingList>> GetShoppingLists()
        {
            var data = await TinyCache.TinyCache.RunAsync<IList<ShoppingList>>("shoppingLists1", async () => {
                var ret = await _client.GetShoppingListsAsync();
                return ret;
            });
            return data;
        }

        public async Task AddItem(Item item)
        {
            await _client.AddListItemAsync(item);
        }

        public async Task AddList(ShoppingList item)
        {
            await _client.AddShoppingListAsync(item);
        }

        public async Task<IList<Item>> GetListItems(int listId)
        {
            var data = await TinyCache.TinyCache.RunAsync("listItems" + listId, async () => {
                var ret = await _client.GetListItemsAsync(listId);
                return ret;
            });
            return data;
        }

        public async Task UpdateList(ShoppingList shoppingList)
        {
            await _client.UpdateShoppingListAsync(shoppingList.Id, shoppingList);
        }
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
            //var result = await api.GetGymsAsync(locale) as IList<Gym>;
            var result = await TinyCache.TinyCache.RunAsync<List<Gym>>("gyms", () => { return api.GetGymListAsync(locale); });
            return result;
        }
    }
}
