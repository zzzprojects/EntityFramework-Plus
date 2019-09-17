---
Permalink: ef-core-audit-autosave
---

# AutoSave Audit

## Problem

You need to automatically save audit entries in the database to keep a history in an audit table.

## Solution

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
[Try it](https://dotnetfiddle.net/wi8men)

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
[Try it](https://dotnetfiddle.net/RZeTZq)

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
[Try it](https://dotnetfiddle.net/JRiebw)

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
[Try it](https://dotnetfiddle.net/FtG10X)

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
