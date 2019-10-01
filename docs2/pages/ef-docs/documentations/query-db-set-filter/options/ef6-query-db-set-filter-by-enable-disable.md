---
Permalink: ef6-query-db-set-filter-by-enable-disable
---

# EF+ Query Filter Enable/Disable

Filters are very flexible, you can enable and disable them at any time and only for a specific inheritance or interface if desired.

{% include template-example.html %} 
```csharp

// using Z.EntityFramework.Plus; // Don't forget to include this.
var ctx = new EntitiesContext();

// CREATE filter by interface
ctx.DbSetFilter<IAnimal>(MyEnum.EnumValue, q => q.Where(x => x.IsDomestic))

// DISABLE filter
ctx.DbSetFilter(MyEnum.EnumValue).Disable();

// SELECT * FROM Dog
var dogs = ctx.Dogs.ToList(); 

// ENABLE filter
ctx.DbSetFilter(MyEnum.EnumValue).Enable();

// SELECT * FROM Dog WHERE IsDomestic = true
var dogs = ctx.Dogs.ToList();

```
[Try it](https://dotnetfiddle.net/girbYB)
