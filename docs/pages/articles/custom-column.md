---
permalink: custom-column
---

## Problem
You want to perform a Bulk Operations (BulkInsert, BulkUpdate, BulkDelete, or BulkMerge) but only on some specific column.

By example, you want to perform a BulkUpdate on a customer list but only UPDATE the name and email property.

## Solution
You can specify the column on which the operation should be performed with the **ColumnInputExpression** options

### Example

{% include template-example.html %} 
{% highlight csharp %}
// DON'T add the key if auto-generated
ctx.BulkInsert(customers, operation => operation.ColumnInputExpression =
    customer => new {customer.Name, customer.Email});

// ALWAYS add the key
ctx.BulkUpdate(customers, operation => operation.ColumnInputExpression =
    customer => new { customer.ID, customer.Name, customer.Email });

// ALWAYS add the key
ctx.BulkMerge(customers, operation => operation.ColumnInputExpression =
    customer => new {customer.ID, customer.Name, customer.Email});
{% endhighlight %}

## Troubleshooting

### INSERT + Destination mapped more than once error
You receive the following error:
> One of your column has a destination mapped more than once. See the inner exception for details.

Make sure the key has not been added to the ColumnInputExpression.

### UPDATE or MERGE + No primary found
You receive the following error:
> An error occurred, no primary could be found or resolved.

Make sure the key has been added to the ColumnInputExpression.
