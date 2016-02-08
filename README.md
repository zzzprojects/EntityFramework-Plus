##Entity Framework Bulk Operations & Utilities##

## Download

Full Version | NuGet | NuGet Install
------------ | :-------------: | :-------------:
Z.EntityFramework.Plus.EF7 | <a href="https://www.nuget.org/packages/Z.EntityFramework.Plus.EF7/" target="_blank" alt="download nuget"><img src="https://img.shields.io/nuget/v/Z.EntityFramework.Plus.EF7.svg?style=flat-square" /></a> <a href="https://www.nuget.org/packages/Z.EntityFramework.Plus.EF7/" target="_blank" alt="download nuget"><img src="https://img.shields.io/nuget/dt/Z.EntityFramework.Plus.EF7.svg?style=flat-square" /></a> | ```PM> Install-Package Z.EntityFramework.Plus.EF7```
Z.EntityFramework.Plus.EF6 | <a href="https://www.nuget.org/packages/Z.EntityFramework.Plus.EF6/" target="_blank" alt="download nuget"><img src="https://img.shields.io/nuget/v/Z.EntityFramework.Plus.EF6.svg?style=flat-square" /></a> <a href="https://www.nuget.org/packages/Z.EntityFramework.Plus.EF6/" target="_blank" alt="download nuget"><img src="https://img.shields.io/nuget/dt/Z.EntityFramework.Plus.EF6.svg?style=flat-square" /></a> | ```PM> Install-Package Z.EntityFramework.Plus.EF6```
Z.EntityFramework.Plus.EF5 | <a href="https://www.nuget.org/packages/Z.EntityFramework.Plus.EF5/" target="_blank" alt="download nuget"><img src="https://img.shields.io/nuget/v/Z.EntityFramework.Plus.EF5.svg?style=flat-square" /></a> <a href="https://www.nuget.org/packages/Z.EntityFramework.Plus.EF5/" target="_blank" alt="download nuget"><img src="https://img.shields.io/nuget/dt/Z.EntityFramework.Plus.EF5.svg?style=flat-square" /></a> | ```PM> Install-Package Z.EntityFramework.Plus.EF5```

<a href="https://github.com/zzzprojects/EntityFramework-Plus/wiki/Downloads">More download options (Full and Standalone Version)</a>

Stay updated with latest changes

<a href="https://twitter.com/zzzprojects" target="_blank"><img src="http://www.zzzprojects.com/images/twitter_follow.png" alt="Twitter Follow" height="24" /></a>
<a href="https://www.facebook.com/zzzprojects/" target="_blank"><img src="http://www.zzzprojects.com/images/facebook_like.png" alt="Facebook Like" height="24" /></a>

