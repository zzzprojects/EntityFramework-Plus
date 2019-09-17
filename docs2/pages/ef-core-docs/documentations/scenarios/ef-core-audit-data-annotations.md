---
Permalink: ef-core-data-annotations
---

# Data Annotations

## Problem

You want to use DataAnnotations.

## Solution

Following DataAnnotations is available:

 - AuditDisplay
 - AuditExclude
 - AuditInclude

However, in order to make them work, you must enable DataAnnotations

**Example Class**

{% include template-example.html %} 
```csharp

[AuditInclude]
[AuditDisplay("MyCustomEntityName")]
public class EntitySimple : IEntitySimple
{
	[AuditDisplay("MyCustomPropertyTable")]
	public int CompanyId { get; set; }
	
	[AuditExclude]
	public int Column1 { get; set;}
}

```
[Try it](https://dotnetfiddle.net/0ffKls)

**Example Enable DataAnnotations**

{% include template-example.html %} 
```csharp

AuditManager.DefaultConfiguration.ExcludeDataAnnotation();
AuditManager.DefaultConfiguration.DataAnnotationDisplayName();

```
