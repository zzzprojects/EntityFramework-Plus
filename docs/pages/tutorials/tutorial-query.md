---
permalink: tutorial-query
---

## Query Cache

Query cache is the second level cache for Entity Framework.

The result of the query is returned from the cache. If the query is not cached yet, the query is materialized and cached before being returned.

You can specify cache policy and cache tag to control CacheItem expiration.

### Support:

#### Cache Policy

{% include template-example.html %} 
{% highlight csharp %}
// The query is cached using default QueryCacheManager options
var countries = ctx.Countries.Where(x => x.IsActive).FromCache();

// (EF5 | EF6) The query is cached for 2 hours
var states = ctx.States.Where(x => x.IsActive).FromCache(DateTime.Now.AddHours(2));

// (EF7) The query is cached for 2 hours without any activity
var options = new MemoryCacheEntryOptions() { SlidingExpiration = TimeSpan.FromHours(2)};
var states = ctx.States.Where(x => x.IsActive).FromCache(options);

{% endhighlight %}

#### Cache Tags

{% include template-example.html %} 
{% highlight csharp %}
var states = db.States.Where(x => x.IsActive).FromCache("countries", "states");
var stateCount = db.States.Where(x => x.IsActive).DeferredCount().FromCache("countries", "states");

// Expire all cache entry using the "countries" tag
QueryCacheManager.ExpireTag("countries");

{% endhighlight %}

***Support:** EF5, EF6, EF Core*

[Learn more](/query-cache)

## Query Deferred

Defer the execution of a query which is normally executed to allow some features like Query Cache and Query Future.

{% include template-example.html %} 
{% highlight csharp %}
// Oops! The query is already executed, we cannot use Query Cache or Query Future features
var count = ctx.Customers.Count();

// Query Cache
ctx.Customers.DeferredCount().FromCache();

// Query Future
ctx.Customers.DeferredCount().FutureValue();
All LINQ extensions are supported: Count, First, FirstOrDefault, Sum, etc.

{% endhighlight %}

***Support:** EF5, EF6, EF Core*

[Learn more](/query-deferred)

## Query Filter

Filter query with predicate at global, instance or query level.

### Support:

#### Global Filter

{% include template-example.html %} 
{% highlight csharp %}
// CREATE global filter
QueryFilterManager.Filter<Customer>(x => x.Where(c => c.IsActive));

var ctx = new EntityContext();

// TIP: Add this line in EntitiesContext constructor instead
QueryFilterManager.InitilizeGlobalFilter(ctx);

// SELECT * FROM Customer WHERE IsActive = true
var customer = ctx.Customers.ToList();

{% endhighlight %}

#### Instance Filter

{% include template-example.html %} 
{% highlight csharp %}
var ctx = new EntityContext();

// CREATE filter
ctx.Filter<Customer>(x => x.Where(c => c.IsActive));

// SELECT * FROM Customer WHERE IsActive = true
var customer = ctx.Customers.ToList();

{% endhighlight %}

#### Query Filter

{% include template-example.html %} 
{% highlight csharp %}
var ctx = new EntityContext();

// CREATE filter disabled
ctx.Filter<Customer>(CustomEnum.EnumValue, x => x.Where(c => c.IsActive), false);

// SELECT * FROM Customer WHERE IsActive = true
var customer = ctx.Customers.Filter(CustomEnum.EnumValue).ToList();

{% endhighlight %}

***Support:** EF5, EF6, EF Core*

[Learn more](/query-filter)

## Query Future

Query Future allow to reduce database roundtrip by batching multiple queries in the same sql command.

All future query are stored in a pending list. When the first future query require a database roundtrip, all query are resolved in the same sql command instead of making a database roundtrip for every sql command.

### Support:

#### Future

{% include template-example.html %} 
{% highlight csharp %}
// GET the states & countries list
var futureCountries = db.Countries.Where(x => x.IsActive).Future();
var futureStates = db.States.Where(x => x.IsActive).Future();

// TRIGGER all pending queries (futureCountries & futureStates)
var countries = futureCountries.ToList();

{% endhighlight %}

#### FutureValue

{% include template-example.html %} 
{% highlight csharp %}
// GET the first active customer and the number of avtive customers
var futureFirstCustomer = db.Customers.Where(x => x.IsActive).DeferredFirstOrDefault().FutureValue();
var futureCustomerCount = db.Customers.Where(x => x.IsActive).DeferredCount().FutureValue();

// TRIGGER all pending queries (futureFirstCustomer & futureCustomerCount)
Customer firstCustomer = futureFirstCustomer.Value;

{% endhighlight %}

***Support:** EF5, EF6, EF Core*

[Learn more](/query-future)

## Query IncludeFilter

Entity Framework already support eager loading however the major drawback is you cannot control what will be included. There is no way to load only active item or load only the first 10 comments.

{% include template-example.html %} 
{% highlight csharp %}
EF+ Query Include make it easy:

var ctx = new EntityContext();

// Load only active comments
var posts = ctx.Post.IncludeFilter(x => x.Comments.Where(x => x.IsActive));

{% endhighlight %}

***Support:** EF6*

[Learn more](/query-include-filter)

## Query IncludeOptimized

Improve SQL generate by Include and filter child collections at the same times!

{% include template-example.html %} 
{% highlight csharp %}
var ctx = new EntityContext();

// Load only active comments using an optimized include
var posts = ctx.Post.IncludeOptimized(x => x.Comments.Where(x => x.IsActive));

{% endhighlight %}

***Support:** EF5, EF6*

[Learn more](/query-include-optimized)