---
Permalink: batch-update
---

# Batch Update

> This feature is now available on [Entity Framework Classic - Update from Query](http://entityframework-classic.net/update-from-query). Entity Framework Classic is a supported version from the latest EF6 code base. It supports .NET Framework and .NET Core and overcomes some EF limitations by adding tons of must-haves built-in features.

## Introduction

Updating using Entity Framework can be very slow if you need to update hundreds or thousands of entities with the same expression. Entities are first loaded in the context before being updated which is very bad for the performance and then, they are updated one by one which makes the update operation even worse.

**EF+ Batch Update** updates multiple rows using an expression in a single database roundtrip and without loading entities in the context.

{% include template-example.html %} 
```csharp

// using Z.EntityFramework.Plus; // Don't forget to include this.

// UPDATE all users inactive for 2 years
var date = DateTime.Now.AddYears(-2);
ctx.Users.Where(x => x.LastLoginDate < date)
         .Update(x => new User() { IsSoftDeleted = 1 });

```
[Try it](https://dotnetfiddle.net/uzsdub)

## Scenarios

 - [Query Criteria](scenarios/ef6-batch-update-query-criteria.md)
 - [Executing Interceptor](scenarios/ef6-batch-update-executing-interceptor.md)
 
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

**EF+ Batch Update** is the most efficient way to update records using an expression. You drastically improve your application performance by removing the need to retrieve and load entities in your context and by performing a single database roundtrip instead of making one for every record.

Need help getting started? [info@zzzprojects.com](mailto:info@zzzprojects.com)

We welcome all comments, ideas and suggestions to improve our library.
