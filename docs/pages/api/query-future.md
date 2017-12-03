---
permalink: query-future
---

## Introduction

Every time an immediate method like **ToList** or **FirstOrDefault** is invoked on a query, a database round trip is made to retrieve data. While the most application doesn't have performance issues with making multiple round trips, batching multiple queries into one can be critical for some heavy traffic applications for scalability. Major ORM like NHibernate had this feature for a long time but, unfortunately for Entity Framework users, batching queries is only available through third party libraries.

**EF+ Query Future** opens up all batching future queries features for Entity Framework users.

To batch multiples queries, simply append **Future** or **FutureValue** method to the query. All future queries will be stored in a pending list, and when the first future query requires a database round trip, all queries will be resolved in the same SQL command.

{% include template-example.html %} 
{% highlight csharp %}

// using Z.EntityFramework.Plus; // Don't forget to include this.
var ctx = new EntitiesContext();

// CREATE a pending list of future queries
var futureCountries = ctx.Countries.Where(x => x.IsActive).Future();
var futureStates = ctx.States.Where(x => x.IsActive).Future();

// TRIGGER all pending queries in one database round trip
// SELECT * FROM Country WHERE IsActive = true;
// SELECT * FROM State WHERE IsActive = true
var countries = futureCountries.ToList();

// futureStates is already resolved and contains the result
var states = futureStates.ToList();

{% endhighlight %}


## EF+ Query Future

Query Future delays the execution of a query returning an IEnumerable.

{% include template-example.html %} 
{% highlight csharp %}

// using Z.EntityFramework.Plus; // Don't forget to include this.
var ctx = new EntitiesContext();

// CREATE a pending list of future queries
var futureCountries = ctx.Countries.Where(x => x.IsActive).Future();
var futureStates = ctx.States.Where(x => x.IsActive).Future();

// TRIGGER all pending queries in one database round trip
// SELECT * FROM Country WHERE IsActive = true;
// SELECT * FROM State WHERE IsActive = true
var countries = futureCountries.ToList();

// futureStates is already resolved and contains the result
var states = futureStates.ToList();

{% endhighlight %}

## EF+ Query FutureValue

Query FutureValue delays the execution of the query returning a result.

{% include template-example.html %} 
{% highlight csharp %}

// using Z.EntityFramework.Plus; // Don't forget to include this.
var ctx = new EntitiesContext();

// GET the minimum and maximum product prices
var futureMaxPrice = ctx.Products.DeferredMax(x => x.Prices).FutureValue<int>();
var futureMinPrice = ctx.Products.DeferredMin(x => x.Prices).FutureValue<int>();

// TRIGGER all pending queries
int maxPrice = futureMaxPrice.Value;

// The future query is already resolved and contains the result
int maxPrice = futureMinPrice.Value;

{% endhighlight %}

## EF+ Query FutureValue Deferred

Immediate resolution methods like **Count()** and **FirstOrDefault()** cannot use future methods since it executes the query immediately.

{% include template-example.html %} 
{% highlight csharp %}

// Oops! The query is already executed, we cannot delay the execution.
var count = ctx.Customers.Count();

// Oops! All customers will be retrieved instead of customer count
var count = ctx.Customers.Future().Count();

{% endhighlight %}

**EF+ Query Deferred** has been created to resolve this issue. The resolution is now deferred instead of being immediate which lets you use FutureValue and get the expected result.

{% include template-example.html %} 
{% highlight csharp %}

// using Z.EntityFramework.Plus; // Don't forget to include this.
var ctx = new EntitiesContext();

// GET the first active customer and the number of active customers
var futureFirstCustomer = ctx.Customer.DeferredFirstOrDefault().FutureValue();
var futureCustomerCount = ctx.Customers.DeferredCount().FutureValue();

// TRIGGER all pending queries
Customer firstCustomer = futureFirstCustomer.Value;

// The future query is already resolved and contains the result
var count = futureCustomerCount.Value;

{% endhighlight %}

## Real Life Scenarios

 - Multi tables information scenario: Client and all related information (order, invoice, etc.) must be loaded.

{% include template-example.html %} 
{% highlight csharp %}

// using Z.EntityFramework.Plus; // Don't forget to include this.
var ctx = new EntitiesContext();

var futureClient = ctx.Clients.DeferredFirst(x => x.ClientID = myClientID)
                                 .FutureValue();
var futureOrders = ctx.Orders.Where(x => x.ClientID = myClientID).Future();
var futureOrderDetails = ctx.OrderDetails.Where(x => x.ClientID = myClientID).Future();
var futureInvoices = ctx.Invoices.Where(x => x.ClientID = myClientID).Future();

// ONE database round trip is required
var client = futureClient.Value;
var orders = futureOrders.ToList();

{% endhighlight %}

 - **Paging Scenario:** The first ten posts must be returned but you also need to know the total numbers of posts

{% include template-example.html %} 
{% highlight csharp %}

// using Z.EntityFramework.Plus; // Don't forget to include this.
var ctx = new EntitiesContext();

var futurePost = ctx.Posts.OrderBy(x => x.CreatedDate).Take(10).Future()
var futurePostCount = ctx.Post.DeferredCount().FutureValue();

// ONE database round trip is required
var post = futurePost.ToList();
var postCount = futurePostCount.Value;

{% endhighlight %}

## Behind the code

 - All queries from a context using query future are added to a batch list.
 - When the first database round trip is required:
   - All sql commands are combined into one sql command.
   - The sql command is executed and a data reader is returned.
   - For every result in the data reader, the result is set to the corresponding query future.

## Limitations

 - Provider:
   - Provider not supported are added on demand
   - Provider which doesn't support multiple statements like SQL Compact cannot be supported

## Requirements

 - **EF+ Query Future:** Full version or Standalone version
 - **Database Provider:** SQL Server
 - **Entity Framework Version:** EF5, EF6, EF Core
 - **Minimum Framework Version:** .NET Framework 4

## Conclusion

As we saw, EF+ Query Future follows a good architecture principle:

 - **Flexible:** Future, FutureValue and FutureValue deferred make it possible to use it in any kind of scenario.
 - **Maintainable:** The easy to use API, documentation and available source code allows new developers to quickly understand this feature.
 - **Scalable:** Query Future gets only better as the number of user/traffic grows by drastically reducing database round trips.

Need help getting started? [info@zzzprojects.com](mailto:info@zzzprojects.com)

We welcome all comments, ideas and suggestions to improve our library.