## Features
- Batch Operations _(Available very soon)_
    - [Batch Delete](https://github.com/zzzprojects/EntityFramework-Plus/wiki/EF-Batch-Delete-%7C-Entity-Framework-Delete-object-without-retrieving-it)
    - [Batch Update](https://github.com/zzzprojects/EntityFramework-Plus/wiki/EF-Batch-Update-%7C-Entity-Framework-Update-object-without-retrieving-it)
- Query
    - [Query Cache](https://github.com/zzzprojects/EntityFramework-Plus/wiki/EF-Query-Cache-%7C-Entity-Framework-Second-Level-Caching)
    - [Query Deferred](https://github.com/zzzprojects/EntityFramework-Plus/wiki/EF-Query-Deferred-%7C-Entity-Framework-deferring-immediate-LINQ-query-execution)
    - [Query Filter](https://github.com/zzzprojects/EntityFramework-Plus/wiki/EF-Query-Filter-%7C-Entity-Framework-Dynamic-Instance-and-Global-Filters)
    - [Query Future](https://github.com/zzzprojects/EntityFramework-Plus/wiki/EF-Query-Future-%7C-Entity-Framework-Combine-and-Execute-Multiple-SQL-Command)
    - [Query IncludeFilter](https://github.com/zzzprojects/EntityFramework-Plus/wiki/EF-Query-IncludeFilter-%7C-Entity-Framework-Include-Related-Entities-using-Where-Filter) 
    - [Query IncludeOptimized](https://github.com/zzzprojects/EntityFramework-Plus/wiki/EF-Query-IncludeOptimized-%7C-Entity-Framework-Filter-Child-Collections-and-Optimize-Include)
- [Audit](https://github.com/zzzprojects/EntityFramework-Plus/wiki/EF-Audit-%7C-Entity-Framework-Audit-Trail-Context-and-Track-Changes)

**Bulk Operation only available with [Entity Framework Extensions](http://www.zzzprojects.com/products/dotnet-development/entity-framework-extensions/)**
- Bulk SaveChanges
- Bulk Insert
- Bulk Update
- Bulk Delete
- Bulk Merge

## Batch Delete
Deletes multiples rows in a single database roundtrip and without loading entities in the context.

```csharp
// using Z.EntityFramework.Plus; // Don't forget to include this.

// DELETE all users which has been inactive for 2 years
ctx.Users.Where(x => x.LastLoginDate < DateTime.Now.AddYears(-2))
         .Delete();

// DELETE using a BatchSize
ctx.Users.Where(x => x.LastLoginDate < DateTime.Now.AddYears(-2))
         .Delete(x => x.BatchSize = 1000);
```

**[Learn more](https://github.com/zzzprojects/EntityFramework-Plus/wiki/EF-Batch-Delete-%7C-Entity-Framework-Delete-object-without-retrieving-it)**

## Batch Update
Updates multiples rows using an expression in a single database roundtrip and without loading entities in the context.

```csharp
// using Z.EntityFramework.Plus; // Don't forget to include this.

// UPDATE all users which has been inactive for 2 years
ctx.Users.Where(x => x.LastLoginDate < DateTime.Now.AddYears(-2))
         .Update(x => new User() { IsSoftDeleted = 1 });

// UPDATE using a BatchSize
ctx.Users.Where(x => x.LastLoginDate < DateTime.Now.AddYears(-2))
         .Update(x => new User() { IsSoftDeleted = 1 },
                 x => x.BatchSize = 1000);
```

**[Learn more](https://github.com/zzzprojects/EntityFramework-Plus/wiki/EF-Batch-Update-%7C-Entity-Framework-Update-object-without-retrieving-it)**

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
var stateCount = db.States.Where(x => x.IsActive).DeferredCount().FromCache("countries", "states");

// Expire all cache entry using the "countries" tag
QueryCacheManager.ExpireTag("countries");
```

**[Learn more](https://github.com/zzzprojects/EntityFramework-Plus/wiki/EF-Query-Cache-%7C-Entity-Framework-Second-Level-Caching)**

## Query Deferred
**Defer the execution of a query which is normally executed to allow some features like Query Cache and Query Future.**

```csharp
// Oops! The query is already executed, we cannot use Query Cache or Query Future features
var count = ctx.Customers.Count();

// Query Cache
ctx.Customers.DeferredCount().FromCache();

// Query Future
ctx.Customers.DeferredCount().FutureValue();
```
> All LINQ extensions are supported: Count, First, FirstOrDefault, Sum, etc. 

**[Learn more](https://github.com/zzzprojects/EntityFramework-Plus/wiki/EF-Query-Deferred-%7C-Entity-Framework-deferring-immediate-LINQ-query-execution)**

## Query Filter
**Filter query with predicate at global, instance or query level.**

**Support:**

_Global Filter_
```csharp
// CREATE global filter
QueryFilterManager.Filter<Customer>(x => x.Where(c => c.IsActive));

var ctx = new EntityContext();

// TIP: Add this line in EntitiesContext constructor instead
QueryFilterManager.InitilizeGlobalFilter(ctx);

// SELECT * FROM Customer WHERE IsActive = true
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
ctx.Filter<Customer>(CustomEnum.EnumValue, x => x.Where(c => c.IsActive), false);

// SELECT * FROM Customer WHERE IsActive = true
var customer = ctx.Customers.Filter(CustomEnum.EnumValue).ToList();
```

**[Learn more](https://github.com/zzzprojects/EntityFramework-Plus/wiki/EF-Query-Filter-%7C-Entity-Framework-Dynamic-Instance-and-Global-Filters)**

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
var futureFirstCustomer = db.Customers.Where(x => x.IsActive).DeferredFirstOrDefault().FutureValue();
var futureCustomerCount = db.Customers.Where(x => x.IsActive).DeferredCount().FutureValue();

// TRIGGER all pending queries (futureFirstCustomer & futureCustomerCount)
Customer firstCustomer = futureFirstCustomer.Value;
```

**[Learn more](https://github.com/zzzprojects/EntityFramework-Plus/wiki/EF-Query-Future-%7C-Entity-Framework-Combine-and-Execute-Multiple-SQL-Command)**

## Query IncludeFilter
Entity Framework already support eager loading however the major drawback is you cannot control what will be included. There is no way to load only active item or load only the first 10 comments.

**EF+ Query Include** make it easy:
```csharp
var ctx = new EntityContext();

// Load only active comments
var posts = ctx.Post.IncludeFilter(x => x.Comments.Where(x => x.IsActive));
```

**[Learn more](https://github.com/zzzprojects/EntityFramework-Plus/wiki/EF-Query-IncludeFilter-%7C-Entity-Framework-Include-Related-Entities-using-Where-Filter)**

## Query IncludeOptimized
Improve SQL generate by Include and filter child collections at the same times!

```csharp
var ctx = new EntityContext();

// Load only active comments using an optimized include
var posts = ctx.Post.IncludeOptimized(x => x.Comments.Where(x => x.IsActive));
```

**[Learn more](https://github.com/zzzprojects/EntityFramework-Plus/wiki/EF-Query-IncludeFilter-%7C-Entity-Framework-Include-Related-Entities-using-Where-Filter)**

## Audit
Allow to easily track changes, exclude/include entity or property and auto save audit entries in the database.

**Support:**
 - AutoSave Audit
 - Exclude & Include Entity
 - Exclude & Include Property
 - Format Value
 - Ignore Events
 - Property Unchanged
 - Soft Add & Soft Delete

```csharp
// using Z.EntityFramework.Plus; // Don't forget to include this.

var ctx = new EntityContext();
// ... ctx changes ...

var audit = new Audit();
audit.CreatedBy = "ZZZ Projects"; // Optional
ctx.SaveChanges(audit);

// Access to all auditing information
var entries = audit.Entries;
foreach(var entry in entries)
{
    foreach(var property in entry.Properties)
    {
    }
}
```

AutoSave audit in your database
```csharp
AuditManager.DefaultConfiguration.AutoSavePreAction = (context, audit) =>
    (context as EntityContext).AuditEntries.AddRange(audit.Entries);
```

**[Learn more](https://github.com/zzzprojects/EntityFramework-Plus/wiki/EF-Audit-%7C-Entity-Framework-Audit-Trail-Context-and-Track-Changes)**

## FREE vs PRO
Every month, a new monthly trial of the PRO Version is available to let you evaluate all its features without limitations.

Features | FREE Version | [PRO Version](http://entityframework-plus.net/#pro)
------------ | :-------------: | :-------------:
Bulk SaveChanges | **No** | Yes
Bulk Insert | **No** | Yes
Bulk Update | **No** | Yes
Bulk Delete | **No** | Yes
Bulk Merge | **No** | Yes
Audit | Yes | Yes
Batch Delete | Yes | Yes
Batch Update | Yes | Yes
Query Cache | Yes | Yes
Query Deferred | Yes | Yes
Query Filter | Yes | Yes
Query Future | Yes | Yes
Query IncludeFilter | Yes | Yes
Query IncludeOptimized | Yes | Yes
Commercial License | Yes | Yes
Royalty-Free | Yes | Yes
Support & Upgrades (1 year) | **No** | Yes
Learn more about the **[PRO Version](http://entityframework-plus.net/#pro)**

(Compatible with license from [.NET Entity Framework Extensions](http://www.zzzprojects.com/products/dotnet-development/entity-framework-extensions/))

## Contribution

Supporting & developing FREE features takes hundreds and thousands of hours! If you like our product please consider making a donation to encourage and keep us running.

We'll never require donations, but we appreciate them greatly

<a href="http://www.zzzprojects.com/contribute/" target="_blank"><img src="http://www.zzzprojects.com/images/paypal-contribute.png" alt="Contribute" height="48"></a>

A **huge thanks** for your extra support.


## More Projects

**Entity Framework**
- [Entity Framework Extensions](http://www.zzzprojects.com/products/dotnet-development/entity-framework-extensions/)
- [Entity Framework Plus](https://github.com/zzzprojects/EntityFramework-Plus)

**Bulk Operations**
- [NET Entity Framework Extensions](http://www.zzzprojects.com/products/dotnet-development/entity-framework-extensions/)
- [NET Bulk Operations](http://www.zzzprojects.com/products/dotnet-development/bulk-operations/)

**Expression Evaluator**
- [Eval SQL.NET](https://github.com/zzzprojects/Eval-SQL.NET)
- [Eval Expression.NET](https://github.com/zzzprojects/Eval-Expression.NET)

**Others**
- [Extension Methods Library](https://github.com/zzzprojects/Z.ExtensionMethods/)
- [LINQ Async](https://github.com/zzzprojects/Linq-AsyncExtensions)

**Need more info?** info@zzzprojects.com

Contact our outstanding customer support for any request. We usually answer within the next business day, hour, or minutes!
