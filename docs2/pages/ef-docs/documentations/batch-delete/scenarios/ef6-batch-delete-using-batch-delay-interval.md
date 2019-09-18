---
Permalink: ef6-batch-delete-using-batch-delay-interval
---

# Batch Delay Interval

### Problem

You need to delete millions of records but also need to pause between batches to let other applications keep on performing their CRUD operations.

### Solution

The **BatchDelayInterval** property sets the amount of time (in milliseconds) to wait before starting the next delete batch.

Default Value = 0

{% include template-example.html %} 
```csharp

// using Z.EntityFramework.Plus; // Don't forget to include this.

// Pause 2 seconds between every batch
var date = DateTime.Now.AddYears(-2);
ctx.Users.Where(x => x.LastLoginDate < date)
         .Delete(x => x.BatchDelayInterval = 2000);

```
[Try it](https://dotnetfiddle.net/to4sjm)
