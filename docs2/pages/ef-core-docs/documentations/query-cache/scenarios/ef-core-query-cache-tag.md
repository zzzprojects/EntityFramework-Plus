---
Permalink: ef-core-query-cache-tag
---

# EF+ Query Cache Tag & ExpireTag

Tagging the cache lets you expire later on all cached entries with a specific tag by calling the **ExpireTag** method.

For example, the daily countries & states importation has been completed and you need to refresh from the database all queries related to the country table. You can now simply expire the tag **countries** to remove all related cached entries.

{% include template-example.html %} 
```csharp

// using Z.EntityFramework.Plus; // Don't forget to include this.
var ctx = new EntitiesContext();

// Cache queries with related tags
var states = ctx.States.FromCache("countries", "states");
var stateCount = ctx.States.DeferredCount().FromCache("countries", "states", "stats");

// Expire all cache entries using the "countries" tag
QueryCacheManager.ExpireTag("countries");
:bulb: Use Enum instead of hard-coding string for tag name.

```
[Try it](https://dotnetfiddle.net/PUQCCY)
