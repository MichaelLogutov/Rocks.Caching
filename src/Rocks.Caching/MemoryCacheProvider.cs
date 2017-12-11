using System;
#if NET461 || NET471
using System.Runtime.Caching;
using System.Linq;
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
#if NET461 || NET471
        private readonly MemoryCache cache = MemoryCache.Default;
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

#if NET461 || NET471
            var cache_item_policy = new CacheItemPolicy ();

            if (parameters.Priority == CachePriority.NotRemovable)
                cache_item_policy.Priority = CacheItemPriority.NotRemovable;

            if (!parameters.Sliding)
                cache_item_policy.AbsoluteExpiration = DateTimeOffset.Now + parameters.Expiration;
            else
                cache_item_policy.SlidingExpiration = parameters.Expiration;

            this.cache.Set (new CacheItem (key, value), cache_item_policy);
            
#elif NETSTANDARD2_0
            var memory_cache_entry_options = new MemoryCacheEntryOptions();
    
            if (parameters.Priority == CachePriority.NotRemovable)
            {
                memory_cache_entry_options.Priority = CacheItemPriority.NeverRemove;
            }

            if (!parameters.Sliding)
            {
                memory_cache_entry_options.AbsoluteExpiration = DateTimeOffset.Now + parameters.Expiration;
            }
            else
            {
                memory_cache_entry_options.SlidingExpiration = parameters.Expiration;
            }

            this.cache.Set(key, value, memory_cache_entry_options);
#endif 
        }


        /// <summary>
        ///     Removes all cached objects.
        /// </summary>
        public void Clear()
        {
#if NET461 || NET471
            this.cache.Trim (100);

            // current implementation of MemoryCache does not clears 100% of items despite of method signature
            // so we have to clean up the rest using expensive keys enumeration which implies lock
            var remain_keys = this.cache.Select (x => x.Key).ToList ();
            foreach (var key in remain_keys)
                this.cache.Remove (key);
#elif NETSTANDARD2_0
            var new_cache = new MemoryCache(new MemoryCacheOptions());
            var old_cache = this.cache;

            this.cache = new_cache;

            old_cache.Dispose();
#endif
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