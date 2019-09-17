---
Permalink: audit
---

# Audit

## Introduction

Entity Framework saves entities in a database but doesn't let you easily track changes, for example, a history of all modifications and their author in an audit table.

**EF+ Audit** easily tracks changes, exclude/include entity or property and auto save audit entries in the database.


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
[Try it](https://dotnetfiddle.net/AJVhpP)

## Scenarios

 - [AutoSave](scenarios/ef6-audit-autosave.md)
 - [Data Annotations](scenarios/ef6-audit-data-annotations.md)
 - [Exclude & Include Entity](scenarios/ef6-audit-exclude-include-entity.md)
 - [Exclude & Include Property](scenarios/ef6-audit-exclude-include-property.md)
 - [Format Value](scenarios/ef6-audit-format-value.md)
 - [Ignore Events](scenarios/ef6-audit-ignore-events.md)
 - [Property Unchanged](scenarios/ef6-audit-property-unchanged.md)
 - [Soft Add & Soft Delete](scenarios/ef6-audit-soft-add-soft-delete.md)
 - [Retrieve AuditEntries for specific item](scenarios/ef6-audit-retrieve-audit-entries-for-specific-item.md)
 - [Audit Customization](scenarios/ef6-audit-customization.md)
 - [Audit + Entity Framework Extensions](scenarios/ef6-audit-ef-extensions.md)
 
## Limitations

 - Entity Framework 7:
   - **DO NOT** support relationship
   - **DO NOT** populate EntitySetName value

## Requirements

 - **EF+ Audit:** Full version or Standalone version
 - **Database Provider:** All supported
 - **Entity Framework Version:** EF5, EF6, EF7
 - **Minimum Framework Version:** .NET Framework 4

## Troubleshooting

Why only my key is added when updating my entity?

This issue often happens for MVC user. They create a new entity through HttpPost values and force the state to "Modified", the context is not aware of the original value and use the current value instead. So, every property has the original value == current value and our auditing only log the key since all other values are equals.

We recommend setting the **IgnorePropertyUnchanged **to false to log every property.

Here is an example of this issue: [Issues #8](https://github.com/zzzprojects/EntityFramework-Plus/issues/8)

## Conclusion

**Auditing** in Entity Framework could not be simpler, there are always some entities in an application where an audit table can be crucial to keep track of what's happening, and you now have access to an easy to use library for these situations.

Need help getting started? [info@zzzprojects.com](mailto:info@zzzprojects.com)

We welcome all comments, ideas and suggestions to improve our library.
