---
Permalink: ef6-query-cache
---

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
