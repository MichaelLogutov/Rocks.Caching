using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace Rocks.Caching
{
	/// <summary>
	///     An implementation of <see cref="ICacheProvider" /> based on <see cref="MemoryCache" /> storage.
	/// </summary>
	public class MemoryCacheProvider : ICacheProvider
	{
		#region Private fields

		private readonly MemoryCache cache = MemoryCache.Default;

		#endregion

		#region ICacheProvider Members

		/// <summary>
		///     Gets cached object by <paramref name="key" />.
		///     Returns null if object was not found in cache.
		/// </summary>
		/// <param name="key">Cache key. Can not be null.</param>
		public object Get (string key)
		{
			return this.cache.Get (key);
		}


		/// <summary>
		///     Adds or updates object in the cache.
		/// </summary>
		/// <param name="key">Object key. Can not be null.</param>
		/// <param name="value">Object value.</param>
		/// <param name="parameters">Caching parameters.</param>
		public void Add (string key, object value, CachingParameters parameters)
		{
			if (parameters == null)
				throw new ArgumentNullException ("parameters");

			if (value == null)
			{
				this.Remove (key);
				return;
			}

			if (parameters.NoCaching)
				return;

			var cache_item_policy = new CacheItemPolicy ();

			if (parameters.Priority == CachePriority.NotRemovable)
				cache_item_policy.Priority = CacheItemPriority.NotRemovable;

			if (!parameters.Sliding)
				cache_item_policy.AbsoluteExpiration = DateTimeOffset.Now + parameters.Expiration;
			else
				cache_item_policy.SlidingExpiration = parameters.Expiration;

			if (parameters.DependencyKeys != null)
			{
				var dependency_keys = parameters.DependencyKeys.Where (x => !string.IsNullOrEmpty (x)).ToList ();
				if (dependency_keys.Count > 0)
				{
					this.EnsureDependencyItemsExist (dependency_keys);
					cache_item_policy.ChangeMonitors.Add (this.cache.CreateCacheEntryChangeMonitor (dependency_keys));
				}
			}

			this.cache.Set (new CacheItem (key, value), cache_item_policy);
		}


		/// <summary>
		///     Removes all cached objects.
		/// </summary>
		public void Clear ()
		{
			this.cache.Trim (100);

			// current implementation of MemoryCache does not clears 100% of items despite of method signature
			// so we have to clean up the rest using expensive keys enumeration which implies lock
			var remain_keys = this.cache.Select (x => x.Key).ToList ();
			foreach (var key in remain_keys)
				this.cache.Remove (key);
		}


		/// <summary>
		///     Remove cached object by it's key.
		/// </summary>
		/// <param name="key">Object key. Can not be null.</param>
		public void Remove (string key)
		{
			this.cache.Remove (key);
		}

		#endregion

		#region Private methods

		private void EnsureDependencyItemsExist (IEnumerable<string> dependencyKeys)
		{
			var dependency_item_policy = new CacheItemPolicy { Priority = CacheItemPriority.NotRemovable };

			foreach (var key in dependencyKeys)
			{
				// note: use add instead of set because we don't want to overwrite existing items
				this.cache.Add (key, true, dependency_item_policy);
			}
		}

		#endregion
	}
}