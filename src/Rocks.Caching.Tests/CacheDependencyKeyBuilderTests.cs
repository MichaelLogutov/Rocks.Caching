using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Rocks.Caching.Tests
{
	[TestClass]
	public class CacheDependencyKeyBuilderTests
	{
		[TestMethod]
		public void ShouldCorrectlyCreate ()
		{
			Assert.AreEqual ("{" + CacheDependencyKeyBuilder.DependencyRootCacheKeyPrefix + "A}", CacheDependencyKeyBuilder.Create ("A", null));
			Assert.AreEqual ("{" + CacheDependencyKeyBuilder.DependencyRootCacheKeyPrefix + "A:B}", CacheDependencyKeyBuilder.Create ("A", "B"));
			Assert.AreEqual
			(
				"{" + CacheDependencyKeyBuilder.DependencyRootCacheKeyPrefix + "A:1}" +
				"{" + CacheDependencyKeyBuilder.DependencyRootCacheKeyPrefix + "A:2}" +
				"{" + CacheDependencyKeyBuilder.DependencyRootCacheKeyPrefix + "A:3}",
				CacheDependencyKeyBuilder.Create ("A", new[] { 1, 2, 3 })
			);
		}


		[TestMethod]
		public void ShouldCorrectlyCreateMany ()
		{
			{
				var res = CacheDependencyKeyBuilder.CreateMany ("A");
				Assert.AreEqual (1, res.Length);
				Assert.AreEqual ("{" + CacheDependencyKeyBuilder.DependencyRootCacheKeyPrefix + "A}", res[0]);
			}
			{
				var res = CacheDependencyKeyBuilder.CreateMany ("A", "B");
				Assert.AreEqual (1, res.Length);
				Assert.AreEqual ("{" + CacheDependencyKeyBuilder.DependencyRootCacheKeyPrefix + "A:B}", res[0]);
			}
			{
				var res = CacheDependencyKeyBuilder.CreateMany ("A", "B", "C");
				Assert.AreEqual (2, res.Length);
				Assert.AreEqual ("{" + CacheDependencyKeyBuilder.DependencyRootCacheKeyPrefix + "A:B}", res[0]);
				Assert.AreEqual ("{" + CacheDependencyKeyBuilder.DependencyRootCacheKeyPrefix + "C}", res[1]);
			}
			{
				var res = CacheDependencyKeyBuilder.CreateMany ("A", new[] { 1, 2, 3 }, "C");
				Assert.AreEqual (4, res.Length);
				Assert.AreEqual ("{" + CacheDependencyKeyBuilder.DependencyRootCacheKeyPrefix + "A:1}", res[0]);
				Assert.AreEqual ("{" + CacheDependencyKeyBuilder.DependencyRootCacheKeyPrefix + "A:2}", res[1]);
				Assert.AreEqual ("{" + CacheDependencyKeyBuilder.DependencyRootCacheKeyPrefix + "A:3}", res[2]);
				Assert.AreEqual ("{" + CacheDependencyKeyBuilder.DependencyRootCacheKeyPrefix + "C}", res[3]);
			}
			{
				var res = CacheDependencyKeyBuilder.CreateMany ("A", "B", null);
				Assert.AreEqual (1, res.Length);
				Assert.AreEqual ("{" + CacheDependencyKeyBuilder.DependencyRootCacheKeyPrefix + "A:B}", res[0]);
			}
			{
				var res = CacheDependencyKeyBuilder.CreateMany ("A", null, null);
				Assert.AreEqual (1, res.Length);
				Assert.AreEqual ("{" + CacheDependencyKeyBuilder.DependencyRootCacheKeyPrefix + "A}", res[0]);
			}
			{
				var res = CacheDependencyKeyBuilder.CreateMany ("A", null, "B");
				Assert.AreEqual (2, res.Length);
				Assert.AreEqual ("{" + CacheDependencyKeyBuilder.DependencyRootCacheKeyPrefix + "A}", res[0]);
				Assert.AreEqual ("{" + CacheDependencyKeyBuilder.DependencyRootCacheKeyPrefix + "B}", res[1]);
			}
			{
				var res = CacheDependencyKeyBuilder.CreateMany (new[] { 1, 2, 3 }, null, "B");
				Assert.AreEqual (2, res.Length);
				Assert.AreEqual ("{" + CacheDependencyKeyBuilder.DependencyRootCacheKeyPrefix + ":1:2:3:}", res[0]);
				Assert.AreEqual ("{" + CacheDependencyKeyBuilder.DependencyRootCacheKeyPrefix + "B}", res[1]);
			}
		}
	}
}
