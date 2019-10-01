---
Permalink: ef6-query-db-set-filter-by-inheritance-interface
---

# EF+ Query Filter By Inheritance/Interface

Filter can be enabled and disabled by class inheritance and interface.

{% include template-example.html %} 
```csharp

// using Z.EntityFramework.Plus; // Don't forget to include this.
var ctx = new EntitiesContext();

// CREATE filter by inheritance
ctx.DbSetFilter<BaseDog>(q => q.Where(x => !x.IsDangerous));

// CREATE filter by interface
ctx.DbSetFilter<IAnimal>(q => q.Where(x => x.IsDomestic));

// SELECT * FROM Cat WHERE IsDomestic = 1
var cats = ctx.Cats.ToList();

// SELECT * FROM Dog WHERE IsDomestic = 1 AND IsDangerous = 0
var dogs = ctx.Dogs.ToList();

```
[Try it](https://dotnetfiddle.net/flFnBf)
