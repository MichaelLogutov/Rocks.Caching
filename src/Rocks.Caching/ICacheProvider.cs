using JetBrains.Annotations;

namespace Rocks.Caching
{
	/// <summary>
	/// A caching provider.
	/// </summary>
	public interface ICacheProvider
	{
		/// <summary>
		/// Gets cached object by <paramref name="key"/>.
		/// Returns null if object was not found in cache.
		/// </summary>
		/// <param name="key">Cache key. Can not be null.</param>
		[CanBeNull]
		object Get ([NotNull] string key);


		/// <summary>
		/// Adds or updates object in the cache.
		/// </summary>
		/// <param name="key">Object key. Can not be null.</param>
		/// <param name="value">Object value.</param>
		/// <param name="parameters">Caching parameters. Can not be null.</param>
		void Add ([NotNull] string key, [CanBeNull] object value, [NotNull] CachingParameters parameters);


		/// <summary>
		/// Removes all cached objects.
		/// </summary>
		void Clear ();


		/// <summary>
		/// Remove cached object by it's key.
		/// </summary>
		/// <param name="key">Object key. Can not be null.</param>
		void Remove ([NotNull] string key);
	}
}