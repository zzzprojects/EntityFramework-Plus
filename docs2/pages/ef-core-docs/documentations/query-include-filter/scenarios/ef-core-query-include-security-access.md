---
Permalink: ef-core-query-include-security-access
---

# Security Access

Security Access (Include available related entities)

{% include template-example.html %} 
```csharp

// myRoleID = 1; // Administrator

// using Z.EntityFramework.Plus; // Don't forget to include this.
var ctx = new EntitiesContext();

// LOAD posts and available comments for the role level.
var posts= ctx.Posts.IncludeFilter(x => x.Comments.Where(y => y.RoleID >= myRoleID))
                    .ToList();

```
[Try it](https://dotnetfiddle.net/RKvLJU)
