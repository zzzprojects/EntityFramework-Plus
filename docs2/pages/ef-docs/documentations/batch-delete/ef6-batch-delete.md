---
Permalink: batch-delete
---

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
[Try it](https://dotnetfiddle.net/R6D5BX)

## Scenarios

 - [Query Criteria](scenarios/ef6-batch-delete-query-criteria.md)
 - [Batch Size](scenarios/ef6-batch-delete-using-batch-size.md)
 - [Batch Delay Interval](scenarios/ef6-batch-delete-using-batch-delay-interval.md)
 - [Executing Interceptor](scenarios/ef6-batch-delete-executing-interceptor.md)
 
## Limitations

 - **DO NOT** support Complex Type
 - **DO NOT** support TPC
 - **DO NOT** support TPH
 - **DO NOT** support TPT
 
If you need to use one of this feature, you need to use the library [Entity Framework Extensions](https://entityframework-extensions.net/)

## Requirements

- **EF+ Batch Delete:** Full version or Standalone version
- **Entity Framework Version:** EF5, EF6
- **Minimum Framework Version:** .NET Framework 4

## Conclusion

**EF+ Batch Delete** is the most efficient way to delete records. You drastically improve your application performance by removing the need to retrieve and load entities in your context and by performing a single database roundtrip instead of one for every record.

Need help getting started? [info@zzzprojects.com](mailto:info@zzzprojects.com)

We welcome all comments, ideas and suggestions to improve our library.
