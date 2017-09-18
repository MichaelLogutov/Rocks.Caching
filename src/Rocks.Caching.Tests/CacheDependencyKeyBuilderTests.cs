using Xunit;

namespace Rocks.Caching.Tests
{
    public class CacheDependencyKeyBuilderTests
    {
        [Fact]
        public void ShouldCorrectlyCreate()
        {
            Assert.Equal("{" + CacheDependencyKeyBuilder.DependencyRootCacheKeyPrefix + "A}",
                CacheDependencyKeyBuilder.Create("A", null));
            Assert.Equal("{" + CacheDependencyKeyBuilder.DependencyRootCacheKeyPrefix + "A:B}",
                CacheDependencyKeyBuilder.Create("A", "B"));
            Assert.Equal
            (
                "{" + CacheDependencyKeyBuilder.DependencyRootCacheKeyPrefix + "A:1}" +
                "{" + CacheDependencyKeyBuilder.DependencyRootCacheKeyPrefix + "A:2}" +
                "{" + CacheDependencyKeyBuilder.DependencyRootCacheKeyPrefix + "A:3}",
                CacheDependencyKeyBuilder.Create("A", new[] {1, 2, 3})
            );
        }


        [Fact]
        public void ShouldCorrectlyCreateMany()
        {
            {
                var res = CacheDependencyKeyBuilder.CreateMany("A");
                Assert.Equal(1, res.Length);
                Assert.Equal("{" + CacheDependencyKeyBuilder.DependencyRootCacheKeyPrefix + "A}", res[0]);
            }
            {
                var res = CacheDependencyKeyBuilder.CreateMany("A", "B");
                Assert.Equal(1, res.Length);
                Assert.Equal("{" + CacheDependencyKeyBuilder.DependencyRootCacheKeyPrefix + "A:B}", res[0]);
            }
            {
                var res = CacheDependencyKeyBuilder.CreateMany("A", "B", "C");
                Assert.Equal(2, res.Length);
                Assert.Equal("{" + CacheDependencyKeyBuilder.DependencyRootCacheKeyPrefix + "A:B}", res[0]);
                Assert.Equal("{" + CacheDependencyKeyBuilder.DependencyRootCacheKeyPrefix + "C}", res[1]);
            }
            {
                var res = CacheDependencyKeyBuilder.CreateMany("A", new[] {1, 2, 3}, "C");
                Assert.Equal(4, res.Length);
                Assert.Equal("{" + CacheDependencyKeyBuilder.DependencyRootCacheKeyPrefix + "A:1}", res[0]);
                Assert.Equal("{" + CacheDependencyKeyBuilder.DependencyRootCacheKeyPrefix + "A:2}", res[1]);
                Assert.Equal("{" + CacheDependencyKeyBuilder.DependencyRootCacheKeyPrefix + "A:3}", res[2]);
                Assert.Equal("{" + CacheDependencyKeyBuilder.DependencyRootCacheKeyPrefix + "C}", res[3]);
            }
            {
                var res = CacheDependencyKeyBuilder.CreateMany("A", "B", null);
                Assert.Equal(1, res.Length);
                Assert.Equal("{" + CacheDependencyKeyBuilder.DependencyRootCacheKeyPrefix + "A:B}", res[0]);
            }
            {
                var res = CacheDependencyKeyBuilder.CreateMany("A", null, null);
                Assert.Equal(1, res.Length);
                Assert.Equal("{" + CacheDependencyKeyBuilder.DependencyRootCacheKeyPrefix + "A}", res[0]);
            }
            {
                var res = CacheDependencyKeyBuilder.CreateMany("A", null, "B");
                Assert.Equal(2, res.Length);
                Assert.Equal("{" + CacheDependencyKeyBuilder.DependencyRootCacheKeyPrefix + "A}", res[0]);
                Assert.Equal("{" + CacheDependencyKeyBuilder.DependencyRootCacheKeyPrefix + "B}", res[1]);
            }
            {
                var res = CacheDependencyKeyBuilder.CreateMany(new[] {1, 2, 3}, null, "B");
                Assert.Equal(2, res.Length);
                Assert.Equal("{" + CacheDependencyKeyBuilder.DependencyRootCacheKeyPrefix + ":1:2:3:}", res[0]);
                Assert.Equal("{" + CacheDependencyKeyBuilder.DependencyRootCacheKeyPrefix + "B}", res[1]);
            }
        }
    }
}