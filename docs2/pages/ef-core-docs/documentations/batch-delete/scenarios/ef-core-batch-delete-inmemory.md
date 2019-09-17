---
Permalink: ef-core-batch-delete-inmemory
---

# EF Core InMemory

### Problem

You want to use **BatchDelete** with In Memory Testing for EF Core.

### Solution

Specify a DbContext factory in the **BatchDeleteManager** to enable BatchDelete with In Memory. A DbContext factory must be

{% include template-example.html %} 
```csharp

// Options
var db = new DbContextOptionsBuilder();
db.UseInMemoryDatabase();

// Specify InMemory DbContext Factory
BatchDeleteManager.InMemoryDbContextFactory = () => new MyContext(db.Options);

// Use the same code as with Relational Database
var _context = new MyContext(db.Options);
_context.Foos.Delete();

```
