---
Permalink: ef-core-query-cache-expiration
---

# EF+ Query Cache Expiration

All common caching features like absolute expiration, sliding expiration, removed callback are supported.

{% include template-example.html %} 
```csharp

// using Z.EntityFramework.Plus; // Don't forget to include this.
var ctx = new EntitiesContext();

// Make the query expire after 2 hours of inactivity

var options = new MemoryCacheEntryOptions() { SlidingExpiration = TimeSpan.FromHours(2)};

var states = ctx.States.FromCache(options);

```
[Try it](https://dotnetfiddle.net/aKPnTD)
