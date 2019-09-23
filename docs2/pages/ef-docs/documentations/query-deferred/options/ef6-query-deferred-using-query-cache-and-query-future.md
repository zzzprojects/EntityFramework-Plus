---
Permalink: ef6-query-deferred-using-query-cache-and-query-future
---

# EF+ Query Deferred

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
[Try it](https://dotnetfiddle.net/5KcNj3)
