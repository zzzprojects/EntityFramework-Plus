---
Permalink: ef6-batch-delete-query-criteria
---

# Batch Delete using Query Criteria

### Problem

You need to delete one or millions of records based on a query criteria.

### Solution

The **Delete** IQueryable extension methods deletes rows matching the query criteria without loading entities in the context.

{% include template-example.html %} 
```csharp

// using Z.EntityFramework.Plus; // Don't forget to include this.

// DELETE all users
ctx.Users.Delete();

// DELETE all users inactive for 2 years
var date = DateTime.Now.AddYears(-2);
ctx.Users.Where(x => x.LastLoginDate < date)
         .Delete();

```
[Try it](https://dotnetfiddle.net/DTWmh1)

## Batch DeleteAsync

### Problem

You need to delete one or millions of records based on a query criteria asynchronously.

### Solution

The **DeleteAsync** IQueryable extension methods deletes asynchronously rows matching the query criteria without loading entities in the context.

{% include template-example.html %} 
```csharp

// using Z.EntityFramework.Plus; // Don't forget to include this.

// DELETE all users
ctx.Users.DeleteAsync();

// DELETE all users inactive for 2 years
var date = DateTime.Now.AddYears(-2);
ctx.Users.Where(x => x.LastLoginDate < date)
         .DeleteAsync();

```
[Try it](https://dotnetfiddle.net/KUHvru)
