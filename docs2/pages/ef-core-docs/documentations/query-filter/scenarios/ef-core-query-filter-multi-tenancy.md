---
Permalink: ef-core-query-filter-multi-tenancy
---

# Multi-Tenancy

An example of multi-tenancy is an online store for which the same instance of the database is used by multiple independent applications or clients and the data should not be shared between them.

Learn more about [Multi-tenancy](https://en.wikipedia.org/wiki/Multitenancy)

*In this example, the application is a tenant. The customer can only see invoice from the current application.*

{% include template-example.html %} 
```csharp

// myApplicationID = 9
// myCustomerID = 6

// using Z.EntityFramework.Plus; // Don't forget to include this.
var ctx = new EntitiesContext();
ctx.Filter<IApplication>(q => q.Where(x => x.ApplicationID == myApplicationID));

// SELECT * FROM Invoice WHERE ApplicationID = 9 and CustomerID= 6
var list = ctx.Invoices.Where(q => q.Where(x => x.CustomerID = myCustomerID)).ToList();

```

[Try it](https://dotnetfiddle.net/uIeV60)
