---
Permalink: ef6-query-deferred-execute
---

# EF+ Query Deferred Execute

Execute the deferred query and return the result.

{% include template-example.html %} 
```csharp

// using Z.EntityFramework.Plus; // Don't forget to include this.
var ctx = new EntitiesContext();

var countDeferred = ctx.Customers.DeferredCount();
var count = countDeferred.Execute();

```
[Try it](https://dotnetfiddle.net/sXOfNB)

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
[Try it](https://dotnetfiddle.net/0BpVn1)
