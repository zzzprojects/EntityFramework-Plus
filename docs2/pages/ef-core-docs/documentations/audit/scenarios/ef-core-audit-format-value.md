---
Permalink: ef-core-audit-format-value
---

# Format Value

## Problem

You need to format a value with a different string representation, for example, adding a dollar sign to the money value.

## Solution

Format value method:

 - Format< T >(selector, formatter)
 
{% include template-example.html %} 
```csharp

// using Z.EntityFramework.Plus; // Don't forget to include this.

// Globally
AuditManager.DefaultConfiguration.Format<OrderItem>(x => x.Price, 
                                                    x => x.ToString("$#.00"));

// By Instance
var audit = new Audit();
audit.CreatedBy = "ZZZ Projects"; // Optional
audit.Configuration.Format<OrderItem>(x => x.Price,
                                      x => x.ToString("$#.00"));
ctx.SaveChanges(audit);

```
[Try it](https://dotnetfiddle.net/PZ4CJ3)
