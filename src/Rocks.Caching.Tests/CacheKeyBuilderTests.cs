using Xunit;

namespace Rocks.Caching.Tests
{
    public class CacheKeyBuilderTests
    {
        [Fact]
        public void ShouldCreate()
        {
            Assert.Equal("{A}", CacheKeyBuilder.Create("A"));
            Assert.Equal("{A}{B}", CacheKeyBuilder.Create("A", "B"));
            Assert.Equal("{A}{B}{C}", CacheKeyBuilder.Create("A", "B", "C"));
            Assert.Equal("{A}{}{C}", CacheKeyBuilder.Create("A", null, "C"));
            Assert.Equal("{:1:2:3:}", CacheKeyBuilder.Create(new[] {1, 2, 3}));
            Assert.Equal("{A}{:1:2:3:}", CacheKeyBuilder.Create("A", new[] {1, 2, 3}));
            Assert.Equal("{A}{B}{:1:2:3:}", CacheKeyBuilder.Create("A", "B", new[] {1, 2, 3}));
            Assert.Equal("{A}{B}{:1:2:3:}{C}", CacheKeyBuilder.Create("A", "B", new[] {1, 2, 3}, "C"));
            Assert.Equal("{A}{}{:1:2:3:}{}", CacheKeyBuilder.Create("A", null, new[] {1, 2, 3}, null));
            Assert.Equal("{A}{::1:2:::3:4:::5:6::}", CacheKeyBuilder.Create("A", new[] {new[] {1, 2}, new[] {3, 4}, new[] {5, 6}}));
        }


        [Fact]
        public void ShouldCreateFromObjectProperties()
        {
            var sut = new
            {
                A = 1,
                B = "abc",
                C = (object) null,
                D = new[] {1, 2, 3}
            };

            Assert.Equal("{1}{abc}{}{:1:2:3:}", CacheKeyBuilder.CreateFromObjectProperties(sut));
        }


        [Fact]
        public void ShouldCreateFromObjectsList()
        {
            Assert.Equal("{:1:abc::}", CacheKeyBuilder.Create(new object[] {1, "abc", null}));
        }
    }
}