---
Permalink: ef-core-query-cache-control
---

# EF Query Cache Control

EF+ Cache is very flexible and lets you have full control over the cache.

You can use your own cache:

{% include template-example.html %} 
```csharp
QueryCacheManager.Cache = new MemoryCache(new MemoryCacheOptions());
context.Customers.FromCache().ToList();

```
[Try it](https://dotnetfiddle.net/6ISVBT)

You can set default policy

{% include template-example.html %} 
```csharp

var options = new MemoryCacheEntryOptions() { SlidingExpiration = TimeSpan.FromHours(2)};
QueryCacheManager.DefaultMemoryCacheEntryOptions = options;
context.Customers.FromCache().ToList();

```
[Try it](https://dotnetfiddle.net/k1TOWX)
