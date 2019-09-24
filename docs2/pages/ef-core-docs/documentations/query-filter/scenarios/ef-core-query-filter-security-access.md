---
Permalink: ef-core-query-filter-security-access
---

# Security Access

Viewing sensible data often requires some permissions. For example, not everyone can see all posts in a forum.

*In this example, some posts are only available by role level.*

{% include template-example.html %} 
```csharp

// myRoleID = 1; // Administrator

// using Z.EntityFramework.Plus; // Don't forget to include this.
var ctx = new EntitiesContext();
ctx.Filter<Post>(x => q => q.Where(x.RoleID >= myRoleID));

// SELECT * FROM Posts WHERE RoleID >= 1
var list = ctx.Posts.ToList();

```

[Try it](https://dotnetfiddle.net/BknS6x)
