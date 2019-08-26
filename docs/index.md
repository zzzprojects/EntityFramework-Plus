---
layout: default
permalink: index
---

<!-- hero !-->
{% include component-rotator-dark-begin.html %}
<div class="hero">
	<div class="container">
		<div class="row">
			<div class="col-lg-5 hero-header">
				<h1>
					<div class="display-4">Entity Framework Plus</div>
				</h1>
				<div class="wow zoomIn">
					<a class="btn btn-xl btn-z" href="{{ site.github.url }}/download"
							onclick="ga('send', 'event', { eventAction: 'download'});">
						<i class="fa fa-cloud-download" aria-hidden="true"></i>
						NuGet Download
						<i class="fa fa-angle-right"></i>
					</a>
				</div>
				<div class="font-italic">
					*Free monthly trial available
				</div>
				<div class="download-count">
					<div class="item-text">Download Count:</div>
					<div class="item-image wow lightSpeedIn"><img src="https://zzzprojects.github.io/images/nuget/ef6-full-version-big-d.svg" /></div>
				</div>
			</div>
			<div class="col-lg-7 hero-examples">
				<div class="row hero-examples-1">
					<div class="col-lg-3 wow slideInUp"> 
						<h3 class="wow rollIn">EASY TO<br />USE</h3>
						<div class="hero-arrow hero-arrow-ltr">
							<img src="images/arrow-down1.png">
						</div>
					</div>
					<div class="col-lg-9 wow slideInRight">
						<div class="card card-code card-code-light">
    <div class="card-header"><h5>Extend Entity Framework DbContext</h5></div>
    <div class="card-body">
    {% highlight csharp %}
// DELETE all users which are inactive for 2 years
ctx.Users.Where(x => 
x.LastLoginDate < DateTime.Now.AddYears(-2))
.Delete();

// UPDATE all users which are inactive for 2 years
ctx.Users.Where(x => 
x.LastLoginDate < DateTime.Now.AddYears(-2))
.Update(x => new User() { IsSoftDeleted = 1 });

{% endhighlight %}
    </div>
</div>

					</div>
				</div>
				<div class="row hero-examples-2">
					<div class="col-lg-3 order-lg-2 wow slideInDown">
						<h3 class="wow rollIn">EASY TO<br />CUSTOMIZE</h3>
						<div class="hero-arrow hero-arrow-rtl">
							<img src="images/arrow-down1.png">
						</div>
					</div>
					<div class="col-lg-9 order-lg-1 wow slideInLeft">
						<div class="card card-code card-code-light">
    <div class="card-header"><h5>Zero configuration required</h5></div>
    <div class="card-body">
{% highlight csharp %}
// DELETE using a BatchSize
ctx.Users.Where(x => 
x.LastLoginDate < DateTime.Now.AddYears(-2))
.Delete(x => x.BatchSize = 1000);

//LINQ Dynamic
var list = ctx.SelectDynamic(x => 
"new { y = x + 1 }").ToList();
var list = ctx.SelectDynamic(x => 
"new { y = x + 1 }", 
new { y = 1 }).ToList();
{% endhighlight %}
    </div>
</div>
					</div>						
				</div>
			</div>
		</div>
	</div>	
</div>
{% include component-rotator-dark-end.html %}
<!-- features !-->
<div class="features">
	<div class="container">
		<!-- Auditing !-->
		<h2 class="wow slideInUp">Auditing</h2>
		<div class="row">
			<div class="col-lg-5 wow slideInLeft">
				<p class="feature-tagline">Improve <span class="text-z">security</span> and know what, when and who did a changes in the context.</p>
				<ul>
					<li>AutoSave audit values in the database</li>
					<li>Keep track of SoftDelete entities</li>
					<li>Filter events you desire</li>
                  <li>Include entity types you desire</li>
                  <li>Format values in your specific format</li>
				</ul>
				<div class="more-info">
					<a href="{{ site.github.url }}/tutorial-audit" class="btn btn-lg btn-z" role="button">
						<i class="fa fa-book"></i>&nbsp;
						Read More
					</a>
				</div>	
			</div>
			<div class="col-lg-7 wow slideInRight">
				<div class="card card-code card-code-dark">
					<div class="card-header"><h5>Auditing Example</h5></div>
					<div class="card-body">
{% highlight csharp %}
var audit = new Audit();
audit.CreatedBy = "ZZZ Projects"; // Optional
ctx.SaveChanges(audit);

