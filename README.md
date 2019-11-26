# TinyCache
Small helper for offline and caching of long-running processes

## Build status
![buildstatus](https://io2gamelabs.visualstudio.com/_apis/public/build/definitions/be16d002-5786-41a1-bf3b-3e13d5e80aa0/14/badge)

## Breaking changes in version 2.0
Version 2.0.x will introduce breaking changes to how to use TinyCache. Pre 2.0 we could only have one instance of TinyCache. 2.0 introduces the option to have multiple instances of TinyCache. To make that possible we have introduced TinyCacheHandler.

## TinyCacheHandler
If we only want one instance of TinyCache, we can use the **Default** method of **TinyCacheHandler**. First time you access the Default property a new instance of TinyCache will be created if there are not instances created. This instance will get **default** as the key.

### Create an additional instance of TinyCache
To add a new instance of TinyCacheHandler we can use either the create method or the add method. 


**Using the Create method:**
```csharp
var newCache = TinyCacheHandler.Create("myNewCache");
```

**Using the Add method:**
```csharp
var newCache = new TinyCache();
TinyCacheHandler.Add("myNewCache", newCache);
```
### Set default cache
If we have multiple cache instances, we maybe not want the first one to be default, then we can change that by passing the cahce key to the SetDefault method of TinyCacheHandler.

```csharp
TinyCacheHandler.SetDefault("myNewCache");
```

### Get a specific instance of TinyCache
If we want to get an instance of TinyCache that not are the default instance, we can use the key for the instance as in the code below.
```csharp
var cache = TinyCacheHandler.Get("myNewCache");

var result = cache.RunAsync<List<Data>>("cachekey", () => { return api.GetData("customdata"); });
```

## Example

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
var result = await TinyCacheHandler.Default.RunAsync<List<Data>>("cachekey", () => { return api.GetData("customdata"); });
```

### Use property storage in Xamarin.Forms
Install NuGet package TinyCache.Forms.

```csharp
// Create a cache storage, in memory cache will be the default.
var store = new XamarinPropertyStorage();

// Set cache storage
TinyCacheHandler.Default.SetCacheStore(store);

// Fetch data with default policy
var result = await TinyCacheHandler.Default..RunAsync<List<Data>>("cachekey", () => { return api.GetData("customdata"); });

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
TinyCacheHandler.Default..SetBasePolicy(
    new TinyCachePolicy()
        .SetMode(TinyCacheModeEnum.CacheFirst) // fetch from cache first
        .SetFetchTimeout(TimeSpan.FromSeconds(5)) // 5 second excecution limit
        .SetExpirationTime(TimeSpan.FromMinutes(10)) // 10 minute expiration before next fetch
        .SetUpdateCacheTimeout(50) // Wait 50ms before trying to update cache in background
        .UpdateHandler = async (key, newdata) => { await DoStuff(key, newdata); }); // Handle background updates 

// Handle background changes
TinyCacheHandler.Default.OnUpdate += async (object sender, CacheUpdatedEvt e) => {
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
