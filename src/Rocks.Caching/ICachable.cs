namespace Rocks.Caching
{
	/// <summary>
	/// Specifies object that can be cached and will provide caching parameters.
	/// <typeparam name="T">Cached object value type.</typeparam>
	/// </summary>
	public interface ICachable<T> : ICacheKeyProvider
	{
		/// <summary>
		/// Returns value for current object that will be cached. Can not be null.
		/// </summary>
		CachableResult<T> GetCachedValue ();
	}
}
