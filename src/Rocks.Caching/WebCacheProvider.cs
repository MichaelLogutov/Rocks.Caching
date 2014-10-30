using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Caching;

namespace Rocks.Caching
{
	/// <summary>
	///     An implementation of <see cref="ICacheProvider" /> based on <see cref="Cache" /> storage.
	/// </summary>
	public sealed class WebCacheProvider : ICacheProvider
	{
		#region Private fields

		private static readonly Dictionary<CachePriority, CacheItemPriority> cachePriorityToCacheItemPriority =
			new Dictionary<CachePriority, CacheItemPriority>
			{
				{
					CachePriority.Low, CacheItemPriority.Low
				},
				{
					CachePriority.BelowNormal,
					CacheItemPriority.BelowNormal
				},
				{
					CachePriority.Normal,
					CacheItemPriority.Normal
				},
				{
					CachePriority.AboveNormal,
					CacheItemPriority.AboveNormal
				},
				{
					CachePriority.High,
					CacheItemPriority.High
				},
				{
					CachePriority.NotRemovable,
					CacheItemPriority.NotRemovable
				}
			};

		private readonly Cache cache;

		private static long DependencyCacheValue;

		#endregion

		#region Construct

		/// <summary>
		///     Creates <see cref="WebCacheProvider" /> object instance
		///     that will use <see cref="HttpRuntime.Cache" />.
		/// </summary>
		public WebCacheProvider ()
		{
			this.cache = HttpRuntime.Cache;
		}

		#endregion

		#region ICacheProvider Members

		/// <summary>
		///     Gets cached object by <paramref name="key" />.
		///     Returns null if object was not found in cache.
		/// </summary>
		/// <param name="key">Cache key. Can not be null.</param>
		public object Get (string key)
		{
			return this.cache[key];
		}


		/// <summary>
		///     Adds or updates object in the cache.
		/// </summary>
		/// <param name="key">Object key. Can not be null.</param>
		/// <param name="value">Object value. Can not be null.</param>
		/// <param name="parameters">Caching parameters. Can not be null.</param>
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

			DateTime absolute_expiration;
			TimeSpan sliding_expiration;

			if (!parameters.Sliding)
			{
				absolute_expiration = DateTime.Now + parameters.Expiration;
				sliding_expiration = Cache.NoSlidingExpiration;
			}
			else
			{
				absolute_expiration = Cache.NoAbsoluteExpiration;
				sliding_expiration = parameters.Expiration;
			}

			CacheDependency dependency = null;
			if (parameters.DependencyKeys != null)
			{
				var dependency_keys = parameters.DependencyKeys.Where (x => !string.IsNullOrEmpty (x)).ToArray ();
				if (dependency_keys.Length > 0)
				{
					this.EnsureDependencyItemsExist (dependency_keys);
					dependency = new CacheDependency (null, dependency_keys);
				}
			}

			this.cache.Add (key,
			                value,
			                dependency,
			                absolute_expiration,
			                sliding_expiration,
			                GetItemPriority (parameters.Priority),
			                null);
		}


		/// <summary>
		///     Removes all cached objects.
		/// </summary>
		public void Clear ()
		{
			var cache_items_enumerator = this.cache.GetEnumerator ();

			var remove_keys = new List<string> ();
			while (cache_items_enumerator.MoveNext ())
				remove_keys.Add ((string) cache_items_enumerator.Key);

			foreach (var key in remove_keys)
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
			foreach (var key in dependencyKeys)
			{
				if (this.cache[key] != null)
					continue;

				var dependency_value = Interlocked.Increment (ref DependencyCacheValue);

				this.cache.Add (key,
				                dependency_value,
				                null,
				                Cache.NoAbsoluteExpiration,
				                Cache.NoSlidingExpiration,
				                CacheItemPriority.NotRemovable,
				                null);
			}
		}


		private static CacheItemPriority GetItemPriority (CachePriority? priority)
		{
			if (priority != null)
			{
				CacheItemPriority result;
				if (cachePriorityToCacheItemPriority.TryGetValue (priority.Value, out result))
					return result;
			}

			return CacheItemPriority.Default;
		}

		#endregion
	}
}