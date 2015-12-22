This library is under construction. New features are added weekly.

Use [.NET Entity Framework Extensions](http://www.zzzprojects.com/products/dotnet-development/entity-framework-extensions/) for stable version.

##Entity Framework Bulk Operations & Utilities##

## Download
Version | NuGet | NuGet Install
------------ | :-------------: | :-------------:
Z.EntityFramework.Plus.EF7 | <a href="https://www.nuget.org/packages/Z.EntityFramework.Plus.EF7/" target="_blank" alt="download nuget"><img src="https://img.shields.io/nuget/v/Z.EntityFramework.Plus.EF7.svg?style=flat-square" /></a> <a href="https://www.nuget.org/packages/Z.EntityFramework.Plus.EF7/" target="_blank" alt="download nuget"><img src="https://img.shields.io/nuget/dt/Z.EntityFramework.Plus.EF7.svg?style=flat-square" /></a> | ```PM> Install-Package Z.EntityFramework.Plus.EF7```
Z.EntityFramework.Plus.EF6 | <a href="https://www.nuget.org/packages/Z.EntityFramework.Plus.EF6/" target="_blank" alt="download nuget"><img src="https://img.shields.io/nuget/v/Z.EntityFramework.Plus.EF6.svg?style=flat-square" /></a> <a href="https://www.nuget.org/packages/Z.EntityFramework.Plus.EF6/" target="_blank" alt="download nuget"><img src="https://img.shields.io/nuget/dt/Z.EntityFramework.Plus.EF6.svg?style=flat-square" /></a> | ```PM> Install-Package Z.EntityFramework.Plus.EF6```
Z.EntityFramework.Plus.EF5 | <a href="https://www.nuget.org/packages/Z.EntityFramework.Plus.EF5/" target="_blank" alt="download nuget"><img src="https://img.shields.io/nuget/v/Z.EntityFramework.Plus.EF5.svg?style=flat-square" /></a> <a href="https://www.nuget.org/packages/Z.EntityFramework.Plus.EF5/" target="_blank" alt="download nuget"><img src="https://img.shields.io/nuget/dt/Z.EntityFramework.Plus.EF5.svg?style=flat-square" /></a> | ```PM> Install-Package Z.EntityFramework.Plus.EF5```

## Features
- BulkSaveChanges _(under development)_
- Bulk Operations
    - Bulk Insert _(under development)_
    - Bulk Update _(under development)_
    - Bulk Delete _(under development)_
    - Bulk Merge _(under development)_
- Query
    - Query Batch Operations _(under development)_
    - [Query Cache (Second Level Cache)](https://github.com/zzzprojects/EntityFramework-Plus/wiki/Query-Cache)
    - [Query Delayed](https://github.com/zzzprojects/EntityFramework-Plus/wiki/Query-Delayed)
    - [Query Filter](https://github.com/zzzprojects/EntityFramework-Plus/wiki/Query-Filter)    
    - [Query Future](https://github.com/zzzprojects/EntityFramework-Plus/wiki/Query-Future)
    - [Query Include Filter] _(under development)_
- Auditing _(under development)_

## Query Cache
**Query cache is the second level cache for Entity Framework.**

The result of the query is returned from the cache. If the query is not cached yet, the query is materialized and cached before being returned.

You can specify cache policy and cache tag to control CacheItem expiration.

**Support:**

_Cache Policy_

```csharp
// The query is cached using default QueryCacheManager options
var countries = ctx.Countries.Where(x => x.IsActive).FromCache();

// (EF5 | EF6) The query is cached for 2 hours
var states = ctx.States.Where(x => x.IsActive).FromCache(DateTime.Now.AddHours(2));

// (EF7) The query is cached for 2 hours without any activity
var options = new MemoryCacheEntryOptions() { SlidingExpiration = TimeSpan.FromHours(2)};
var states = ctx.States.Where(x => x.IsActive).FromCache(options);
```

_Cache Tags_

```csharp
var states = db.States.Where(x => x.IsActive).FromCache("countries", "states");
var stateCount = db.States.Where(x => x.IsActive).DelayedCount().FromCache("countries", "states");

// Expire all cache entry using the "countries" tag
QueryCacheManager.ExpireTag("countries");
```

**[Learn more](https://github.com/zzzprojects/EntityFramework-Plus/wiki/Query-Cache)**

## Query Delayed
**Delay the execution of a query which is normally executed to allow some features like Query Cache and Query Future.**

```csharp
// Oops! The query is already executed, we cannot use Query Cache or Query Future features
var count = ctx.Customers.Count();

// Query Cache
ctx.Customers.DelayedCount().FromCache();

// Query Future
ctx.Customers.DelayedCount().FutureValue();
```
> All LINQ extensions are supported: Count, First, FirstOrDefault, Sum, etc. 

**[Learn more](https://github.com/zzzprojects/EntityFramework-Plus/wiki/Query-Delayed)**

## Query Filter
**Filter query with predicate at global, instance or query level.**

**Support**

_Global Filter_
```csharp
public class EntityContext : DbContext
{
        public EntityContext() : base("MyDatabase")
        {
            // CONFIGURE all your filters for all context
            this.Filter<Customer>(x => x.Where(c => c.IsActive));
        }
        
        public DbSet<Customer> Customers { get; set; }
}

// SELECT * FROM Customer WHERE IsActive = true
var ctx = new EntityContext();
var customer = ctx.Customers.ToList();
```

_Instance Filter_
```csharp
var ctx = new EntityContext();

// CREATE filter
ctx.Filter<Customer>(x => x.Where(c => c.IsActive));

// SELECT * FROM Customer WHERE IsActive = true
var customer = ctx.Customers.ToList();
```

_Query Filter_
```csharp
var ctx = new EntityContext();

// CREATE filter disabled
ctx.Filter<Customer>(custom.EnumValue, x => x.Where(c => c.IsActive), false);

// SELECT * FROM Customer WHERE IsActive = true
var customer = ctx.Customers.Filter(MyEnum.Filter1).ToList();
```

**[Learn more](https://github.com/zzzprojects/EntityFramework-Plus/wiki/Query-Filter)**

## Query Future
**Query Future allow to reduce database roundtrip by batching multiple queries in the same sql command.**

All future query are stored in a pending list. When the first future query require a database roundtrip, all query are resolved in the same sql command instead of making a database roundtrip for every sql command.

**Support:**

_Future_

```csharp
// GET the states & countries list
var futureCountries = db.Countries.Where(x => x.IsActive).Future();
var futureStates = db.States.Where(x => x.IsActive).Future();

// TRIGGER all pending queries (futureCountries & futureStates)
var countries = futureCountries.ToList();
```

_FutureValue_

```csharp
// GET the first active customer and the number of avtive customers
var futureFirstCustomer = db.Customers.Where(x => x.IsActive).DelayedFirstOrDefault().FutureValue();
var futureCustomerCount = db.Customers.Where(x => x.IsActive).DelayedCount().FutureValue();

// TRIGGER all pending queries (futureFirstCustomer & futureCustomerCount)
Customer firstCustomer = futureFirstCustomer.Value;
```

**[Learn more](https://github.com/zzzprojects/EntityFramework-Plus/wiki/Query-Future)**

## FREE vs PRO
Every month, a new monthly trial of the PRO Version is available to let you evaluate all its features without limitations.

Features | FREE Version | PRO Version
------------ | :-------------: | :-------------:
Audit | Yes | Yes
Query Batch | Yes | Yes
Query Cache | Yes | Yes
Query Delayed | Yes | Yes
Query Filter | Yes | Yes
Query Future | Yes | Yes
Commercial License | **No** | Yes
Royalty-Free | **No** | Yes
Support & Upgrades (1 year) | **No** | Yes
Learn more about the PRO Version **(Not available until 2016)**

(Compatible with license from [.NET Entity Framework Extensions](http://www.zzzprojects.com/products/dotnet-development/entity-framework-extensions/))

## Support
Contact our outstanding customer support for any request. We usually answer within the next business day, hour, or minutes!

- Website (Not yet available)
- [Documentation](https://github.com/zzzprojects/EntityFramework-Plus/wiki)
- [Forum](https://zzzprojects.uservoice.com/forums/283924-entity-framework-plus)
- sales@zzzprojects.com

## More Projects
  - [NET Entity Framework Extensions](http://www.zzzprojects.com/products/dotnet-development/entity-framework-extensions/)
  - [NET Bulk Operations](http://www.zzzprojects.com/products/dotnet-development/bulk-operations/)
  - [Eval Expression.NET](https://github.com/zzzprojects/Eval-Expression.NET)
  - [Eval SQL.NET](https://github.com/zzzprojects/Eval-SQL.NET)
  - [Extension Methods Library](https://github.com/zzzprojects/Z.ExtensionMethods/)

