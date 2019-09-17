---
Permalink: ef6-batch-delete-executing-interceptor
---

# Executing Interceptor

### Problem

You need to log the DbCommand information or change the CommandText, Connection or Transaction before the batch is executed.

### Solution

The **Executing** property intercepts the DbCommand with an action before being executed.


{% include template-example.html %} 
```csharp

// using Z.EntityFramework.Plus; // Don't forget to include this.

string commandText
var date = DateTime.Now.AddYears(-2);
ctx.Users.Where(x => x.LastLoginDate < date)
         .Delete(x => { x.Executing = command => commandText = command.CommandText; });

```
[Try it](https://dotnetfiddle.net/VOEdOD)
