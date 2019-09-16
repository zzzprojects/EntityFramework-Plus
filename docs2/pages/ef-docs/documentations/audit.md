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
[Try it in EF6](https://dotnetfiddle.net/AJVhpP) | [Try it in EF Core](https://dotnetfiddle.net/dc7v3W)

## AutoSave Audit

### Problem

You need to automatically save audit entries in the database to keep a history in an audit table.

### Solution

If an action for the property **AutoSavePreAction** is set, audit entries will automatically be saved in the database when **SaveChanges** or **SaveChangesAsync** methods are called.

***By using EF+ Audit entity***

{% include template-example.html %} 
```csharp

public class EntityContext : DbContext
{
   // ... context code ...
   public DbSet<AuditEntry> AuditEntries { get; set; }
   public DbSet<AuditEntryProperty> AuditEntryProperties { get; set; }

   static EntityContext()
   {
      AuditManager.DefaultConfiguration.AutoSavePreAction = (context, audit) =>
         // ADD "Where(x => x.AuditEntryID == 0)" to allow multiple SaveChanges with same Audit
         (context as EntityContext).AuditEntries.AddRange(audit.Entries);
   }
}

var audit = new Audit();
audit.CreatedBy = "ZZZ Projects"; // Optional
ctx.SaveChanges(audit);

```
[Try it in EF6](https://dotnetfiddle.net/BfkqWm) | [Try it in EF Core](https://dotnetfiddle.net/wi8men)

***By using a different context***

{% include template-example.html %} 
```csharp

public class OracleContext : DbContext
{
   // ... context code ...
}

public class SqlServerContext : DbContext
{
   // ... context code ...
   public DbSet<AuditEntry> AuditEntries { get; set; }
   public DbSet<AuditEntryProperty> AuditEntryProperties { get; set; }
}

AuditManager.DefaultConfiguration.AutoSavePreAction = (context, audit) =>
{
	var sqlServerContext = new SqlServerContext();
	sqlServerContext.AuditEntries.AddRange(audit.Entries);
	sqlServerContext.SaveChanges();
};

var audit = new Audit();
audit.CreatedBy = "ZZZ Projects"; // Optional
oracleContext.SaveChanges(audit);

```
[Try it in EF6](https://dotnetfiddle.net/wTwh77) | [Try it in EF Core](https://dotnetfiddle.net/RZeTZq)

***Custom AuditEntry & Database First Approach***

{% include template-example.html %} 
```csharp

AuditManager.DefaultConfiguration.AutoSavePreAction = (context, audit) =>
{
    // ADD "Where(x => x.AuditEntryID == 0)" to allow multiple SaveChanges with same Audit
	var customAuditEntries = audit.Entries.Select(x => Import(x));
	(context as Entities).AuditEntries.AddRange(customAuditEntries);
};

using (var ctx = new Entities())
{
    Audit audit = new Audit();
    audit.CreatedBy = "ZZZ Projects"; // Optional

    ctx.Entity_Basic.Add(new Entity_Basic() {ColumnInt = 2});
    ctx.SaveChanges(audit);
}

public Static AuditEntry Import(Z.EntityFramework.Plus.AuditEntry entry)
{
    var customAuditEntry = new AuditEntry
    {
        EntitySetName = entry.EntitySetName,
        EntityTypeName = entry.EntityTypeName,
        State = (int)entry.State,
        StateName = entry.StateName,
        CreatedBy = entry.CreatedBy,
        CreatedDate = entry.CreatedDate
    };

    customAuditEntry.AuditEntryProperties = entry.Properties.Select(x => Import(x)).ToList();

    return customAuditEntry;
}

public Static AuditEntryProperty Import(Z.EntityFramework.Plus.AuditEntryProperty property)
{
    var customAuditEntry = new AuditEntryProperty
    {
        RelationName = property.RelationName,
        PropertyName = property.PropertyName,
        OldValue = property.OldValueFormatted,
        NewValue = property.NewValueFormatted
    };

    return customAuditEntry;
}

```
[Try it in EF6](https://dotnetfiddle.net/9XjCSM) | [Try it in EF Core](https://dotnetfiddle.net/JRiebw)

***Saving automatically by overriding SaveChanges & SaveChangesAsync***

{% include template-example.html %} 
```csharp

AuditManager.DefaultConfiguration.AutoSavePreAction = (context, audit) =>
    // ADD "Where(x => x.AuditEntryID == 0)" to allow multiple SaveChanges with same Audit
   (context as TestContext).AuditEntries.AddRange(audit.Entries);
	
public class EntityContext : DbContext
{
	// ... context code ...
	
	public override int SaveChanges()
	{
		var audit = new Audit();
		audit.PreSaveChanges(this);
		var rowAffecteds = base.SaveChanges();
		audit.PostSaveChanges();

		if (audit.Configuration.AutoSavePreAction != null)
		{
			audit.Configuration.AutoSavePreAction(this, audit);
			base.SaveChanges();
		}

		return rowAffecteds;
	}

	public override Task<int> SaveChangesAsync()
	{
		return SaveChangesAsync(CancellationToken.None);
	}

	public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
	{
		var audit = new Audit();
		audit.PreSaveChanges(this);
		var rowAffecteds = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
		audit.PostSaveChanges();

		if (audit.Configuration.AutoSavePreAction != null)
		{
			audit.Configuration.AutoSavePreAction(this, audit);
			await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
		}

		return rowAffecteds;
	}
}

using(var ctx = new EntityContext())
{
	// ... code ...
	ctx.SaveChanges();
}

```
[Try it in EF6](https://dotnetfiddle.net/TFf8aj) | [Try it in EF Core](https://dotnetfiddle.net/FtG10X)

***SQL Script (for Database First)***

{% include template-example.html %} 
```csharp

CREATE TABLE [dbo].[AuditEntries] (
    [AuditEntryID] [int] NOT NULL IDENTITY,
    [EntitySetName] [nvarchar](255),
    [EntityTypeName] [nvarchar](255),
    [State] [int] NOT NULL,
    [StateName] [nvarchar](255),
    [CreatedBy] [nvarchar](255),
    [CreatedDate] [datetime] NOT NULL,
    CONSTRAINT [PK_dbo.AuditEntries] PRIMARY KEY ([AuditEntryID])
)

GO

CREATE TABLE [dbo].[AuditEntryProperties] (
    [AuditEntryPropertyID] [int] NOT NULL IDENTITY,
    [AuditEntryID] [int] NOT NULL,
    [RelationName] [nvarchar](255),
    [PropertyName] [nvarchar](255),
    [OldValue] [nvarchar](max),
    [NewValue] [nvarchar](max),
    CONSTRAINT [PK_dbo.AuditEntryProperties] PRIMARY KEY ([AuditEntryPropertyID])
)

GO

CREATE INDEX [IX_AuditEntryID] ON [dbo].[AuditEntryProperties]([AuditEntryID])

GO

ALTER TABLE [dbo].[AuditEntryProperties] 
ADD CONSTRAINT [FK_dbo.AuditEntryProperties_dbo.AuditEntries_AuditEntryID] 
FOREIGN KEY ([AuditEntryID])
REFERENCES [dbo].[AuditEntries] ([AuditEntryID])
ON DELETE CASCADE

GO

```

## Data Annotations

### Problem

You want to use DataAnnotations.

### Solution

Following DataAnnotations is available:

 - AuditDisplay
 - AuditExclude
 - AuditInclude

However, in order to make them work, you must enable DataAnnotations

**Example Class**

{% include template-example.html %} 
```csharp

[AuditInclude]
[AuditDisplay("MyCustomEntityName")]
public class EntitySimple : IEntitySimple
{
	[AuditDisplay("MyCustomPropertyTable")]
	public int CompanyId { get; set; }
	
	[AuditExclude]
	public int Column1 { get; set;}
}

```
[Try it in EF6](https://dotnetfiddle.net/QPcF2a) | [Try it in EF Core](https://dotnetfiddle.net/0ffKls)

**Example Enable DataAnnotations**

{% include template-example.html %} 
```csharp

AuditManager.DefaultConfiguration.ExcludeDataAnnotation();
AuditManager.DefaultConfiguration.DataAnnotationDisplayName();

```

## Exclude & Include Entity

### Problem

You need to audit only a subset of your entities and you need to exclude/include by entity types, base class or interface.

### Solution

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
[Try it in EF6](https://dotnetfiddle.net/8TbEdO) | [Try it in EF Core](https://dotnetfiddle.net/Ky4SpL)

## Exclude & Include Property

### Problem

You need to audit only a subset of your properties and you need to exclude/include properties by name and by entity type, base class or interface.

### Solution

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
[Try it in EF6](https://dotnetfiddle.net/eHO1fP) | [Try it in EF Core](https://dotnetfiddle.net/E5fDhN)

## Format Value

### Problem

You need to format a value with a different string representation, for example, adding a dollar sign to the money value.

### Solution

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
[Try it in EF6](https://dotnetfiddle.net/Of9Dbz) | [Try it in EF Core](https://dotnetfiddle.net/PZ4CJ3)

## Ignore Events

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
[Try it in EF6](https://dotnetfiddle.net/Ya95EQ) | [Try it in EF Core](https://dotnetfiddle.net/w6D8z4)

## Property Unchanged

### Problem

You need to keep track of all changed and unchanged properties

### Solution

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
[Try it in EF6](https://dotnetfiddle.net/tYE0YR) | [Try it in EF Core](https://dotnetfiddle.net/6oWfr2)

## Soft Add & Soft Delete

### Problem

You need to keep an audit when a soft add/restore or a soft delete event happens.

### Solution

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
[Try it in EF6](https://dotnetfiddle.net/OadxfN) | [Try it in EF Core](https://dotnetfiddle.net/Etv7yC)

## Retrieve AuditEntries for specific item

### Problem

You want to retrieve all AuditEntry for a specific item.

### Solution

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
[Try it in EF6](https://dotnetfiddle.net/ETFLlO) | [Try it in EF Core](https://dotnetfiddle.net/6qiMrl)

### Audit, AuditEntry & AuditEntryProperty

AuditEntry and AuditEntryProperty can be added in your context to automatically save audit entries using the AutoSavePreAction property. Only mapped properties are mapped to your database.

### Audit

 - Properties
   - Configuration
   - CreatedBy
   - Entries
 - Methods
   - PostSaveChanges()
   - PreSaveChanges()

### AuditEntry

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

### AuditEntryProperty

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

## Audit Customization

### Problem

You want to customize some fields value like CreatedDate or even more, want to use your own class inheriting from AuditEntry && AuditEntryProperty?

### Solution

Use AuditEntryFactory && AuditEntryPropertyFactory

#### Customize CreatedDate


{% include template-example.html %} 
```csharp

// using Z.EntityFramework.Plus; // Don't forget to include this.

// Globally
AuditManager.DefaultConfiguration
            .AuditEntryFactory = args => 
			    new AuditEntry() { CreatedDate = DateTime.Now.AddHours(-5) };

// By Instance
var audit = new Audit();
audit.CreatedBy = "ZZZ Projects"; // Optional
audit.Configuration.AuditEntryFactory = args => 
                       new AuditEntry() {CreatedDate = DateTime.Now.AddHours(-5)};
ctx.SaveChanges(audit);

```
[Try it in EF6](https://dotnetfiddle.net/aVIC0C) | [Try it in EF Core](https://dotnetfiddle.net/uYAB7B)

#### Custom Class

{% include template-example.html %} 
```csharp

// using Z.EntityFramework.Plus; // Don't forget to include this.

// Globally
AuditManager.DefaultConfiguration
            .AuditEntryFactory = args => 
			    new CustomAuditEntry() { IpAdress = address };

AuditManager.DefaultConfiguration
            .AuditEntryPropertyFactory = args => 
			    new CustomAuditEntryProperty() { CustomField = value };

// By Instance
var audit = new Audit();
audit.CreatedBy = "ZZZ Projects"; // Optional
audit.Configuration.AuditEntryFactory = args => 
			    new CustomAuditEntry() { IpAdress = address };

audit.Configuration.AuditEntryPropertyFactory = args => 
			    new CustomAuditEntryProperty() { CustomField = value };
ctx.SaveChanges(audit);

```
[Try it in EF6](https://dotnetfiddle.net/xazeGj) | [Try it in EF Core](https://dotnetfiddle.net/ZV3lxd)

#### Custom DbSet with AutoSave

{% include template-example.html %} 
```csharp

public class EntityContext : DbContext
{
   // ... context code ...
   public DbSet<CustomAuditEntry> CustomAuditEntries { get; set; }
   public DbSet<CustomAuditEntryProperty> CustomAuditEntryProperties { get; set; }
}

AuditManager.DefaultConfiguration.AutoSavePreAction = (context, audit) =>
    // ADD "Where(x => x.AuditEntryID == 0)" to allow multiple SaveChanges with same Audit
    (context as EntityContext).CustomAuditEntries.AddRange(audit.Entries.Cast<CustomAuditEntry>());

var audit = new Audit();
audit.CreatedBy = "ZZZ Projects"; // Optional
ctx.SaveChanges(audit);

```
[Try it in EF6](https://dotnetfiddle.net/lC7eWg) | [Try it in EF Core](https://dotnetfiddle.net/k1A2OD)

## Audit + Entity Framework Extensions

If you are using Entity Framework Extensions, it is still possible to use the EF+ Audit features.

However, it will only work with BulkSaveChanges.

Other Bulk Operations (BulkInsert, BulkUpdate, BulkDelete, and BulkMerge) doesn't use the Change Tracker so, there is nothing to track!

### BulkSaveChanges + AutoSave

{% include template-example.html %} 
```csharp

public class CurrentContext : DbContext
{
	// ...code...
	
	public void BulkSaveChangesWithAudit()
	{
		var audit = new Audit();
		audit.PreSaveChanges(this);

		this.BulkSaveChanges();
		
		audit.PostSaveChanges();

		if (audit.Entries.Count > 0)
		{
			this.BulkInsert(audit.Entries);

			// The ID must be set, there is currently no navigation properties in the AuditEntryProperty class
			audit.Entries.ForEach(x => x.Properties.ForEach(y => y.AuditEntryID = x.AuditEntryID));

			// Don't output the Id for EntryProperty for more performance
			this.BulkInsert(audit.Entries.SelectMany(y => y.Properties), operation =>
			{
				operation.AutoMapOutputDirection = false;
				operation.BatchSize = 50000;
			});
		}
	}
}

```
[Try it in EF6](https://dotnetfiddle.net/AREtca) | [Try it in EF Core](https://dotnetfiddle.net/nWXn84)

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
