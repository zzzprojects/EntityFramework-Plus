---
Permalink: ef6-query-cache-control
---

# EF Query Cache Control

EF+ Cache is very flexible and lets you have full control over the cache.

You can use your own cache:

{% include template-example.html %} 
```csharp

// EF5 | EF6 (Cache must inherit from Sytem.Runtime.Caching.ObjectCache)
QueryCacheManager.Cache = MemoryCache.Default;


```
[Try it](https://dotnetfiddle.net/mIhcff)

You can set default policy

{% include template-example.html %} 
```csharp

// (EF5 | EF6) The query is cached for 2 hours of inactivity
var options = new CacheItemPolicy() { SlidingExpiration = TimeSpan.FromHours(2)};
QueryCacheManager.DefaultCacheItemPolicy = options;

// (EF7) The query is cached for 2 hours of inactivity
var options = new MemoryCacheEntryOptions() { SlidingExpiration = TimeSpan.FromHours(2)};
QueryCacheManager.DefaultMemoryCacheEntryOptions = options;

```
[Try it in EF6](https://dotnetfiddle.net/7JptcT)
