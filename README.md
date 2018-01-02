# TinyCache
Small helper for offline and caching of long-running processes
## Example


```csharp
// Create a cache storage, in memory cache will be the default.
var store = new XamarinPropertyStorage();

// Set cache storage
TinyCache.TinyCache.SetCacheStore(store);

// Fetch data with default policy
var result = await TinyCache.TinyCache.UsePolicy<List<Data>>("cachekey", () => { return api.GetData("customdata"); });

```
## Some extra examples
Not needed, but nice to have
```csharp

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

```
## Some extra for preloading cache
Not needed, but nice to have
```csharp

// Get all cached data from storage (to be saved in static file in project and then loaded)
var preloadString = store.GetAllAsLoadableString();

// Preload cache if needed from stored string:
store.LoadFromString(CacheResources.PreloadData.JsonData);
```
