---
Permalink: ef6-query-db-set-filter-by-query
---

# EF+ Query Filter By Query

Query filter applies filters to specific queries only. The filtering logic is added globally or by instance but in a disabled state and then it is enabled by these specific queries.

{% include template-example.html %} 
```csharp

// using Z.EntityFramework.Plus; // Don't forget to include this.
var ctx = new EntitiesContext();

// CREATE a disabled filter
ctx.DbSetFilter<Customer>(MyEnum.EnumValue, q => q.Where(x => x.IsActive), false);

// SELECT * FROM Customer
var list = ctx.Customers.DbSetFilter(MyEnum.EnumValue).ToList();

```
[Try it](https://dotnetfiddle.net/XmtVWN)
