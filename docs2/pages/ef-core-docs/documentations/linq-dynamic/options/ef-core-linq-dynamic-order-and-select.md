---
Permalink: ef-core-linq-dynamic-order-and-select
---

# Order && Select

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
[Try it](https://dotnetfiddle.net/8n2Xc0)
