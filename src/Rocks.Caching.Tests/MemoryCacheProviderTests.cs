namespace Rocks.Caching.Tests
{
    public class MemoryCacheProviderTests : CacheProviderTestBase
    {
        protected override ICacheProvider CreateSut()
        {
            return new MemoryCacheProvider();
        }
    }
}