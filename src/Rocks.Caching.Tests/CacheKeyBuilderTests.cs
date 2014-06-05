using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Rocks.Caching.Tests
{
	[TestClass]
	public class CacheKeyBuilderTests
	{
		[TestMethod]
		public void ShouldCreate ()
		{
			Assert.AreEqual ("{A}", CacheKeyBuilder.Create ("A"));
			Assert.AreEqual ("{A}{B}", CacheKeyBuilder.Create ("A", "B"));
			Assert.AreEqual ("{A}{B}{C}", CacheKeyBuilder.Create ("A", "B", "C"));
			Assert.AreEqual ("{A}{}{C}", CacheKeyBuilder.Create ("A", null, "C"));
			Assert.AreEqual ("{:1:2:3:}", CacheKeyBuilder.Create (new[] { 1, 2, 3 }));
			Assert.AreEqual ("{A}{:1:2:3:}", CacheKeyBuilder.Create ("A", new[] { 1, 2, 3 }));
			Assert.AreEqual ("{A}{B}{:1:2:3:}", CacheKeyBuilder.Create ("A", "B", new[] { 1, 2, 3 }));
			Assert.AreEqual ("{A}{B}{:1:2:3:}{C}", CacheKeyBuilder.Create ("A", "B", new[] { 1, 2, 3 }, "C"));
			Assert.AreEqual ("{A}{}{:1:2:3:}{}", CacheKeyBuilder.Create ("A", null, new[] { 1, 2, 3 }, null));
			Assert.AreEqual ("{A}{::1:2:::3:4:::5:6::}", CacheKeyBuilder.Create ("A", new[] { new[] { 1, 2 }, new[] { 3, 4 }, new[] { 5, 6 } }));
		}


		[TestMethod]
		public void ShouldCreateFromObjectProperties ()
		{
			var sut = new
			{
				A = 1,
				B = "abc",
				C = (object) null,
				D = new[] { 1, 2, 3 }
			};

			Assert.AreEqual ("{1}{abc}{}{:1:2:3:}", CacheKeyBuilder.CreateFromObjectProperties (sut));
		}


		[TestMethod]
		public void ShouldCreateFromObjectsList ()
		{
			Assert.AreEqual ("{:1:abc::}", CacheKeyBuilder.Create (new object[] { 1, "abc", null }));
		}
	}
}
