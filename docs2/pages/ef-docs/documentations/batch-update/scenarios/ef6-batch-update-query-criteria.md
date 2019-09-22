---
Permalink: ef6-batch-update-query-criteria
---

# Batch Update using Query Criteria

## Problem

You need to update one or millions of records based on a query criteria and an expression.

## Solution

The **Update** IQueryable extension method updates rows matching the query criteria with an expression without loading entities in the context.

{% include template-example.html %} 
```csharp

// using Z.EntityFramework.Plus; // Don't forget to include this.

// UPDATE all users inactive for 2 years
var date = DateTime.Now.AddYears(-2);
ctx.Users.Where(x => x.LastLoginDate < date)
         .Update(x => new User() { IsSoftDeleted = 1 });

```
[Try it](https://dotnetfiddle.net/sfMLRj)

## Batch UpdateAsync

### Problem

You need to update one or millions of records based on a query criteria and an expression asynchronously.

### Solution

The **UpdateAsync** IQueryable extension method updates asynchronously rows matching the query criteria with an expression without loading entities in the context.

{% include template-example.html %} 
```csharp

// using Z.EntityFramework.Plus; // Don't forget to include this.

// UPDATE all users inactive for 2 years
var date = DateTime.Now.AddYears(-2);
ctx.Users.Where(x => x.LastLoginDate < date)
         .UpdateAsync(x => new User() { IsSoftDeleted = 1 });

```
[Try it](https://dotnetfiddle.net/7fHg1g)
