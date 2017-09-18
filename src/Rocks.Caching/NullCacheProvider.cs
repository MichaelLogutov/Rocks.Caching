namespace Rocks.Caching
{
    /// <summary>
    ///     An pass-through <see cref="ICacheProvider" /> implementation that does not holds any cache items and always empty.
    /// </summary>
    public class NullCacheProvider : ICacheProvider
    {
        /// <summary>
        ///     Gets cached object by <paramref name="key" />.
        ///     Returns null if object was not found in cache.
        /// </summary>
        /// <param name="key">Cache key. Can not be null.</param>
        public object Get(string key)
        {
            return null;
        }


        /// <summary>
        ///     Adds or updates object in the cache.
        /// </summary>
        /// <param name="key">Object key. Can not be null.</param>
        /// <param name="value">Object value.</param>
        /// <param name="parameters">Caching parameters. Can not be null.</param>
        public void Add(string key, object value, CachingParameters parameters)
        {
        }


        /// <summary>
        ///     Removes all cached objects.
        /// </summary>
        public void Clear()
        {
        }


        /// <summary>
        ///     Remove cached object by it's key.
        /// </summary>
        /// <param name="key">Object key. Can not be null.</param>
        public void Remove(string key)
        {
        }
    }
}