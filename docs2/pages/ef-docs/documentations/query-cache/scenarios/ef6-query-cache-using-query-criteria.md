---
Permalink: ef6-query-cache-using-query-criteria
---

# EF+ Query Cache

Return the query result from the cache. If the query is not cached yet, it will be materialized and cached before being returned.

{% include template-example.html %} 
```csharp

// using Z.EntityFramework.Plus; // Don't forget to include this.
var ctx = new EntitiesContext();

// The query is cached using default QueryCacheManager options
var countries = ctx.Countries.Where(x => x.IsActive).FromCache();

// (EF5 | EF6) The query is cached for 2 hours
var states = ctx.States.Where(x => x.IsActive).FromCache(DateTime.Now.AddHours(2));

```
[Try it](https://dotnetfiddle.net/BcScGT)

## EF+ Query Cache Async

Return the query result from the cache. If the query is not cached yet, the query will be materialized asynchronously and cached before being returned.

**FromCacheAsync** methods are available starting from .NET Framework 4.5 and support all the same options as "FromCache" methods.

{% include template-example.html %} 
```csharp

// using Z.EntityFramework.Plus; // Don't forget to include this.
var ctx = new EntitiesContext();

// The query is cached using default QueryCacheManager options
var task = ctx.Countries.Where(x => x.IsActive).FromCacheAsync();

// (EF5 | EF6) The query is cached for 2 hours
var task = ctx.States.Where(x => x.IsActive).FromCacheAsync(DateTime.Now.AddHours(2));

```
[Try it](https://dotnetfiddle.net/Uh9miZ)
