---
Permalink: ef-core-query-future-value-deferred
---

# EF+ Query FutureValue Deferred

Immediate resolution methods like **Count()** and **FirstOrDefault()** cannot use future methods since it executes the query immediately.

{% include template-example.html %} 
```csharp

// Oops! The query is already executed, we cannot delay the execution.
var count = ctx.Customers.Count();

// Oops! All customers will be retrieved instead of customer count
var count = ctx.Customers.Future().Count();

```
[Try it](https://dotnetfiddle.net/62LQVi)

**EF+ Query Deferred** has been created to resolve this issue. The resolution is now deferred instead of being immediate which lets you use FutureValue and get the expected result.

{% include template-example.html %} 
```csharp

// using Z.EntityFramework.Plus; // Don't forget to include this.
var ctx = new EntitiesContext();

// GET the first active customer and the number of active customers
var futureFirstCustomer = ctx.Customers.DeferredFirstOrDefault().FutureValue();
var futureCustomerCount = ctx.Customers.DeferredCount().FutureValue();

// TRIGGER all pending queries
Customer firstCustomer = futureFirstCustomer.Value;

// The future query is already resolved and contains the result
var count = futureCustomerCount.Value;

```

[Try it](https://dotnetfiddle.net/BI16rq)
