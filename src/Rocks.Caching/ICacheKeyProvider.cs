namespace Rocks.Caching
{
	/// <summary>
	/// Specifies object that can return cache key for itself.
	/// </summary>
	public interface ICacheKeyProvider
	{
		/// <summary>
		/// Returns cache key for current object. Can not be null.
		/// </summary>
		string GetCacheKey ();
	}
}
