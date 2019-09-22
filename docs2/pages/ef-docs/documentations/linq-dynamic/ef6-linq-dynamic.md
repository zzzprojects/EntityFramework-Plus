---
Permalink: linq-dynamic
---

# LINQ Dynamic

> This feature is now available on [Entity Framework Classic - LINQ Dynamic](http://entityframework-classic.net/linq-dynamic). Entity Framework Classic is a supported version from the latest EF6 code base. It supports .NET Framework and .NET Core and overcomes some EF limitations by adding tons of must-haves built-in features.

## Introduction

**LINQ Dynamic** in Entity Framework is supported through the [Eval-Expression.NET](http://eval-expression.net/) Library.

### Predicate

All LINQ predicate methods are supported. A string expression which return a Boolean function can be used as parameter.

 - Deferred
 - SkipWhile
 - TakeWhile
 - Where
 - Immediate
 - All
 - Any
 - Count
 - First
 - FirstOrDefault
 - Last
 - LastOrDefault
 - LongCount
 - Single
 - SingleOrDefault

{% include template-example.html %} 
```csharp

var list = ctx.Where(x => "x > 2").ToList();
var list = ctx.Where(x => "x > y", new { y = 2 }).ToList();

```
[Try it](https://dotnetfiddle.net/Otm0Aa)

## Options

 - [Order && Select](options/ef6-linq-dynamic-order-and-select.md)
 - [Execute](options/ef6-linq-dynamic-execute.md)
 
## Order && Select

All LINQ selector and order are supported. Most of them require the "Dynamic" suffix to not override default behavior (Ordering or selecting by a string is valid).

 - OrderByDescendingDynamic
 - OrderByDynamic
 - SelectDynamic
 - SelectMany
 - ThenByDescendingDynamic
 - ThenByDynamic

{% include template-example.html %} 
```csharp

var list = context.Customers.OrderByDescendingDynamic(x => "x.Name").ToList();
var list = context.Customers.SelectDynamic(x => "x.Name").ToList();

```
[Try it in EF6](https://dotnetfiddle.net/OJjBgK) | [Try it in EF Core](https://dotnetfiddle.net/8n2Xc0)

## Execute

The Execute method is the LINQ Dynamic ultimate methods which let you evaluate and execute a dynamic expression and return the result.

 - Execute
 - Execute< TResult >

{% include template-example.html %} 
```csharp
 
var list = ctx.Execute<IEnumerable<int>>("Where(x => x > 2)");
var list3 = ctx.Execute("Where(x => x > y).OrderBy(x => x).ToList()", new { y = 2 });

```
[Try it in EF6](https://dotnetfiddle.net/mwTqW7) | [Try it in EF Core](https://dotnetfiddle.net/LbU2at)
