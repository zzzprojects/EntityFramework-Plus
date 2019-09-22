---
Permalink: ef-core-query-cache
---

# Query Cache

> This feature is now available on [Entity Framework Classic - Query Cache](http://entityframework-classic.net/query-cache). Entity Framework Classic is a supported version from the latest EF6 code base. It supports .NET Framework and .NET Core and overcomes some EF limitations by adding tons of must-haves built-in features.

## Introduction

Caching entities or query results to improve an application's performance is a very frequent scenario. Major ORM like NHibernate had this feature for a long time but, unfortunately for Entity Framework Core users, second level caching is only available through third party libraries.

Caching is very simple to understand, the first time a query is invoked, data are retrieved from the database and stored in the memory before being returned. All future calls will retrieve data from the memory to avoid making additional database round trips which drastically increases an application's performance.

**EF+ Query Cache** opens up all caching features for Entity Framework Core users.

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
[Try it](https://dotnetfiddle.net/EZZkhP)

## Scenarios

 - [Query Criteria](scenarios/ef-core-query-cache-using-query-criteria.md)
 - [Query Deferred](scenarios/ef-core-query-cache-query-deferred.md)
 - [Tag & ExpireTag](scenarios/ef-core-query-cache-tag.md)
 - [Expiration](scenarios/ef-core-query-cache-expiration.md)
 - [Query Cache Control](scenarios/ef-core-query-cache-control.md)
 
## Real Life Scenarios

 - Caching read-only data like application configuration.
 - Caching data that cQuery Cache Controlan only be modified via an importation like states & countries and make all cache entries related expired once the importation is completed (Tag Expiration).
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
 - **Entity Framework Version:** EF Core
 - **Minimum Framework Version:** .NET Framework 4

## Conclusion

As we have seen, EF+ Query Cache follows a good architecture principle:

 - **Flexible:** Tag & Cache Options make it possible to use Query Cache in a various number of scenarios.
 - **Extensible:** If there is something missing, the library creates your own extension method or your own cache to overcome the problem.
 - **Maintainable:** The easy to use API, documentation and available source code allows new developers to quickly understand this feature.
 - **Scalable:** Caching gets only better as the number of user/traffic grows by drastically reducing database round trip.

Need help getting started? [info@zzzprojects.com](mailto:info@zzzprojects.com)

We welcome all comments, ideas and suggestions to improve our library.
