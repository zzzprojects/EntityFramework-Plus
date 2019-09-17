---
Permalink: ef6-audit-exclude-include-entity
---

# Exclude & Include Entity

## Problem

You need to audit only a subset of your entities and you need to exclude/include by entity types, base class or interface.

## Solution

Exclude/Include entity methods:

{% include template-example.html %} 
```csharp

Exclude(predicate) // Exclude entities using a predicate
Exclude<T>() // Exclude entities of type "T" or derived from type "T"
Include(predicate) // Include entities using a predicate
Include<T>() // Include entities of type "T" or derived from type "T"
// using Z.EntityFramework.Plus; // Don't forget to include this.

// Globally
AuditManager.DefaultConfiguration.Exclude(x => true); // Exclude ALL
AuditManager.DefaultConfiguration.Include<IAuditable>();

// By Instance
var audit = new Audit();
audit.CreatedBy = "ZZZ Projects"; // Optional
audit.Configuration.Exclude(x => true); // Exclude ALL
audit.Configuration.Include<IAuditable>();
ctx.SaveChanges(audit);

```
[Try it](https://dotnetfiddle.net/8TbEdO)
