using System;

#if NET471
using System.Runtime.Caching;
#elif NETSTANDARD2_0
using Microsoft.Extensions.Caching.Memory;
#endif

namespace Rocks.Caching
{
    /// <summary>
    ///     An implementation of <see cref="ICacheProvider" /> based on <see cref="MemoryCache" /> storage.
    /// </summary>
    public class MemoryCacheProvider : ICacheProvider
    {
#if NET471
        // ReSharper disable once AssignNullToNotNullAttribute
        private MemoryCache cache = new MemoryCache(typeof(MemoryCacheProvider).FullName);
#elif NETSTANDARD2_0
        private MemoryCache cache = new MemoryCache(new MemoryCacheOptions());
#endif

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
                throw new ArgumentNullException(nameof(parameters));

            if (value == null)
            {
                this.Remove(key);
                return;
            }

            if (parameters.NoCaching)
                return;

#if NET471
            var options = new CacheItemPolicy();
    
            if (parameters.Priority == CachePriority.NotRemovable)
                options.Priority = CacheItemPriority.NotRemovable;
#elif NETSTANDARD2_0
            var options = new MemoryCacheEntryOptions();
    
            if (parameters.Priority == CachePriority.NotRemovable)
                options.Priority = CacheItemPriority.NeverRemove;
#endif
            
            if (!parameters.Sliding)
                options.AbsoluteExpiration = DateTimeOffset.Now + parameters.Expiration;
            else
                options.SlidingExpiration = parameters.Expiration;

            this.cache.Set(key, value, options);
        }


        /// <summary>
        ///     Removes all cached objects.
        /// </summary>
        public void Clear()
        {
#if NET471
            // ReSharper disable once AssignNullToNotNullAttribute
            var new_cache = new MemoryCache(typeof(MemoryCacheProvider).FullName);
#elif NETSTANDARD2_0
            var new_cache = new MemoryCache(new MemoryCacheOptions());
#endif
            var old_cache = this.cache;

            this.cache = new_cache;

            old_cache.Dispose();
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