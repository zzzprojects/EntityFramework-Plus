---
Permalink: ef6-query-future-paging
---

# Paging Scenario

The first ten posts must be returned but you also need to know the total numbers of posts

{% include template-example.html %} 
```csharp

// using Z.EntityFramework.Plus; // Don't forget to include this.
var ctx = new EntitiesContext();

var futurePost = ctx.Posts.OrderBy(x => x.CreatedDate).Take(10).Future()
var futurePostCount = ctx.Post.DeferredCount().FutureValue();

// ONE database round trip is required
var post = futurePost.ToList();
var postCount = futurePostCount.Value;

```

[Try it](https://dotnetfiddle.net/oA24FP)
