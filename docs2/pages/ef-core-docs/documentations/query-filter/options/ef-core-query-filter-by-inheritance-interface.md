---
Permalink: ef-core-query-filter-by-inheritance-interface
---

# EF+ Query Filter By Inheritance/Interface

Filter can be enabled and disabled by class inheritance and interface.

{% include template-example.html %} 
```csharp

// using Z.EntityFramework.Plus; // Don't forget to include this.
var ctx = new EntitiesContext();

// CREATE filter by inheritance
ctx.Filter<BaseDog>(q => q.Where(x => !x.IsDangerous));

// CREATE filter by interface
ctx.Filter<IAnimal>(q => q.Where(x => x.IsDomestic));

// SELECT * FROM Cat WHERE IsDomestic = true
var cats = ctx.Cats.ToList();

// SELECT * FROM Dog WHERE IsDomestic = true AND IsDangerous = false
var dogs = ctx.Dogs.ToList();

```

[Try it](https://dotnetfiddle.net/iX5gWN)
