---
Permalink: ef6-query-future-value
---

# EF+ Query FutureValue

Query FutureValue delays the execution of the query returning a result.

{% include template-example.html %} 
```csharp

// using Z.EntityFramework.Plus; // Don't forget to include this.
var ctx = new EntitiesContext();

// GET the minimum and maximum product prices
var futureMaxPrice = ctx.Products.DeferredMax(x => x.Prices).FutureValue<int>();
var futureMinPrice = ctx.Products.DeferredMin(x => x.Prices).FutureValue<int>();

// TRIGGER all pending queries
int maxPrice = futureMaxPrice.Value;

// The future query is already resolved and contains the result
int maxPrice = futureMinPrice.Value;

```

[Try it](https://dotnetfiddle.net/4K4Fx2)
