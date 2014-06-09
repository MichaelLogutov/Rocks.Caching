using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Rocks.Caching
{
	/// <summary>
	/// A dummy cache provider that will cache everything in the <see cref="Items"/> property.
	/// Items will not expire.
	/// Good for unit tests.
	/// </summary>
	public class DummyCacheProvider : ICacheProvider
	{
		public IDictionary<string, object> Items { get; set; }


		public DummyCacheProvider ()
		{
			this.Items = new ConcurrentDictionary<string, object> ();
		}


		/// <summary>
		/// Gets cached object by <paramref name="key"/>.
		/// Returns null if object was not found in cache.
		/// </summary>
		/// <param name="key">Cache key. Can not be null.</param>
		public virtual object Get (string key)
		{
			object value;

			if (!this.Items.TryGetValue (key, out value))
				return null;

			return value;
		}


		/// <summary>
		/// Adds or updates object in the cache.
		/// </summary>
		/// <param name="key">Object key. Can not be null.</param>
		/// <param name="value">Object value.</param>
		/// <param name="parameters">Caching parameters.</param>
		public virtual void Add (string key, object value, CachingParameters parameters)
		{
			this.Items[key] = value;
		}


		/// <summary>
		/// Removes all cached objects.
		/// </summary>
		public virtual void Clear ()
		{
			this.Items.Clear ();
		}


		/// <summary>
		/// Remove cached object by it's key.
		/// </summary>
		/// <param name="key">Object key. Can not be null.</param>
		public virtual void Remove (string key)
		{
			this.Items.Remove (key);
		}
	}
}