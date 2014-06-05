using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Rocks.Caching.Tests
{
	[TestClass]
	public class MemoryCacheProviderTests : CacheProviderTestBase
	{
		protected override ICacheProvider CreateSut ()
		{
			return new MemoryCacheProvider ();
		}
	}
}