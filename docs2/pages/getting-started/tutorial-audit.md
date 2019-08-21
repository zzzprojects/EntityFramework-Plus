# Audit

## Audit

Allow to easily track changes, exclude/include entity or property and auto save audit entries in the database.

### Support:

 - AutoSave Audit
 - Exclude & Include Entity
 - Exclude & Include Property
 - Format Value
 - Ignore Events
 - Property Unchanged
 - Soft Add & Soft Delete

{% include template-example.html %} 
```csharp
// using Z.EntityFramework.Plus; // Don't forget to include this.

var ctx = new EntityContext();
// ... ctx changes ...

var audit = new Audit();
audit.CreatedBy = "ZZZ Projects"; // Optional
ctx.SaveChanges(audit);

// Access to all auditing information
var entries = audit.Entries;
foreach(var entry in entries)
{
    foreach(var property in entry.Properties)
    {
    }
}

```
[Try it in EF6](https://dotnetfiddle.net/89tii1) | [Try it in EF Core](https://dotnetfiddle.net/LPZvLa)

AutoSave audit in your database

{% include template-example.html %} 
```csharp
AuditManager.DefaultConfiguration.AutoSavePreAction = (context, audit) =>
    (context as EntityContext).AuditEntries.AddRange(audit.Entries);

```
[Try it in EF6](https://dotnetfiddle.net/rRORzM) | [Try it in EF Core](https://dotnetfiddle.net/DeLrTJ)

***Support:** EF5, EF6, EF Core*

[Learn more](/audit)
