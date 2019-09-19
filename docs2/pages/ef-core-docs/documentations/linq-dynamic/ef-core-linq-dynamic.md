---
Permalink: ef-core-linq-dynamic
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
[Try it in EF6](https://dotnetfiddle.net/Otm0Aa) | [Try it in EF Core](https://dotnetfiddle.net/1sTQwA)

## Options

 - [Order && Select](options/ef-core-linq-dynamic-order-and-select.md)
 - [Execute](options/ef-core-linq-dynamic-execute.md)
