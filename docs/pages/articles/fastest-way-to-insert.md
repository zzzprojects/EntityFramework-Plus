---
permalink: fastest-way-to-insert
---

## What’s the FASTEST way to insert in Entity Framework?

This question keeps getting asked a few times every week on Stack Overflow.
Most common solutions usually involve:

- Using AddRange over Add method
- Calling SaveChanges every X entities
- Creating a new context every X entities
- Disabling Change Detection
- Disabling Change Validation
- Disabling Proxy

Some of these solutions are good, some lead to side impact and none of them fix the main issue: One database round-trip is made for every entity you save!
So if you have 5000 entities to insert, 5000 databases round trip will be executed which are **INSANELY** slow.

A very common mistake is believing the AddRange method perform a Bulk Insert. Using the AddRange method greatly improve the performance because it calls the DetectChanges method only once after all entities are added to the change tracker. However, the SaveChanges method will still make one database round-trip per entity.

### Solution
To solve the performance problem, you need to reduce the number of database round-trip, and this is exactly what Entity Framework Extensions made by ZZZ Projects do by providing the BulkInsert method and all other bulk operations:

-	[BulkSaveChanges](#ef-bulksavechanges)
-	[BulkInsert](#ef-bulkinsert)
-	[BullUpdate](#ef-bulkupdate)
-	[BulkDelete](#ef-bulkdelete)
-	[BulkMerge](#ef-bulkmerge)

With this library, only a few database round-trip will now be required when saving 5000 entities which can lead to performance improvement by 50x time and more.

You can find the library documentation here: [EFE – Tutorials](http://entityframework-extensions.net/tutorials)

Getting started with this library cannot be easier. You download it using [NuGet](https://www.nuget.org/packages/Z.EntityFramework.Extensions/){:target="_blank"}, and all bulk method are automatically available from your context instance since methods extend the DbContext class.

Without having to configure anything, all models and scenarios are supported such Code First, Database First, Inheritances (TPC, TPH, and TPT), Relationship, and everything else!

## EF BulkSaveChanges
BulkSaveChanges method works like SaveChanges but perform save operations (INSERT, UPDATE, and DELETE) way faster.
There are multiple options available such:

- Disable Concurrency Check
- Disable Entity Framework Propagation for better performance
- Update Modified values only

This method is normally recommended over bulk method such as Bulk Insert since it acts like SaveChanges by using the Change Tracker and propagating the value.

Tutorial: [EFE – Bulk SaveChanges](http://entityframework-extensions.net/bulk-savechanges)

### Example
```csharp
ctx.BulkSaveChanges();

ctx.BulkSaveChanges(useEntityFrameworkPropagation: false);

ctx.BulkSaveChanges(options =>
{
    options.AllowConcurrency = false;
    options.ForceUpdateUnmodifiedValues = false;
});
```

## EF BulkInsert
BulkInsert method performs an INSERT operation and outputs the identity value.
There are multiple options available such:

-	Custom Column
-	Disabled output value for better performance
-	Insert only entities that don't already exists
-	Keep Identity Value

Bulk Insert is faster than BulkSaveChanges. However, Bulk Insert doesn’t use the Change Tracker so identity value propagation may differ. If you need to perform multiple bulk operations with some relation, it’s recommended to use BulkSaveChanges instead.

Tutorial: [EFE – Bulk Insert](http://entityframework-extensions.net/bulk-insert)

### Example
```csharp
ctx.BulkInsert(customers);
               
ctx.BulkInsert(customers, options =>
{
    options.ColumnInputExpression = x => new {x.Code, x.Email};
    options.AutoMapOutputDirection = false;
    options.InsertIfNotExists = true;
    options.InsertKeepIdentity = true;
});
```

## EF BulkUpdate
BulkUpdate method performs an UPDATE operation.

Tutorial: [EFE – Bulk Update](http://entityframework-extensions.net/bulk-update)

There are multiple options available such:

- Custom Column
- Custom Key
- Disable Concurrency Check

### Example
```csharp
ctx.BulkUpdate(customers, options =>
{
    options.ColumnInputExpression = x => new { x.Code, x.Email };
    options.ColumnPrimaryKeyExpression = x => new { x.Code };
    options.AllowConcurrency = false;
});
```

## EF BulkDelete
BulkDelete method performs a DELETE operation.

There are multiple options available such:

- Custom Key
-	Disable Concurrency Check

Tutorial: [EFE – Bulk Delete](http://entityframework-extensions.net/bulk-delete)

### Example
```csharp
ctx.BulkDelete(list);

ctx.BulkDelete(list, options =>
{
    options.ColumnPrimaryKeyExpression = x => new { x.Code };
    options.AllowConcurrency = false;
});
```

## EF BulkMerge
BulkMerge method performs an UPSERT operation and outputs the identity value. Existing entities matching the key are updated, and newly entities are inserted.

There are multiple options available such:

-	Custom Column
-	Custom Column (Insert Only)
-	Custom Column (Update Only)
-	Custom Key
- Disabled output value for better performance
-	Keep Identity Value

Tutorial: [EFE – Bulk Merge](http://entityframework-extensions.net/bulk-merge)

### Example
```csharp
ctx.BulkMerge(list);
               
ctx.BulkMerge(list, options =>
{
    options.ColumnInputExpression = x => new { x.Code, x.CreatedDate, x.UpdatedDate };
    options.IgnoreOnMergeInsertExpression = x => new { x. UpdatedDate };
    options.IgnoreOnMergeUpdateExpression = x => new { x.ID, x.CreatedDate };
    options.ColumnPrimaryKeyExpression = x => new { x.Code };
    options.AutoMapOutputDirection = false;
    options.MergeKeepIdentity = true;
});
```

## Conclusion
Entity Framework Extensions is well tested with over 10k unit tests and currently used by thousands of company all around the world. Without a doubt, it is the ultimate library to improve your performance with Entity Framework.

One the negative side, this library is not free.

ZZZ Projects give back value to the .NET community by supporting some other free libraries, tutorials and sponsoring project such as:

- [Entity Framework Plus](http://entityframework-plus.net/){:target="_blank"}
- [Html Agility Pack](http://html-agility-pack.net/){:target="_blank"}
- [Extension Methods](https://github.com/zzzprojects/Z.ExtensionMethods){:target="_blank"}
- [Dapper Tutorial](http://dapper-tutorial.net/){:target="_blank"}
- Help partners like [Entity Framework Tutorial](http://www.entityframeworktutorial.net/){:target="_blank"}
