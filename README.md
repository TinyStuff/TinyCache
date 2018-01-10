# TinyCache
Small helper for offline and caching of long-running processes
## Example

![buildstatus](https://io2gamelabs.visualstudio.com/_apis/public/build/definitions/be16d002-5786-41a1-bf3b-3e13d5e80aa0/14/badge)

### Use file storage
Install NuGet package TinyCache.FileStorage.

```csharp
// Create a cache storage, in memory cache will be the default.
var store = new FileStorage();

var cacheFolder = string.Empty;
            
#if __IOS__ || __MACOS__
            cacheFolder = NSSearchPath.GetDirectories(NSSearchPathDirectory.CachesDirectory, NSSearchPathDomain.User)[0];
#elif __ANDROID__
            cacheFolder = Application.Context.CacheDir.AbsolutePath;
#elif __UWP__
            cacheFolder = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
#else
            cacheFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
#endif

store.Initialize(cacheFolder);

// Set cache storage
TinyCache.TinyCache.SetCacheStore(store);

// Fetch data with default policy
var result = await TinyCache.TinyCache.RunAsync<List<Data>>("cachekey", () => { return api.GetData("customdata"); });


### Use property storage in Xamarin.Forms
Install NuGet package TinyCache.Forms.

```csharp
// Create a cache storage, in memory cache will be the default.
var store = new XamarinPropertyStorage();

// Set cache storage
TinyCache.TinyCache.SetCacheStore(store);

// Fetch data with default policy
var result = await TinyCache.TinyCache.RunAsync<List<Data>>("cachekey", () => { return api.GetData("customdata"); });

```
## Caching DelegationHandler
```csharp
AuoRestApi api = new AuoRestApi(apiEndPoint, new NoClientCredentials(), new TinyCache.TinyCacheDelegationHandler());
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
        .SetMode(TinyCacheModeEnum.CacheFirst) // fetch from cache first
        .SetFetchTimeout(TimeSpan.FromSeconds(5)) // 5 second excecution limit
        .SetExpirationTime(TimeSpan.FromMinutes(10)) // 10 minute expiration before next fetch
        .SetUpdateCacheTimeout(50) // Wait 50ms before trying to update cache in background
        .UpdateHandler = async (key, newdata) => { await DoStuff(key, newdata); }); // Handle background updates 

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
