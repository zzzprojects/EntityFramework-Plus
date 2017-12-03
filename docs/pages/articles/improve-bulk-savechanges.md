---
permalink: improve-bulk-savechanges
---

## Introduction
BulkSaveChanges is already very fast. But you can make it even faster by simply turning off the "EntityFrameworkPropagation" options.

While the performance will be significantly increased, this option works with  99,9% of models. Unfortunately, we cannot turn this options by default for backward compatibility until we make it work with all models

We are currently working on the next major version which will have performance even better!

[Learn - Why turning off Entity Framework Propagation is faster](#why-turning-off-entity-framework-propagation-is-faster)

### Example - Globally
{% include template-example.html %} 
{% highlight csharp %}
EntityFrameworkManager.DefaultEntityFrameworkPropagationValue = false;
{% endhighlight %}

### Example - By Instance
{% include template-example.html %} 
{% highlight csharp %}
var ctx = new EntitiesContext();

ctx.Customers.AddRange(listToAdd);
ctx.Customers.RemoveRange(listToRemove);
listToModify.ForEach(x => x.DateModified = DateTime.Now);

// Easy to use
ctx.BulkSaveChanges(false);
{% endhighlight %}

### Performance Comparisons

| Operations      | 1,000 Entities | 2,000 Entities | 5,000 Entities |
| :-------------- | -------------: | -------------: | -------------: |
| SaveChanges            | 1,000 ms       | 2,000 ms       | 5,000 ms       |
| BulkSaveChanges()      | 90 ms          | 150 ms         | 350 ms         |
| BulkSaveChanges(false) | 60 ms          | 70 ms          | 140 ms         |

For SQL Server, performance improvment is around 2x faster.

For some provider like SQLite, performance improvement can be as high as 10x faster.

### Unsupported Scenario

- There is too much cross reference table, and the library is not able to create a saving strategy. You will receive an error on the first use.
- An entity uses a temporary generated GUID when adding (not empty), but the GUID is replaced later by the GUID generated in the database. Data will be correctly inserted, but entities may still have the temporary GUID for relation not using navigation property.

> These limitations will be fixed in the next major version.

## Why turning off Entity Framework Propagation is faster
Unfortunately, Entity Framework is very slow at generating commands to be executed. For some provider, it takes more time to generate these queries than executing them!

When turning off, the library does not longer use the method from Entity Framework but internal method from our library.
