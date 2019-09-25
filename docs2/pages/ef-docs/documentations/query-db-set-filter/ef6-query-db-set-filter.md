---
Permalink: query-db-set-filter
---

# Query DbSetFilter

## Definition

The **Query DbSetFilter** is the old filter API for Entity Framework 6 (EF6) before the code switched to use Interceptor instead.

The feature revamp was done in 2016 to support include and LazyLoading but, also have some limitation such as performance and instance filter that the old code didn't have.

## Limitations

 - **DO NOT** Support include
 - **DO NOT** Support lazy loading

## API Rename

To make both features live together, method in Query DbSetFilter has been renamed to add prefix with DbSet before Filter.

|Query Filter	|Query DbSetFilter |
|:------------- |:---------------- |
|AsNoFilter	|AsNoDbSetFilter|
|Filter	|DbSetFilter|
|QueryFilterManager	|QueryDbSetFilterManager|

## Options

 - [Global](options/ef6-query-db-set-filter-global.md)
 - [By Instance](options/ef6-query-db-set-filter-by-instance.md)
 - [By Query](options/ef6-query-db-set-filter-by-query.md)
 - [By Inheritance/Interface](options/ef6-query-db-set-filter-by-inheritance-interface.md)
 - [By Enable/Disable](options/ef6-query-db-set-filter-by-enable-disable.md)
 - [By AsNoFilter](options/ef6-query-db-set-filter-by-as-no-filter.md)
