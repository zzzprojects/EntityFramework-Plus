---
Permalink: ef-core-audit-ignore-events
---

# Ignore Events

### Problem

You need to ignore a relationship or a subset of all available events.

### Solution

Ignore event properties:

 - IgnoreEntityAdded
 - IgnoreEntityDeleted
 - IgnoreEntityModified
 - IgnoreEntitySoftAdded
 - IgnoreEntitySoftDeleted
 - IgnoreRelationshipAdded
 - IgnoreRelationshipDeleted

{% include template-example.html %} 
```csharp

// using Z.EntityFramework.Plus; // Don't forget to include this.

// Globally
AuditManager.DefaultConfiguration.IgnoreRelationshipAdded = true;
AuditManager.DefaultConfiguration.IgnoreRelationshipDeleted = true;

// By Instance
var audit = new Audit();
audit.CreatedBy = "ZZZ Projects"; // Optional
audit.Configuration.IgnoreRelationshipAdded = true;
audit.Configuration.IgnoreRelationshipDeleted = true;
ctx.SaveChanges(audit);

```
[Try it](https://dotnetfiddle.net/w6D8z4)
