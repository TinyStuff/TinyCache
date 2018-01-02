# TinyCache
Small helper for offline and caching of long-running processes
## Example


```csharp
// Create a cache storage, in memory cache will be the default.
var store = new XamarinPropertyStorage();

// Fetch data with default policy
var result = await TinyCache.TinyCache.UsePolicy<List<Data>>("cachekey", () => { return api.GetData("customdata"); });

// Set cache storage
TinyCache.TinyCache.SetCacheStore(store);

// Preload cache if needed:
store.LoadFromString(CacheResources.PreloadData.JsonData);

// Handle errors
TinyCache.TinyCache.OnError += (sender, e) =>
{
    ShowError(e);
};

// Set a base policy that will be used when no policy is specified
TinyCache.TinyCache.SetBasePolicy(
    new TinyCachePolicy()
        .SetMode(TinyCacheModeEnum.CacheFirst)
        .SetFetchTimeout(6000));
        
// Handle background changes
TinyCache.TinyCache.OnUpdate += async (object sender, CacheUpdatedEvt e) => {
    var cacheKey = e.Key;
    var dataObject = e.Value;
    async HandleObjectChange(cacheKey,dataObject as MyDataType);
};
