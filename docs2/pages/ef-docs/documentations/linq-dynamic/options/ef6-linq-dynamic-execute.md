---
Permalink: ef6-linq-dynamic-execute
---

# Execute

The Execute method is the LINQ Dynamic ultimate methods which let you evaluate and execute a dynamic expression and return the result.

 - Execute
 - Execute< TResult >

{% include template-example.html %} 
```csharp
 
var list = ctx.Execute<IEnumerable<int>>("Where(x => x > 2)");
var list3 = ctx.Execute("Where(x => x > y).OrderBy(x => x).ToList()", new { y = 2 });

```
[Try it](https://dotnetfiddle.net/mwTqW7)
