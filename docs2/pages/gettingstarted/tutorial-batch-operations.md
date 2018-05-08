---
permalink: tutorial-batch-operations
---

## Batch Delete

Deletes multiples rows in a single database roundtrip and without loading entities in the context.

{% include template-example.html %} 
```csharp
// using Z.EntityFramework.Plus; // Don't forget to include this.

// DELETE all users which has been inactive for 2 years
ctx.Users.Where(x => x.LastLoginDate < DateTime.Now.AddYears(-2))
         .Delete();

// DELETE using a BatchSize
ctx.Users.Where(x => x.LastLoginDate < DateTime.Now.AddYears(-2))
         .Delete(x => x.BatchSize = 1000);

```

***Support:** EF5, EF6, EF Core*

[Learn more](/batch-delete)

## Batch Update

Updates multiples rows using an expression in a single database roundtrip and without loading entities in the context.

{% include template-example.html %} 
```csharp
// using Z.EntityFramework.Plus; // Don't forget to include this.

// UPDATE all users which has been inactive for 2 years
ctx.Users.Where(x => x.LastLoginDate < DateTime.Now.AddYears(-2))
         .Update(x => new User() { IsSoftDeleted = 1 });

```

***Support:** EF5, EF6, EF Core*

[Learn more](/batch-update)