---
Permalink: ef-core-query-filter-default-ordering
---

# Default Ordering

Default ordering can be often useful for base table like category. No matter the query, you probably want to show categories by alphabetic order.

*In this example, categories are sorted by name*

{% include template-example.html %} 
```csharp

// using Z.EntityFramework.Plus; // Don't forget to include this.
var ctx = new EntitiesContext();
ctx.Filter<Category>(q => q.OrderByDescending(x => x.Name));

// SELECT * FROM Category ORDER BY Name
var list = ctx.Categories.ToList()

```
