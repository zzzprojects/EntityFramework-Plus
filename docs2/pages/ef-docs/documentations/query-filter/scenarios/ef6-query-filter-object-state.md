---
Permalink: ef6-query-filter-object-state
---

# Object State

Removing inactive or soft deleted records is probably the most common scenario. A soft delete is often useful when related data cannot be deleted. For example, the customer cannot be deleted because related orders cannot be deleted instead, he becomes inactive.

*In this example, we display only active category.*

{% include template-example.html %} 
```csharp

// using Z.EntityFramework.Plus; // Don't forget to include this.
var ctx = new EntitiesContext();
ctx.Filter<ISoftDeleted>(q => q.Where(x => !x.IsSoftDeleted));

// SELECT * FROM Category WHERE IsSoftDeleted = false
var list = ctx.Categories.ToList();

```

[Try it](https://dotnetfiddle.net/4vcAQA)
