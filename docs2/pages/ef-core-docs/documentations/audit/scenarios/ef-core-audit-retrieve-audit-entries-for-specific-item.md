---
Permalink: ef-core-audit-retrieve-audit-entries-for-specific-item
---

# Retrieve AuditEntries for specific item

## Problem

You want to retrieve all AuditEntry for a specific item.

## Solution

You can filter the AuditEntries DbSet using Where method and providing either the item or the key.

{% include template-example.html %} 
```csharp

using (var ctx = new TestContext())
{
    ctx.AuditEntries.Where(item);
    ctx.AuditEntries.Where<Entity_Basic>(item.ID);
    ctx.AuditEntries.Where<Entity_Basic>(101);
}

```
[Try it](https://dotnetfiddle.net/6qiMrl)

## Audit, AuditEntry & AuditEntryProperty

AuditEntry and AuditEntryProperty can be added in your context to automatically save audit entries using the AutoSavePreAction property. Only mapped properties are mapped to your database.

## Audit

 - Properties
   - Configuration
   - CreatedBy
   - Entries
 - Methods
   - PostSaveChanges()
   - PreSaveChanges()

## AuditEntry

 - Properties (Mapped)
   - AuditEntryID
   - EntitySetName
   - EntityTypeName
   - State
   - StateName
   - CreatedBy
   - CreatedDate
 - Properties (Not Mapped)
   - ObjectStateEntry
   - Parent
   - Properties
   - State

## AuditEntryProperty

 - Properties (Mapped)
   - AuditEntryPropertyID
   - AuditEntryID
   - RelationName
   - PropertyName
   - NewValueFormatted (Mapped to NewValue)
   - OldValueFormatted (Mapped to OldValue)
 - Properties (Not Mapped)
   - NewValue
   - OldValue
   - Parent
