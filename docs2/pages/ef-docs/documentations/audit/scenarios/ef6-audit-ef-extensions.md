---
Permalink: ef6-audit-ef-extensions
---

# Audit + Entity Framework Extensions

If you are using Entity Framework Extensions, it is still possible to use the EF+ Audit features.

However, it will only work with BulkSaveChanges.

Other Bulk Operations (BulkInsert, BulkUpdate, BulkDelete, and BulkMerge) doesn't use the Change Tracker so, there is nothing to track!

## BulkSaveChanges + AutoSave

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
[Try it](https://dotnetfiddle.net/AREtca)
