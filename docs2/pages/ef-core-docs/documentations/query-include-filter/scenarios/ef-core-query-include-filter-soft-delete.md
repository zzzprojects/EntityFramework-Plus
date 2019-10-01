---
Permalink: ef-core-query-include-filter-soft-delete
---

# Soft Delete

Soft Deleted Records (Include active related entities)

{% include template-example.html %} 
```csharp

// using Z.EntityFramework.Plus; // Don't forget to include this.
var ctx = new EntitiesContext();

// LOAD posts and active comments.
var posts= ctx.Posts.IncludeFilter (x => x.Comments.Where(y => !y.IsSoftDeleted))
                    .ToList();

```
[Try it](https://dotnetfiddle.net/qXHvYM)
