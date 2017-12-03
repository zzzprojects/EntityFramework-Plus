---
permalink: concurrency
---

## Problem
Your model have concurrency entity and you must resolve optimistic concurrency using a pattern.

Concurrency exception normally happen on:
- BulkSaveChanges
- BulkUpdate

## Solution - BulkSaveChanges
When an concurrency error happen, only the first entry in error is returned (exactly like SaveChanges).

There is three possible scenario:
- Database Wins (You keep database values)
- Client Wins (You keep current entity values)
- Custom Resolution (You merge properties from database and client entity)

### Database Wins
You keep database values.

{% include template-example.html %} 
{% highlight csharp %}
public void BulkSaveChanges_DatabaseWins(DbContext ctx)
{
    bool saveFailed;

    do
    {
        saveFailed = false;

        try
        {
            ctx.BulkSaveChanges();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            saveFailed = true;

            // Update the values of the entity that failed to save from the store 
            ex.Entries.Single().Reload();
        }

    } while (saveFailed); 
}
{% endhighlight %}

### Client Wins
You keep current entity values.

{% include template-example.html %} 
{% highlight csharp %}
public void BulkSaveChanges_ClientWins(DbContext ctx)
{
    bool saveFailed;

    do
    {
        saveFailed = false;

        try
        {
            ctx.BulkSaveChanges();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            saveFailed = true;

            // Update original values from the database 
            var entry = ex.Entries.Single();
            entry.OriginalValues.SetValues(entry.GetDatabaseValues()); 
        }

    } while (saveFailed); 
}
{% endhighlight %}

### Custom Resolution
You merge properties from database and client entity.

{% include template-example.html %} 
{% highlight csharp %}
public void BulkSaveChanges_CustomResolution(CurrentContext ctx)
        {
public void BulkSaveChanges_CustomResolution(CurrentContext ctx)
{

    bool saveFailed;

    do
    {
        saveFailed = false;

        try
        {
            ctx.BulkSaveChanges();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            saveFailed = true;

            // Get the current entity values and the values in the database 
            // as instances of the entity type 
            var entry = ex.Entries.Single();
            var databaseValues = entry.GetDatabaseValues();

            if (entry.Entity is EntitySimple_Concurrency)
            {
                var clientEntity = (EntitySimple_Concurrency) entry.Entity;
                var databaseEntity = (EntitySimple_Concurrency) databaseValues.ToObject();

                // Choose an initial set of resolved values. In this case we 
                // make the default be the values currently in the database. 
                var resolvedEntity = (EntitySimple_Concurrency) databaseValues.ToObject();

                // Have the user choose what the resolved values should be
                resolvedEntity.IntColumn = clientEntity.IntColumn + 100;
                // ... merge all columns...

                // Update the original values with the database values and 
                // the current values with whatever the user choose. 
                entry.OriginalValues.SetValues(databaseValues);
                entry.CurrentValues.SetValues(resolvedEntity);
            }
        }

    } while (saveFailed);
}
{% endhighlight %}

## Solution - BulkUpdate
When an concurrency error happen, BulkUpdate return a **DbBulkOperationConcurrencyException** which contains all entries in error.

There is three possible scenario:
- Database Wins (You keep database values)
- Client Wins (You keep current entity values)
- Custom Resolution (You merge properties from database and client entity)

### Database Wins
You keep database values.

{% include template-example.html %} 
{% highlight csharp %}
public void BulkUpdate_DatabaseWins<T>(CurrentContext ctx, List<T> list) where T : class
{
    try
    {
        ctx.BulkUpdate(list);
    }
    catch (DbBulkOperationConcurrencyException ex)
    {
        // DO nothing (or log), keep database values!
    }
}
{% endhighlight %}

### Client Wins
You keep current entity values.

{% include template-example.html %} 
{% highlight csharp %}
public void BulkUpdate_StoreWins<T>(CurrentContext ctx, List<T> list) where T : class
{
    try
    {
        ctx.BulkUpdate(list);
    }
    catch (DbBulkOperationConcurrencyException ex)
    {
        // FORCE update store entities
        ctx.BulkUpdate(list, operation => operation.AllowConcurrency = false);
    }
}
{% endhighlight %}

### Custom Resolution
You merge properties from database and client entity.

{% include template-example.html %} 
{% highlight csharp %}
public void BulkUpdate_CustomResolution<T>(CurrentContext ctx, List<T> list) where T : class
{
    try
    {
        ctx.BulkUpdate(list);
    }
    catch (DbBulkOperationConcurrencyException ex)
    {
        foreach (var entry in ex.Entries)
        {
            ObjectStateEntry objectEntry;

            if (entry is EntitySimple_Concurrency)
            {
                var clientEntry = (EntitySimple_Concurrency) entry;
                var databaseEntry = ctx.EntitySimple_Concurrencys.Single(x => x.ID == clientEntry.ID);

                // merge properties like you want
                clientEntry.IntColumn = databaseEntry.IntColumn;
            }
        }

        // FORCE update store entities
        ctx.BulkUpdate(list, operation => operation.AllowConcurrency = false);
    }
}
{% endhighlight %}
