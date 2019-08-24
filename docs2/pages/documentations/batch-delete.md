# Batch Delete

> This feature is now available on [Entity Framework Classic - Delete from Query](http://entityframework-classic.net/delete-from-query). Entity Framework Classic is a supported version from the latest EF6 code base. It supports .NET Framework and .NET Core and overcomes some EF limitations by adding tons of must-haves built-in features.

## Introduction

Deleting using Entity Framework can be very slow if you need to delete hundreds or thousands of entities. Entities are first loaded in the context before being deleted which is very bad for the performance and then, they are deleted one by one which makes the delete operation even worse.

**EF+ Batch Delete** deletes multiple rows in a single database roundtrip and without loading entities in the context.

{% include template-example.html %} 
```csharp
// using Z.EntityFramework.Plus; // Don't forget to include this.

// DELETE all users inactive for 2 years
var date = DateTime.Now.AddYears(-2);
ctx.Users.Where(x => x.LastLoginDate < date)
         .Delete();

// DELETE using a BatchSize
var date = DateTime.Now.AddYears(-2);
ctx.Users.Where(x => x.LastLoginDate < date)
         .Delete(x => x.BatchSize = 1000);

```
[Try it in EF6](https://dotnetfiddle.net/R6D5BX) | [Try it in EF Core](https://dotnetfiddle.net/8qqyPw)

## Batch Delete

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
[Try it in EF6](https://dotnetfiddle.net/DTWmh1) | [Try it in EF Core](https://dotnetfiddle.net/HQauA0)

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
[Try it in EF6](https://dotnetfiddle.net/KUHvru) | [Try it in EF Core](https://dotnetfiddle.net/Xdxjg1)

## Batch Size

### Problem

You need to delete millions of records and need to use a batch size to increase performance.

### Solution

The **BatchSize** property sets the amount of rows to delete in a single batch.

Default Value = 4000


{% include template-example.html %} 
```csharp

// using Z.EntityFramework.Plus; // Don't forget to include this.
var date = DateTime.Now.AddYears(-2);
ctx.Users.Where(x => x.LastLoginDate < date)
         .Delete(x => x.BatchSize = 1000);

```
[Try it in EF6](https://dotnetfiddle.net/c6TLU3) | [Try it in EF Core](https://dotnetfiddle.net/CpzGUz)

## Batch Delay Interval

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
[Try it in EF6](https://dotnetfiddle.net/to4sjm) | [Try it in EF Core](https://dotnetfiddle.net/3iRApA
)

## Executing Interceptor

### Problem

You need to log the DbCommand information or change the CommandText, Connection or Transaction before the batch is executed.

### Solution

The **Executing** property intercepts the DbCommand with an action before being executed.


{% include template-example.html %} 
```csharp

// using Z.EntityFramework.Plus; // Don't forget to include this.

string commandText
var date = DateTime.Now.AddYears(-2);
ctx.Users.Where(x => x.LastLoginDate < date)
         .Delete(x => { x.Executing = command => commandText = command.CommandText; });

```
[Try it in EF6](https://dotnetfiddle.net/VOEdOD) | [Try it in EF Core](https://dotnetfiddle.net/KrtT0e)

## EF Core InMemory

### Problem

You want to use **BatchDelete** with In Memory Testing for EF Core.

### Solution

Specify a DbContext factory in the **BatchDeleteManager** to enable BatchDelete with In Memory. A DbContext factory must be

{% include template-example.html %} 
```csharp

// Options
var db = new DbContextOptionsBuilder();
db.UseInMemoryDatabase();

// Specify InMemory DbContext Factory
BatchDeleteManager.InMemoryDbContextFactory = () => new MyContext(db.Options);

// Use the same code as with Relational Database
var _context = new MyContext(db.Options);
_context.Foos.Delete();

```

## Limitations

 - **DO NOT** support Complex Type
 - **DO NOT** support TPC
 - **DO NOT** support TPH
 - **DO NOT** support TPT
 
 If you need to use one of this feature, you need to use the library [Entity Framework Extensions](https://entityframework-extensions.net/)
 
 ### EF Core & Client Evaluation
 
 _Do not use this feature if Client Evaluation is enabled_
 
`Batch Delete` use the SQL generated by EF Core. When a filter is made on client-side, it means the filtering happens in the application and not in the SQL executed.

In other words, even if you put a filter, all rows tables could be potentially deleted if the filter is made on the client-side.

We always recommend to [disable the client evaluation](https://docs.microsoft.com/en-us/ef/core/querying/client-eval#optional-behavior-throw-an-exception-for-client-evaluation) to avoid performance issue in your application.

## Requirements

- **EF+ Batch Delete:** Full version or Standalone version
- **Entity Framework Version:** EF5, EF6, EFCore
- **Minimum Framework Version:** .NET Framework 4

## Conclusion

**EF+ Batch Delete** is the most efficient way to delete records. You drastically improve your application performance by removing the need to retrieve and load entities in your context and by performing a single database roundtrip instead of one for every record.

Need help getting started? [info@zzzprojects.com](mailto:info@zzzprojects.com)

We welcome all comments, ideas and suggestions to improve our library.
