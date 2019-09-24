---
Permalink: ef-core-query-filter-by-as-no-filter
---

# EF+ Query Filter AsNoFilter

You can bypass all filters by using AsNoFilter method in a query if a special scenario doesn't require filtering.

{% include template-example.html %} 
```csharp

// using Z.EntityFramework.Plus; // Don't forget to include this.
var ctx = new EntitiesContext();

this.Filter<Customer>(q => q.Where(x => x.IsActive));

// SELECT * FROM Customer WHERE IsActive = true
var list = ctx.Customers.ToList();

// SELECT * FROM Customer
var list = ctx.Customers.AsNoFilter().ToList();

```

[Try it](https://dotnetfiddle.net/El05r4)
