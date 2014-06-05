using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Rocks.Caching.Tests
{
	[TestClass]
	public class WebCacheProviderTests : CacheProviderTestBase
	{
		protected override ICacheProvider CreateSut ()
		{
			return new WebCacheProvider ();
		}
	}
}