# Query DbSetFilter

## Definition

The **Query DbSetFilter** is the old filter API for Entity Framework 6 (EF6) before the code switched to use Interceptor instead.

The feature revamp was done in 2016 to support include and LazyLoading but, also have some limitation such as performance and instance filter that the old code didn't have.

## Limitations

 - **DO NOT** Support include
 - **DO NOT** Support lazy loading

## API Rename

To make both features live together, method in Query DbSetFilter has been renamed to add prefix with DbSet before Filter.

|Query Filter	|Query DbSetFilter |
|:------------- |:---------------- |
|AsNoFilter	|AsNoDbSetFilter|
|Filter	|DbSetFilter|
|QueryFilterManager	|QueryDbSetFilterManager|

## EF+ Query Filter Global

Global filter can be used by any context.

Global filter is normally preferred in most scenarios over instance filter since the filter code is centralized in one method over being spread in multiple methods.

{% include template-example.html %} 
```csharp

// using Z.EntityFramework.Plus; // Don't forget to include this.
QueryDbSetFilterManager.Filter<Customer>(q => q.Where(x => x.IsActive));

var ctx = new EntitiesContext();
// TIP: You can also add this line in EntitiesContext constructor instead
QueryDbSetFilterManager.InitilizeGlobalFilter(ctx);

// SELECT * FROM Customer WHERE IsActive = true
var list = ctx.Customers.ToList();

```
{% include component-try-it.html href='https://dotnetfiddle.net/HijoZX' %}

***Use entities context constructor to initialize global filter by default.***

## EF+ Query Filter By Instance

Instance filter applies filters to the current context only. The filtering logic is added once the context is created.

{% include template-example.html %} 
```csharp

// using Z.EntityFramework.Plus; // Don't forget to include this.
var ctx = new EntitiesContext();

ctx.DbSetFilter<Customer>(q => q.Where(x => x.IsActive));

// SELECT * FROM Customer WHERE IsActive = true
var list = ctx.Customers.ToList();

```
{% include component-try-it.html href='https://dotnetfiddle.net/3Xz39f' %}

***Use entities context constructor to make some filter "global" to all context.***

## EF+ Query Filter By Query

Query filter applies filters to specific queries only. The filtering logic is added globally or by instance but in a disabled state and then it is enabled by these specific queries.

{% include template-example.html %} 
```csharp

// using Z.EntityFramework.Plus; // Don't forget to include this.
var ctx = new EntitiesContext();

// CREATE a disabled filter
ctx.DbSetFilter<Customer>(MyEnum.EnumValue, q => q.Where(x => x.IsActive), false);

// SELECT * FROM Customer
var list = ctx.Customers.DbSetFilter(MyEnum.EnumValue).ToList();

```
{% include component-try-it.html href='https://dotnetfiddle.net/XmtVWN' %}

## EF+ Query Filter By Inheritance/Interface

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
{% include component-try-it.html href='https://dotnetfiddle.net/flFnBf' %}

## EF+ Query Filter Enable/Disable

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
{% include component-try-it.html href='https://dotnetfiddle.net/girbYB' %}

## EF+ Query Filter AsNoFilter

You can bypass all filters by using AsNoFilter method in a query if a special scenario doesn't require filtering.

{% include template-example.html %} 
```csharp

// using Z.EntityFramework.Plus; // Don't forget to include this.
var ctx = new EntitiesContext();

this.DbSetFilter<Customer>(q => q.Where(x => x.IsActive));

// SELECT * FROM Customer WHERE IsActive = true
var list = ctx.Customers.ToList();

// SELECT * FROM Customer
var list = ctx.Customers.AsNoDbSetFilter().ToList();

```
{% include component-try-it.html href='https://dotnetfiddle.net/ZIA1kt' %}
