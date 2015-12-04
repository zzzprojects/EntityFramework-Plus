##EF Bulk Operations & Utilities | Bulk Insert, Update, Delete, Merge from database##

## Download
Version | NuGet | NuGet Install
------------ | :-------------: | :-------------:
Z.EntityFramework.Plus.EF7 | <a href="https://www.nuget.org/packages/EntityFramework.Plus.EF7/" target="_blank" alt="download nuget"><img src="https://img.shields.io/nuget/v/EntityFramework.Plus.EF7.svg?style=flat-square" /></a> <a href="https://www.nuget.org/packages/EntityFramework.Plus.EF7/" target="_blank" alt="download nuget"><img src="https://img.shields.io/nuget/dt/EntityFramework.Plus.EF7.svg?style=flat-square" /></a> | ```PM> Install-Package EntityFramework.Plus.EF7```
Z.EntityFramework.Plus.EF6 | <a href="https://www.nuget.org/packages/EntityFramework.Plus.EF6/" target="_blank" alt="download nuget"><img src="https://img.shields.io/nuget/v/EntityFramework.Plus.EF6.svg?style=flat-square" /></a> <a href="https://www.nuget.org/packages/EntityFramework.Plus.EF6/" target="_blank" alt="download nuget"><img src="https://img.shields.io/nuget/dt/EntityFramework.Plus.EF6.svg?style=flat-square" /></a> | ```PM> Install-Package EntityFramework.Plus.EF6```
Z.EntityFramework.Plus.EF5 | <a href="https://www.nuget.org/packages/EntityFramework.Plus.EF5/" target="_blank" alt="download nuget"><img src="https://img.shields.io/nuget/v/EntityFramework.Plus.EF5.svg?style=flat-square" /></a> <a href="https://www.nuget.org/packages/EntityFramework.Plus.EF5/" target="_blank" alt="download nuget"><img src="https://img.shields.io/nuget/dt/EntityFramework.Plus.EF5.svg?style=flat-square" /></a> | ```PM> Install-Package EntityFramework.Plus.EF5```

## Features
- BulkSaveChanges _(under development)_
- Bulk Operations
    - Bulk Insert _(under development)_
    - Bulk Update _(under development)_
    - Bulk Delete _(under development)_
    - Bulk Merge _(under development)_
- Query
    - Query Batch Operations _(under development)_
    - [Query Cache (Second Level Cache)](https://github.com/zzzprojects/Entity-Framework-Plus/wiki/Query-Cache)
    - Query Future _(under development)_
- Auditing _(under development)_

## Query Cache
**Query cache is the second level cache for Entity Framework.**

The result of the query is returned from the cache. If the query is not cached yet, the query is materialized and cached before being returned.

**Support:**

_Cache Policy_

```csharp
// The query is cached using default QueryCacheManager options
var countries = ctx.Countries.Where(x => x.IsActive).FromCache();

// (EF5 | EF6) The query is cached for 2 hours
var states = ctx.States.Where(x => x.IsActive).FromCache(DateTime.Now.AddHours(2));

// (EF7) The query is cached for 2 hours without any activity
var options = new MemoryCacheEntryOptions() { SlidingExpiration = TimeSpan.FromHours(2)};
var states = ctx.States.Where(x => x.IsActive).FromCache(options).Count();
```

_Tags_
```csharp
var countries = db.Countries.Where(x => x.IsActive).FromCache("states", "countries");

// All cached query using the "states" tag are expired
QueryCacheManager.ExpireTag("states");
```

**[Learn more](https://github.com/zzzprojects/Entity-Framework-Plus/wiki/Query-Cache)**

## FREE vs PRO
Every month, a new monthly trial of the PRO Version is available to let you evaluate all its features without limitations.

Features | FREE Version | PRO Version
------------ | :-------------: | :-------------:
Query Cache | Yes | Yes
Commercial License | No | Yes
Royalty-Free | No | Yes
Support & Upgrades (1 year) | No | Yes
Learn more about the **(Not available until 2016)**

## Support
Contact our outstanding customer support for any request. We usually answer within the next business day, hour, or minutes!

- Website (Not yet available)
- [Documentation](https://github.com/zzzprojects/Entity-Framework-Plus/wiki)
- [Forum](https://zzzprojects.uservoice.com/forums/283924-entity-framework-plus)
- sales@zzzprojects.com

## More Projects
  - [NET Entity Framework Extensions](http://www.zzzprojects.com/products/dotnet-development/entity-framework-extensions/)
  - [NET Bulk Operations](http://www.zzzprojects.com/products/dotnet-development/bulk-operations/)
  - [Eval Expression.NET](https://github.com/zzzprojects/Eval-Expression.NET)
  - [Eval SQL.NET](https://github.com/zzzprojects/Eval-SQL.NET)
  - [Extension Methods Library](https://github.com/zzzprojects/Z.ExtensionMethods/)

