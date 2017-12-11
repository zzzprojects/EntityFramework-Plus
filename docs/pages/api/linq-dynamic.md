---
permalink: linq-dynamic
---

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
{% highlight csharp %}

var list = ctx.Where(x => "x > 2").ToList();
var list = ctx.Where(x => "x > y", new { y = 2 }).ToList();

{% endhighlight %}

## Order && Select

All LINQ selector and order are supported. Most of them require the "Dynamic" suffix to not override default behavior (Ordering or selecting by a string is valid).

 - OrderByDescendingDynamic
 - OrderByDynamic
 - SelectDynamic
 - SelectMany
 - ThenByDescendingDynamic
 - ThenByDynamic

{% include template-example.html %} 
{% highlight csharp %}

var list = ctx.SelectDynamic(x => "new { y = x + 1 }").ToList();
var list = ctx.SelectDynamic(x => "new { y = x + 1 }", new { y = 1 }).ToList();
var list = new List<int>() { 5, 2, 4, 1, 3 };

var list2 = list.OrderByDynamic(x => "x + 1");
var list3 = list.OrderByDynamic(x => "x + y", new { y = 1 });

{% endhighlight %}

## Execute

The Execute method is the LINQ Dynamic ultimate methods which let you evaluate and execute a dynamic expression and return the result.

 - Execute
 - Execute< TResult >

{% include template-example.html %} 
{% highlight csharp %}
 
var list = ctx.Execute<IEnumerable<int>>("Where(x => x > 2)");
var list3 = ctx.Execute("Where(x => x > y).OrderBy(x => x).ToList()", new { y = 2 });

{% endhighlight %}
