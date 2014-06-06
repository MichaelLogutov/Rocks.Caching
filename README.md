Rocks.Caching
=============

Rocks.Caching is a library that aimed to provide clean minimal abstraction over caching mechanics for it to be unit test and refactor friendly.

## ICacheProvider root interface
The root interface is [ICacheProvider](https://github.com/MichaelLogutov/Rocks.Caching/blob/master/src/Rocks.Caching/ICacheProvider.cs) which provides minimal method set for caching capabilities:

* Get item
* Add item
* Remove item
* Clear cache

## Built in cache provider implementation
There are couple implementation of the ICacheProvider interface built in package:
* WebCacheProvider - uses System.Web.Caching.Cache (HttpRuntime.Cache).
* MemoryCacheProvider - uses System.Runtime.Caching.MemoryCache (MemoryCache.Default).
* DummyCacheProvider - uses dictionary to simply store all items and without any expiration or dependency caching support. Mostly used in unit testing.
* NullCacheProvider – a Null-object pattern implementation which simply does not store any cache items and always empty. Mostly used in unit testing.

## Example of usage
Choose a provider and register it in application root. This is an example to do register cache provider inside web app using [SimpleInjector](https://simpleinjector.codeplex.com/):
```csharp
public class Global : HttpApplication
{
	protected void Application_Start (object sender, EventArgs e)
	{
		// ...
  		container.RegisterSingle<ICacheProvider> (new WebCacheProvider ());
		// ...
	}
}

```

And now it's easy to use it anywhere. For example, in services:

```csharp
internal class UsersService : IUsersService
{
	private readonly ICacheProvider cache;


	public UsersService (ICacheProvider cache)
	{
		if (cache == null)
			throw new ArgumentNullException ("cache");
		  
		this.cache = cache;
	}


	public int GetUsersCount ()
	{
		var result = this.cache.Get ("UsersCount",
			                            () => new CachableResult<int> (GetUsersCountFromDb (),
			                            		new CachingParameters (TimeSpan.FromMinutes (15))));

		return result;
	}


	private static int GetUsersCountFromDb ()
	{
		Thread.Sleep (TimeSpan.FromSeconds (1));
		return Database.UsersCount;
	}
}
```

## NuGet package
You can install nuget package for Rocks.Caching here: https://www.nuget.org/packages/Rocks.Caching/
