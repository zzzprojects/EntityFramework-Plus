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
