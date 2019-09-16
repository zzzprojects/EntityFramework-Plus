# Query Cache

> This feature is now available on [Entity Framework Classic - Query Cache](http://entityframework-classic.net/query-cache). Entity Framework Classic is a supported version from the latest EF6 code base. It supports .NET Framework and .NET Core and overcomes some EF limitations by adding tons of must-haves built-in features.

## Introduction

Caching entities or query results to improve an application's performance is a very frequent scenario. Major ORM like NHibernate had this feature for a long time but, unfortunately for Entity Framework users, second level caching is only available through third party libraries.

Caching is very simple to understand, the first time a query is invoked, data are retrieved from the database and stored in the memory before being returned. All future calls will retrieve data from the memory to avoid making additional database round trips which drastically increases an application's performance.

**EF+ Query Cache** opens up all caching features for Entity Framework users.

To use caching, simply append to the query "FromCache" method before using an immediate resolution method like "ToList()" or "FirstOrDefault()".

{% include template-example.html %} 
```csharp

// using Z.EntityFramework.Plus; // Don't forget to include this.
var ctx = new EntitiesContext();

// The first call perform a database round trip
var countries1 = ctx.Countries.FromCache().ToList();

// Subsequent calls will take the value from the memory instead
var countries2 = ctx.Countries.FromCache().ToList();

```
[Try it in EF6](https://dotnetfiddle.net/nx9A2H) | [Try it in EF Core](https://dotnetfiddle.net/EZZkhP)

## EF+ Query Cache

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
[Try it in EF6](https://dotnetfiddle.net/BcScGT) | [Try it in EF Core](https://dotnetfiddle.net/z5gyPI)

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
[Try it in EF6](https://dotnetfiddle.net/Uh9miZ) | [Try it in EF Core](https://dotnetfiddle.net/U9pU1a)

## EF+ Query Cache Query Deferred

Immediate resolution methods like **Count()** and **FirstOrDefault()** cannot be cached since their expected behavior is to cache the count result or only the first item.

{% include template-example.html %} 
```csharp

// Oops! The query is already executed, we cannot cache it.
var count = ctx.Customers.Count();

// Oops! All customers are cached instead of customer count.
var count = ctx.Customers.FromCache().Count();
```
[Try it in EF6](https://dotnetfiddle.net/SOjxeY) | [Try it in EF Core](https://dotnetfiddle.net/3EXZBt)

**EF+ Query Deferred** has been created to resolve this issue, the resolution is now deferred instead of being immediate which lets us cache the expected result.

{% include template-example.html %} 
```csharp
// using Z.EntityFramework.Plus; // Don't forget to include this.
var ctx = new EntitiesContext();

// The count is deferred, it is now cached.
var count = ctx.Customers.DeferredCount().FromCache();

```
[Try it in EF6](https://dotnetfiddle.net/4MUlef) | [Try it in EF Core](https://dotnetfiddle.net/V0G6pe)

Query Deferred supports all Queryable extension methods and overloads.

## EF+ Query Cache Tag & ExpireTag

Tagging the cache lets you expire later on all cached entries with a specific tag by calling the **ExpireTag** method.

For example, the daily countries & states importation has been completed and you need to refresh from the database all queries related to the country table. You can now simply expire the tag **countries** to remove all related cached entries.

{% include template-example.html %} 
```csharp

// using Z.EntityFramework.Plus; // Don't forget to include this.
var ctx = new EntitiesContext();

// Cache queries with related tags
var states = ctx.States.FromCache("countries", "states");
var stateCount = ctx.States.DeferredCount().FromCache("countries", "states", "stats");

// Expire all cache entries using the "countries" tag
QueryCacheManager.ExpireTag("countries");
:bulb: Use Enum instead of hard-coding string for tag name.

```
[Try it in EF6](https://dotnetfiddle.net/RScINk) | [Try it in EF Core](https://dotnetfiddle.net/PUQCCY)

## EF+ Query Cache Expiration

All common caching features like absolute expiration, sliding expiration, removed callback are supported.

{% include template-example.html %} 
```csharp

// using Z.EntityFramework.Plus; // Don't forget to include this.
var ctx = new EntitiesContext();

// Make the query expire after 2 hours of inactivity

// (EF5 | EF6)
var options = new CacheItemPolicy() { SlidingExpiration = TimeSpan.FromHours(2)};
// (EF7)
var options = new MemoryCacheEntryOptions() { SlidingExpiration = TimeSpan.FromHours(2)};

var states = ctx.States.FromCache(options);

```
[Try it in EF6](https://dotnetfiddle.net/f57ZoI)

## EF Query Cache Control

EF+ Cache is very flexible and lets you have full control over the cache.

You can use your own cache:

{% include template-example.html %} 
```csharp

// EF5 | EF6 (Cache must inherit from Sytem.Runtime.Caching.ObjectCache)
QueryCacheManager.Cache = MemoryCache.Default;

// EF7 (Cache must inherit from Microsoft.Extensions.Caching.Memory.IMemoryCache)
QueryCacheManager.Cache = new MemoryCache(new MemoryCacheOptions());

```
[Try it in EF6](https://dotnetfiddle.net/mIhcff) | [Try it in EF Core](https://dotnetfiddle.net/6ISVBT)

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

## Real Life Scenarios

 - Caching read-only data like application configuration.
 - Caching data that can only be modified via an importation like states & countries and make all cache entries related expired once the importation is completed (Tag Expiration).
 - Caching data that are frequently accessed but rarely modified like comments and expire the tag when a new comment is added or after one hour without any access (Tag Expiration & Sliding Expiration).
 - Caching statistics that don't require to be live like client count and expire them every hour (Absolute Expiration).

## Behind the code

When **FromCache** is invoked, The QueryCacheManager returns the result if a cache entry exists for the cache key. If no cache entry exists, the query is materialized then cached in the memory using the cache key before being returned. If a cache tag is specified, the cache key is also stored in a concurrent dictionary for all tags.

 - **The Cache:** The memory cache is used by default.

 - **The Cache Key:** The cache key is created by combining a cache prefix, all cache tags and the query expression.

 - **The Query Materialized:** The query is materialized by either using "ToList()" method or "Execute()" method for query deferred.

## Limitations

 - Entity Framework Core:
   - **DO NOT** support Cache Query Deferred (May be available when EF Core will be more stable)

## Requirements

 - **EF+ Query Cache:** Full version or Standalone version
 - **Database Provider:** All supported
 - **Entity Framework Version:** EF5, EF6, EF Core
 - **Minimum Framework Version:** .NET Framework 4

## Conclusion

As we have seen, EF+ Query Cache follows a good architecture principle:

 - **Flexible:** Tag & Cache Options make it possible to use Query Cache in a various number of scenarios.
 - **Extensible:** If there is something missing, the library creates your own extension method or your own cache to overcome the problem.
 - **Maintainable:** The easy to use API, documentation and available source code allows new developers to quickly understand this feature.
 - **Scalable:** Caching gets only better as the number of user/traffic grows by drastically reducing database round trip.

Need help getting started? [info@zzzprojects.com](mailto:info@zzzprojects.com)

We welcome all comments, ideas and suggestions to improve our library.
