---
Permalink: ef6-query-filter-logical-data-partitioning
---

# Logical Data Partitioning

A common scenario is to retrieve products by category or the ones available only for a specific country. All data are stored in the same table but only a specific range should be available.

*In this example, we retrieve only the products available for the selected category.*

**Single category by product**

{% include template-example.html %} 
```csharp

// myCategoryID = 9

// using Z.EntityFramework.Plus; // Don't forget to include this.
var ctx = new EntitiesContext();
ctx.Filter<Product>(q => q.Where(x => x.CategoryID == myCategoryID));

// SELECT * FROM Product WHERE CategoryID = 9
var list = ctx.Products.ToList();

```

[Try it](https://dotnetfiddle.net/IZhSC0)

**Many categories by product**

{% include template-example.html %} 
```csharp

// myCategoryID = 9

// using Z.EntityFramework.Plus; // Don't forget to include this.
var ctx = new EntitiesContext();
ctx.Filter<Product>(x => ctx.ProductByCategory.Any(
                          y => y.CategoryID == myCategoryID 
                               && y.ProductID == x.ProductID))

// SELECT * FROM Product AS X WHERE EXISTS
//   (SELECT 1 FROM ProductByCategory AS Y 
//        WHERE Y.CategoryID = 9 AND Y.ProductID = X.ProductID)
var list = ctx.Products.ToList();

```
