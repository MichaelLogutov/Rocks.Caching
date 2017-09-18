using System;
using Microsoft.Extensions.Caching.Memory;

namespace Rocks.Caching
{
    /// <summary>
    ///     An implementation of <see cref="ICacheProvider" /> based on <see cref="MemoryCache" /> storage.
    /// </summary>
    public class MemoryCacheProvider : ICacheProvider
    {
        private MemoryCache cache = new MemoryCache(new MemoryCacheOptions());


        /// <summary>
        ///     Gets cached object by <paramref name="key" />.
        ///     Returns null if object was not found in cache.
        /// </summary>
        /// <param name="key">Cache key. Can not be null.</param>
        public object Get(string key)
        {
            return this.cache.Get(key);
        }


        /// <summary>
        ///     Adds or updates object in the cache.
        /// </summary>
        /// <param name="key">Object key. Can not be null.</param>
        /// <param name="value">Object value.</param>
        /// <param name="parameters">Caching parameters.</param>
        public void Add(string key, object value, CachingParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            if (value == null)
            {
                this.Remove(key);
                return;
            }

            if (parameters.NoCaching)
            {
                return;
            }

            var memoryCacheEntryOptions = new MemoryCacheEntryOptions();

            if (parameters.Priority == CachePriority.NotRemovable)
            {
                memoryCacheEntryOptions.Priority = CacheItemPriority.NeverRemove;
            }

            if (!parameters.Sliding)
            {
                memoryCacheEntryOptions.AbsoluteExpiration = DateTimeOffset.Now + parameters.Expiration;
            }
            else
            {
                memoryCacheEntryOptions.SlidingExpiration = parameters.Expiration;
            }

            this.cache.Set(key, value, memoryCacheEntryOptions);
        }


        /// <summary>
        ///     Removes all cached objects.
        /// </summary>
        public void Clear()
        {
            var newCache = new MemoryCache(new MemoryCacheOptions());
            var oldCache = this.cache;

            this.cache = newCache;

            oldCache.Dispose();
        }


        /// <summary>
        ///     Remove cached object by it's key.
        /// </summary>
        /// <param name="key">Object key. Can not be null.</param>
        public void Remove(string key)
        {
            this.cache.Remove(key);
        }
    }
}