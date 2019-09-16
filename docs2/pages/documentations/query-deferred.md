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
[Try it in EF6](https://dotnetfiddle.net/WgpFfH) | [Try it in EF Core](https://dotnetfiddle.net/cu3UiE)

Here comes in play the deferred query which acts exactly like deferred methods, by modifying the query expression without resolving it.

{% include template-example.html %} 
```csharp

// using Z.EntityFramework.Plus; // Don't forget to include this.
var ctx = new EntitiesContext();

// The count is deferred and cached.
var count = ctx.Customers.DeferredCount().FromCache();

```
[Try it in EF6](https://dotnetfiddle.net/ZChhmD) | [Try it in EF Core](https://dotnetfiddle.net/xIz5wx)

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

## EF+ Query Deferred

Defer the execution of a query which is normally executed to allow some features like Query Cache and Query Future.

{% include template-example.html %} 
```csharp

// using Z.EntityFramework.Plus; // Don't forget to include this.
var ctx = new EntitiesContext();

// Query Cache
ctx.Customers.DeferredCount().FromCache();

// Query Future
ctx.Customers.DeferredCount().FutureValue();

```
[Try it in EF6](https://dotnetfiddle.net/5KcNj3) | [Try it in EF Core](https://dotnetfiddle.net/ohLJL3)

## EF+ Query Deferred Execute

Execute the deferred query and return the result.

{% include template-example.html %} 
```csharp

// using Z.EntityFramework.Plus; // Don't forget to include this.
var ctx = new EntitiesContext();

var countDeferred = ctx.Customers.DeferredCount();
var count = countDeferred.Execute();

```
[Try it in EF6](https://dotnetfiddle.net/sXOfNB) | [Try it in EF Core](https://dotnetfiddle.net/Ou2Ly4)

## EF+ Query Deferred Execute Async

Execute the Deferred query asynchronously and return the result.

**ExecuteAsync** methods are available starting from .NET Framework 4.5 and support all the same options than **Execute** methods.

{% include template-example.html %} 
```csharp

// using Z.EntityFramework.Plus; // Don't forget to include this.
var ctx = new EntitiesContext();

var countDeferred = ctx.Customers.DeferredCount();
var taskCount = countDeferred.ExecuteAsync();

```
[Try it in EF6](https://dotnetfiddle.net/0BpVn1) | [Try it in EF Core](https://dotnetfiddle.net/1pttmj)

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
 - **Entity Framework Version:** EF5, EF6, EF Core
 - **Minimum Framework Version:** .NET Framework 4

## Conclusion

As we saw, EF+ **Query Deferred** brings considerable advantages to other libraries by letting them use immediate methods without removing any of their features.

Need help getting started? [info@zzzprojects.com](mailto:info@zzzprojects.com)

We welcome all comments, ideas and suggestions to improve our library.
