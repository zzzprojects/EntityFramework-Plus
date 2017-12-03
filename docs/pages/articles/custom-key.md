---
permalink: custom-key
---

## Problem
You want to perform a Bulk Operations (BulkUpdate, BulkDelete, or BulkMerge) but using a different key than the one specifying.

By example, you want to perform a BulkUpdate on a customer list but by using the customer "Code" instead of the "CustomerID" for the key.

## Solution
You can specify the column to use for the key with the **ColumnPrimaryKeyExpression** options.

### Example

{% include template-example.html %} 
{% highlight csharp %}
// Single Key
ctx.BulkUpdate(customers, operation => operation.ColumnPrimaryKeyExpression =
    customer => customer.Code);

// Surrogate Key (with anonymous type)
ctx.BulkUpdate(customers, operation => operation.ColumnPrimaryKeyExpression =
    customer => new { customer.Code1, customer.Code2, customer.Code3 });
{% endhighlight %}

## Troubleshooting

### Duplicate Key
You receive the following error:
>  An error occurred because the primary key specified is not unique, you can set the property 'AllowDuplicateKeys' to true to allow duplicate key

This error happens because the key is used in more than one row in the source

#### Example 

| Code | Name  |
| :--- | :---- |
| 001  | Jon |
| 002  | Henri |
| 001  | Jonathan |

The code "001" is here more than once with a different name.

You can allow duplicate key with the **AllowDuplicateKeys** options

{% include template-example.html %} 
{% highlight csharp %}
ctx.BulkUpdate(customers, operation =>
{
    operation.AllowDuplicateKeys = true;
    operation.ColumnPrimaryKeyExpression = customer => customer.Code;
});
{% endhighlight %}
