---
Permalink: ef6-batch-delete-using-batch-size
---

# Batch Size

## Problem

You need to delete millions of records and need to use a batch size to increase performance.

## Solution

The **BatchSize** property sets the amount of rows to delete in a single batch.

Default Value = 4000


{% include template-example.html %} 
```csharp

// using Z.EntityFramework.Plus; // Don't forget to include this.
var date = DateTime.Now.AddYears(-2);
ctx.Users.Where(x => x.LastLoginDate < date)
         .Delete(x => x.BatchSize = 1000);

```
[Try it](https://dotnetfiddle.net/c6TLU3)
