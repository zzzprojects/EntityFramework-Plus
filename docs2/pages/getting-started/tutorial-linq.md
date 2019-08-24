# LINQ

## LINQ Dynamic

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
[Try it in EF6](https://dotnetfiddle.net/iSnS3l) | [Try it in EF Core](https://dotnetfiddle.net/96FCM6)

[Learn more](/linq-dynamic)