// Access to all auditing information
var entries = audit.Entries;
foreach(var entry in entries)
{
    foreach(var property in entry.Properties)
    {
    }
}
{% endhighlight %}
					</div>
				</div>
			</div>
		</div>

		<hr class="m-y-md" />
		
		<!-- Delete without loading entities !-->
		<h2 class="wow slideInUp">Delete without loading entities</h2>
		<div class="row">
			<div class="col-lg-5 wow slideInLeft">
				<p class="feature-tagline">Delete rows from LINQ Query in a single database round trip without loading entities in the context.</p>
				<ul>
					<li>Use Async methods to make your application responsive</li>
					<li>Use batch size to improve performance</li>
					<li>Use batch delay interval to reduce server load</li>
					<li>Use Intercept to customize DbCommand</li>
				</ul>
				<div class="more-info">
					<a href="{{ site.github.url }}/tutorial-batch-operations" class="btn btn-lg btn-z" role="button">
						<i class="fa fa-book"></i>&nbsp;
						Read More
					</a>
				</div>	
			</div>
			<div class="col-lg-7 wow slideInRight">
				<div class="card card-code card-code-dark">
					<div class="card-header"><h5>Delete Example</h5></div>
					<div class="card-body">
{% highlight csharp %}

/ DELETE all users inactive for 2 years
var date = DateTime.Now.AddYears(-2);
ctx.Users.Where(x => x.LastLoginDate < date)
         .Delete();

// DELETE using a BatchSize
var date = DateTime.Now.AddYears(-2);
ctx.Users.Where(x => x.LastLoginDate < date)
         .Delete(x => x.BatchSize = 1000);
{% endhighlight %}	
					</div>
				</div>
			</div>
		</div>
		
		<hr class="m-y-md" />
		
		<!-- Update without loading entities !-->
		<h2 class="wow slideInUp">Update without loading entities</h2>
		<div class="row">
			<div class="col-lg-5 wow slideInLeft">
				<p class="feature-tagline">Update rows from LINQ Query in a single database round trip without loading entities in the context.</p>
				<ul>
					<li>Use Async methods to make your application responsive</li>
					<li>Use Intercept to customize DbCommand</li>
				</ul>
				<div class="more-info">
					<a href="{{ site.github.url }}/tutorial-batch-operations#batch-update" class="btn btn-lg btn-z" role="button">
						<i class="fa fa-book"></i>&nbsp;
						Read More
					</a>
				</div>	
			</div>
			<div class="col-lg-7 wow slideInRight">
				<div class="card card-code card-code-dark">
					<div class="card-header"><h5>Update Example</h5></div>
					<div class="card-body">
{% highlight csharp %}
// UPDATE all users inactive for 2 years
var date = DateTime.Now.AddYears(-2);
ctx.Users.Where(x => x.LastLoginDate < date)
         .Update(x => new User() { IsSoftDeleted = 1 });
{% endhighlight %}	
					</div>
				</div>
			</div>
		</div>

        <hr class="m-y-md" />

        <!-- Second Level Cache !-->
		<h2 class="wow slideInUp">Second Level Cache</h2>
		<div class="row">
			<div class="col-lg-5 wow slideInLeft">
				<p class="feature-tagline">Improve application <span class="text-z">performance</span> and reduce sql server load by using a second level caching.</p>
				<ul>
					<li>Use Cache Tag to expire cache</li>
					<li>Use Cache Policy to control caching</li>
				</ul>
				<div class="more-info">
					<a href="{{ site.github.url }}/tutorial-query" class="btn btn-lg btn-z" role="button">
						<i class="fa fa-book"></i>&nbsp;
						Read More
					</a>
				</div>	
			</div>
			<div class="col-lg-7 wow slideInRight">
				<div class="card card-code card-code-dark">
					<div class="card-header"><h5>Cache Example</h5></div>
					<div class="card-body">
{% highlight csharp %}
// The first call perform a database round trip
var countries1 = ctx.Countries.FromCache().ToList();

