# Batch Operations

## Batch Delete

Deletes multiples rows in a single database roundtrip and without loading entities in the context.

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
[Try it in EF6](https://dotnetfiddle.net/asjI4U) | [Try it in EF Core](https://dotnetfiddle.net/KMgmZs)

***Support:** EF5, EF6, EF Core*

[Learn more](/batch-delete)

## Batch Update

Updates multiples rows using an expression in a single database roundtrip and without loading entities in the context.

{% include template-example.html %} 
```csharp
// using Z.EntityFramework.Plus; // Don't forget to include this.

// UPDATE all users inactive for 2 years
var date = DateTime.Now.AddYears(-2);
ctx.Users.Where(x => x.LastLoginDate < date)
         .Update(x => new User() { IsSoftDeleted = 1 });

```
[Try it in EF6](https://dotnetfiddle.net/cV3IHD) | [Try it in EF Core](https://dotnetfiddle.net/KMgmZs)

***Support:** EF5, EF6, EF Core*

[Learn more](/batch-update)
