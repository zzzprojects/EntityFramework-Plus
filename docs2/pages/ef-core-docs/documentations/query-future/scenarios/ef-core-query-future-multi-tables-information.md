---
Permalink: ef-core-query-future-multi-tables-information
---

# Multi tables information scenario 

Client and all related information (order, invoice, etc.) must be loaded.

{% include template-example.html %} 
```csharp

// using Z.EntityFramework.Plus; // Don't forget to include this.
var ctx = new EntitiesContext();

var futureClient = ctx.Clients.DeferredFirst(x => x.ClientID = myClientID)
                                 .FutureValue();
var futureOrders = ctx.Orders.Where(x => x.ClientID = myClientID).Future();
var futureOrderDetails = ctx.OrderDetails.Where(x => x.ClientID = myClientID).Future();
var futureInvoices = ctx.Invoices.Where(x => x.ClientID = myClientID).Future();

// ONE database round trip is required
var client = futureClient.Value;
var orders = futureOrders.ToList();

```