// Subsequent calls will take the value from the memory instead
var countries2 = ctx.Countries.FromCache().ToList();
{% endhighlight %}	
					</div>
				</div>
			</div>
		</div>

       <hr class="m-y-md" />

        <!-- Filtering !-->
		<h2 class="wow slideInUp">Filtering</h2>
		<div class="row">
			<div class="col-lg-5 wow slideInLeft">
				<p class="feature-tagline">Improve your context <span class="text-z">extensibility</span> and create filters to query only what's really available.</p>
				<ul>
					<li>Create Multi-Tenancy application</li>
					<li>Exclude soft deleted record</li>
                  <li>Filter record by security access</li>
				</ul>
				<div class="more-info">
					<a href="{{ site.github.url }}/tutorial-query#query-filter" class="btn btn-lg btn-z" role="button">
						<i class="fa fa-book"></i>&nbsp;
						Read More
					</a>
				</div>	
			</div>
			<div class="col-lg-7 wow slideInRight">
				<div class="card card-code card-code-dark">
					<div class="card-header"><h5>Filtering Example</h5></div>
					<div class="card-body">
{% highlight csharp %}
QueryFilterManager.Filter<Post>(q => q.Where(x => !x.IsSoftDeleted));

var ctx = new EntitiesContext();

// SELECT * FROM Post WHERE IsSoftDeleted = false
var list = ctx.Posts.ToList();
{% endhighlight %}	
					</div>
				</div>
			</div>
		</div>

       <hr class="m-y-md" />

        <!-- Future & FutureValue !-->
		<h2 class="wow slideInUp">Future & FutureValue</h2>
		<div class="row">
			<div class="col-lg-5 wow slideInLeft">
				<p class="feature-tagline">Delay queries execution and batch all queries in a single database round trip.</p>
				<ul>
					<li>Query Future</li>
					<li>Query Future Value</li>
					<li>Query Future Value Deferred</li>
					<li>Include entity type you desire</li>
					<li>Format value in your specific format</li>
				</ul>
				<div class="more-info">
					<a href="{{ site.github.url }}/tutorial-query#query-future" class="btn btn-lg btn-z" role="button">
						<i class="fa fa-book"></i>&nbsp;
						Read More
					</a>
				</div>	
			</div>
			<div class="col-lg-7 wow slideInRight">
				<div class="card card-code card-code-dark">
					<div class="card-header"><h5>FutureValue Example</h5></div>
					<div class="card-body">
{% highlight csharp %}
// CREATE a pending list of future queries
var futureCountries = db.Countries.Where(x => x.IsActive).Future();
var futureStates = db.States.Where(x => x.IsActive).Future();

// TRIGGER all pending queries in one database round trip
// SELECT * FROM Country WHERE IsActive = true;
// SELECT * FROM State WHERE IsActive = true
var countries = futureCountries.ToList();

// futureStates is already resolved and contains the result
var states = futureStates.ToList();
{% endhighlight %}	
					</div>
				</div>
			</div>
		</div>

       <hr class="m-y-md" />

       <!-- Filter included related entities !-->
		<h2 class="wow slideInUp">Filter included related entities</h2>
		<div class="row">
			<div class="col-lg-5 wow slideInLeft">
				<p class="feature-tagline">Overcome Include method limitations by filtering included related entities.</p>
				<ul>
					<li>Filter child entities with IncludeFilter</li>
					<li>Improve performance with IncludeOptimized</li>
				</ul>
				<div class="more-info">
					<a href="{{ site.github.url }}/tutorial-query#query-includefilter" class="btn btn-lg btn-z" role="button">
						<i class="fa fa-book"></i>&nbsp;
						Read More
					</a>
				</div>	
			</div>
			<div class="col-lg-7 wow slideInRight">
				<div class="card card-code card-code-dark">
					<div class="card-header"><h5>Filter Example</h5></div>
					<div class="card-body">
{% highlight csharp %}
// LOAD orders and the first 10 active related entities.
var list = ctx.Orders.IncludeFilter(x => x.Items
            .Where(y => !y.IsSoftDeleted)
            .OrderBy(y => y.Date)
            .Take(10))
            .ToList();
{% endhighlight %}	
					</div>
				</div>
			</div>
		</div>
	</div>
</div>
