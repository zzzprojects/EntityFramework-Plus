---
Permalink: ef6-audit-property-unchanged
---

# Property Unchanged

## Problem

You need to keep track of all changed and unchanged properties

## Solution

You can choose to ignore or not property unchanged with IgnorePropertyUnchanged.

By default, properties unchanged are ignored unless it's part of the primary key.

{% include template-example.html %} 
```csharp

// using Z.EntityFramework.Plus; // Don't forget to include this.

// Globally
AuditManager.DefaultConfiguration.IgnorePropertyUnchanged = false;

// By Instance
var audit = new Audit();
audit.CreatedBy = "ZZZ Projects"; // Optional
audit.Configuration.IgnorePropertyUnchanged = false;
ctx.SaveChanges(audit);

```
[Try it](https://dotnetfiddle.net/tYE0YR)
