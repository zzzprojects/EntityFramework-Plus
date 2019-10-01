---
Permalink: ef6-query-include-filter-load-one-level
---

# Load one level

{% include template-example.html %} 
```csharp

// using Z.EntityFramework.Plus; // Don't forget to include this.
var ctx = new EntitiesContext();

// LOAD blogs and related active posts.
var blogs = ctx.Blogs.IncludeFilter(x => x.Posts.Where(y => !y.IsSoftDeleted)).ToList();

```
[Try it](https://dotnetfiddle.net/10sM7d)
