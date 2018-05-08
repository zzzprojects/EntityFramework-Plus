---
permalink: api
---
@{
    ViewBag.MarkdownEnabled = false;
}
<div class="row">
    <div class="col">
        <div class="card card-z">
            <div class="card-header">Batch Operations</div>
            <div class="card-body">
                <a class="navbar-brand" href="{{ site.github.url }}/batch-delete">
                    Batch Delete
                </a>
                <a class="navbar-brand" href="{{ site.github.url }}/batch-update">
                    Batch Update
                </a>
            </div>
        </div>
    </div>
    <div class="col">
        <div class="card card-z">
            <div class="card-header">LINQ</div>
            <div class="card-body">
                <a class="navbar-brand" href="{{ site.github.url }}/linq-dynamic">
                    LINQ Dynamic
                </a>
            </div>
        </div>
    </div>
    <div class="col">
        <div class="card  card-z">
            <div class="card-header">Audit</div>
            <div class="card-body">
                <a class="navbar-brand" href="{{ site.github.url }}/audit">
                    Audit
                </a>
            </div>
        </div>
    </div>
    <div class="col">
        <div class="card card-z">
            <div class="card-header">Query</div>
            <div class="card-body">
                <a class="navbar-brand" href="{{ site.github.url }}/query-cache">
                    Query Cache
                </a>
                <a class="navbar-brand" href="{{ site.github.url }}/query-deferred">
                    Query Deferred
                </a>
                <a class="navbar-brand" href="{{ site.github.url }}/query-db-set-filter">
                    Query DbSetFilter
                </a>
                <a class="navbar-brand" href="{{ site.github.url }}/query-future">
                    Query Future
                </a>
                <a class="navbar-brand" href="{{ site.github.url }}/query-include-filter">
                    Query IncludeFilter
                </a>
                <a class="navbar-brand" href="{{ site.github.url }}/query-include-optimized">
                    Query IncludeOptimized
                </a>
            </div>
        </div>
    </div>
</div>