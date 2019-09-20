---
Permalink: ef6-query-cache-query-deferred
---

## EF+ Query Cache Query Deferred

Immediate resolution methods like **Count()** and **FirstOrDefault()** cannot be cached since their expected behavior is to cache the count result or only the first item.

{% include template-example.html %} 
```csharp

// Oops! The query is already executed, we cannot cache it.
var count = ctx.Customers.Count();

// Oops! All customers are cached instead of customer count.
var count = ctx.Customers.FromCache().Count();
```
[Try it](https://dotnetfiddle.net/SOjxeY)

**EF+ Query Deferred** has been created to resolve this issue, the resolution is now deferred instead of being immediate which lets us cache the expected result.

{% include template-example.html %} 
```csharp
// using Z.EntityFramework.Plus; // Don't forget to include this.
var ctx = new EntitiesContext();

// The count is deferred, it is now cached.
var count = ctx.Customers.DeferredCount().FromCache();

```
[Try it](https://dotnetfiddle.net/4MUlef)

Query Deferred supports all Queryable extension methods and overloads.
