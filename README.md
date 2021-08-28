# StashCache

**StashCache** is an in-memory caching library for your .NET application. Under the hood, it uses the `MemoryCache` from `Microsoft.Extensions.Caching.Memory`.

## Why StashCache?
Developers tend to procrastinate caching because oftentimes it is divorced from retrieving of data. StashCache is designed such that retrieving and caching of data is inside the same method block.

## Download
StashCache is available on [nuget](https://www.nuget.org/packages/StashCache/).

## Getting Started
See example in `Sample.AspNetCore` project in this repo.

* Register the extension method for the service in `Startup.ConfigureServices`.
``` csharp
    public void ConfigureServices(IServiceCollection services)
    {
        // Import namespace:
        // using StashCache;
        services.AddStashCache();
    }
```
* Choose cache key generator; or implement one as the need arises.

   `TypeCacheKeyGenerator` can be selected in order to generate cache key which is structured into: **`type/class` + `method` + `additional segments`**.
```csharp
    // From Sample.AspNetCore WeatherForecastService class
    private static readonly ICacheKeyGenerator<TypeCacheKeyGenerator> CacheKeyGenerator = CacheKeyGeneratorFactory.GetCacheKeyGenerator<TypeCacheKeyGenerator>();

    private static readonly TimeSpan DefaultCacheExpiry = TimeSpan.FromHours(1);
```
* Inject `ILocalCache` from classes which retrieves data
```csharp
    private readonly ILocalCache _localCache;
    
    public WeatherForecastService(ILocalCache localCache)
    {
        _localCache = localCache;
    }
```

* Lastly, implement caching in the same method block as the data retrieval.
```csharp
    public async Task<IEnumerable<WeatherForecast>> GetAll(CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeyGenerator.GenerateCacheKey<WeatherForecastService>();
        var cachedValues = await _localCache.GetOrAddAsync(cacheKey, async () =>
        {
            var summaries = await GetSummariesAsyc(); // This can be your database call
            return summaries;
        }, DefaultCacheExpiry, cancellationToken).ConfigureAwait(false);

        return cachedValues;
    }
```
