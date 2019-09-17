---
Permalink: ef-core-audit-exclude-include-property
---

# Exclude & Include Property

## Problem

You need to audit only a subset of your properties and you need to exclude/include properties by name and by entity type, base class or interface.

## Solution

Exclude/Include property methods:

{% include template-example.html %} 
```csharp

Exclude() // Exclude all properties for all entities
Exclude<T>() // Exclude all properties for entities of type "T" or derived from type "T"
Exclude<T>(selector) // Exclude all properties from the selector for entities of type "T" or derived from type "T"
Include() // Include all properties for all entities
Include<T>() // Include all properties for entities of type "T" or derived from type "T"
Include<T>(selector) // Include all properties from the selector for entities of type "T" or derived from type "T"
// using Z.EntityFramework.Plus; // Don't forget to include this.

// Globally
AuditManager.DefaultConfiguration.ExcludeProperty(); // Exclude ALL
AuditManager.DefaultConfiguration.IncludeProperty<IAuditable>();
AuditManager.DefaultConfiguration.ExcludeProperty<IAuditable>(x =>
   new { x.Property1, x.Property2 }); // Exclude single or many properties

// By Instance
var audit = new Audit();
audit.CreatedBy = "ZZZ Projects"; // Optional
audit.Configuration.ExcludeProperty(); // Exclude ALL
audit.Configuration.IncludeProperty<IAuditable>();
audit.Configuration.ExcludeProperty<IAuditable>(x =>
   new { x.Property1, x.Property2 }); // Exclude single or many properties
ctx.SaveChanges(audit);

```
[Try it](https://dotnetfiddle.net/E5fDhN)
