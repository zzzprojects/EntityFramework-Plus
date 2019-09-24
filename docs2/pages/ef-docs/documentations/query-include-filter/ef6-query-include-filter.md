---
Permalink: query-include-filter
---

# Query IncludeFilter

## Introduction

With Entity Framework, "Include" method is often used to load related entities / child collections. However, the method doesn't let you use LINQ queryable methods like **Where** to filter entities to include which is a major drawback. Excluding soft deleted records or limiting the result is a frequent real-life scenario which the "Include" method doesn't support.

**EF+ Query IncludeFilter** lets you filter and perform LINQ queryable methods on related entities.

{% include template-example.html %} 
```csharp

// using Z.EntityFramework.Plus; // Don't forget to include this.
var ctx = new EntitiesContext();

// LOAD orders and the first 10 active related entities.
var list = ctx.Orders.IncludeFilter(x => x.Items.Where(y => !y.IsSoftDeleted)
                                               .OrderBy(y => y.Date)
                                               .Take(10))
                     .ToList();

```
[Try it](https://dotnetfiddle.net/Duyw5p)

## EF+ Query IncludeFilter

IncludeFilter method works the same as "Include" method but lets you use LINQ Queryable extension methods as part of the query to filter related entities.

 - [Load one level](options/ef6-query-include-filter-load-one-level.md)
 - [Load multiple level](options/ef6-query-include-filter-load-multiple-level.md)

## Real Life Scenarios

 - [Paging](scenarios/ef6-query-include-filter-paging.md)
 - [Security Access](scenarios/ef6-query-include-filter-security-access.md)
 - [Soft Delete](scenarios/ef6-query-include-filter-soft-delete.md)
 
## Behind the code

### Step 1 - Custom Queryable

The first time IncludeFilter is called, a custom Queryable and Provider are created for your query and will be used to append all queries.

### Step 2 - Iterate or Execute

When an immediate method is invoked to resolve the query, the initial query is modified to create a new query which includes related entities.

 - The iterate method is invoked when it is possible to return multiple results like **ToList()** and **ToArray()**
 - The execute method is invoked when it is possible to return only one result like **First()** and **Last()**

### Original Query:

{% include template-example.html %} 
```csharp

var list = ctx.Orders.IncludeFilter(x => x.OrderItems.Where(y => !y.IsSoftDeleted))
                     .Take(10)
                     .ToList();

```

### Executed Query:

{% include template-example.html %} 
```csharp

var q1 = ctx.Orders.Take(10)
var p1 = q1.Select(x => x.OrderItems.Where(y => !y.IsSoftDeleted));

var list = q1.Select(x => new { x, p1 }).ToList().Select(x => x.X);

```

Entity Framework already does all the job by linking related entities to the parent. No future logic is required.

## Limitations

 - **DO NOT** work with **AsNoTracking**
 - Entity Framework 5:
   - Will never be supported. Use IncludeOptimized instead to filter collections
 - Cannot be mixed with projection
 - Cannot be mixed with Include (Include doesn't support projection)
 - Cannot be mixed with IncludeOptimized
 - Many to Many relation:
   - Not supported yet
 - Relationship:
   - Entities will contain all previously loaded related entities even if the Query does not return them. It's a limitation due to how Entity Framework relation work.

{% include template-example.html %} 
```csharp

// using Z.EntityFramework.Plus; // Don't forget to include this.
var ctx = new EntitiesContext();

ctx.Comments.ToList();

// The posts automatically contain all comments even without using the "Include" method.
ctx.Posts.ToList();

// Trying to load only one comment will obviously not work either.
ctx.Posts.IncludeFilter(q => q.Comments.Take(1)).ToList();

```

## Requirements

 - **EF+ Query IncludeFilter:** Full version or Standalone version
 - **Database Provider:** All supported
 - **Entity Framework Version:** EF6
 - **Minimum Framework Version:** .NET Framework 4

## Conclusion

Filtering included related entities is one of the most frequently asked questions in forums. **EF+ Query IncludeFilter** now makes it possible without any learning curve required for a junior developer to understand what the method does.

Need help getting started? [info@zzzprojects.com](mailto:info@zzzprojects.com)

We welcome all comments, ideas and suggestions to improve our library.
