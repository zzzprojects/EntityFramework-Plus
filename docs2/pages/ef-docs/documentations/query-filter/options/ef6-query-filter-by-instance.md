---
Permalink: ef6-query-filter-by-instance
---

# EF+ Query Filter By Instance

Instance filter applies filters to the current context only. The filtering logic is added once the context is created.

{% include template-example.html %} 
```csharp

// using Z.EntityFramework.Plus; // Don't forget to include this.
var ctx = new EntitiesContext();

ctx.Filter<Customer>(q => q.Where(x => x.IsActive));

// SELECT * FROM Customer WHERE IsActive = true
var list = ctx.Customers.ToList();
```

[Try it](https://dotnetfiddle.net/Tyw4Xy)

***Use entities context constructor to make some filter "global" to all context.***
