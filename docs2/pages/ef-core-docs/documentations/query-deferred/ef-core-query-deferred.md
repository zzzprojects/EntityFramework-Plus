---
Permalink: ef-core-query-deferred
---

# Query Deferred

> This feature is now available on [Entity Framework Classic - Query Deferred](http://entityframework-classic.net/query-deferred). Entity Framework Classic is a supported version from the latest EF6 code base. It supports .NET Framework and .NET Core and overcomes some EF limitations by adding tons of must-haves built-in features.

## Introduction

There are two types of IQueryable extension methods:

Deferred Methods: The query expression is modified but the query is not resolved (Select, Where, etc.).
Immediate Methods: The query expression is modified and the query is resolved (Count, First, etc.).
However, some third party features like **Query Cache** and **Query Future** cannot be used directly with Immediate Method since the query is already resolved.

**EF+ Query Deferred** provides more flexibility to other features.

{% include template-example.html %} 
```csharp

// Oops! The query is already executed, we cannot cache it.
var count = ctx.Customers.Count();

// Oops! All customers are cached instead of the customer count.
var count = ctx.Customers.FromCache().Count();

```
[Try it](https://dotnetfiddle.net/cu3UiE)

Here comes in play the deferred query which acts exactly like deferred methods, by modifying the query expression without resolving it.

{% include template-example.html %} 
```csharp

// using Z.EntityFramework.Plus; // Don't forget to include this.
var ctx = new EntitiesContext();

// The count is deferred and cached.
var count = ctx.Customers.DeferredCount().FromCache();

```
[Try it](https://dotnetfiddle.net/xIz5wx)

#### All LINQ IQueryable extension methods and overloads are supported:

 - DeferredAggregate
 - DeferredAll
 - DeferredAny
 - DeferredAverage
 - DeferredContains
 - DeferredCount
 - DeferredElementAt
 - DeferredElementAtOrDefault
 - DeferredFirst
 - DeferredFirstOrDefault
 - DeferredLast
 - DeferredLastOrDefault
 - DeferredLongCount
 - DeferredMax
 - DeferredMin
 - DeferredSequenceEqual
 - DeferredSingle
 - DeferredSingleOrDefault
 - DeferredSum

## Options

 - [Using Query Cache and Query Future](options/ef-core-query-deferred-using-query-cache-and-query-future.md)
 - [Execute](options/ef-core-query-deferred-execute.md)
 
## Real Life Scenarios

EF Query Deferred brings advantages to other third party features:

 - Allows to use Immediate Method with EF+ Query Cache.
 - Allows to use Immediate Method with EF+ Query Future.
 - Allows to use Immediate Method with YOUR own features.

## Behind the code
When a deferred method is used, the query expression is created exactly like a non-deferred method but instead of invoking the execute method from the query provider, a new instance of a class QueryDeferred<TResult> is created using the query and the expression.

The QueryDeferred instance has methods to either execute the expression from the query provider or let a third party library use the object query.

## Limitations

None.

## Requirements

 - **EF+ Query Deferred:** Full version or Standalone version
 - **Database Provider:** All supported
 - **Entity Framework Version:** EF Core
 - **Minimum Framework Version:** .NET Framework 4

## Conclusion

As we saw, EF+ **Query Deferred** brings considerable advantages to other libraries by letting them use immediate methods without removing any of their features.

Need help getting started? [info@zzzprojects.com](mailto:info@zzzprojects.com)

We welcome all comments, ideas and suggestions to improve our library.
