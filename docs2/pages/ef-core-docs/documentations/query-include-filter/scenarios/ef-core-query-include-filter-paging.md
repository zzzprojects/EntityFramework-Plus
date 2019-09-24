
---
Permalink: ef-core-query-include-filter-paging
---

# Paging

Paging (Include a range of related entities)

{% include template-example.html %} 
```csharp

// using Z.EntityFramework.Plus; // Don't forget to include this.
var ctx = new EntitiesContext();

// LOAD posts and most threading comments.
var posts= ctx.Posts.IncludeFilter(x => x.Comments
                                         .OrderByDescending(y => y.ThreadingScore)
                                         .Take(10))
                     .ToList();

```
[Try it](https://dotnetfiddle.net/IMdizK)
