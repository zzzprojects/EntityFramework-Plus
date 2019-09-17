---
Permalink: ef-core-audit-soft-add-soft-delete
---

# Soft Add & Soft Delete

## Problem

You need to keep an audit when a soft add/restore or a soft delete event happens.

## Solution

Soft Add & Soft Delete methods:

 - SoftAdded(predicate)
 - SoftDeleted(predicate)

When an entity satisfies the predicate, the audit entry state will be changed from "EntityModified" to either "EntitySoftAdded" or "EntitySoftDeleted".


{% include template-example.html %} 
```csharp

// using Z.EntityFramework.Plus; // Don't forget to include this.

// Globally
AuditManager.DefaultConfiguration.SoftDeleted<ISoftDelete>(x => x.IsDeleted);

// By Instance
var audit = new Audit();
audit.CreatedBy = "ZZZ Projects"; // Optional
audit.Configuration.SoftDeleted<ISoftDelete>(x => x.IsDeleted);
ctx.SaveChanges(audit);

```
[Try it](https://dotnetfiddle.net/Etv7yC)
