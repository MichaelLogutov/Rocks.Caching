using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Rocks.Caching
{
	/// <summary>
	///     A helpers class that can wrap operation of retrieving cached object value into thread safe lock
	///     to ensure that creation of cached value will be performed only once when needed.
	/// </summary>
	public static class GetWithLockExtensions
	{
		#region Private fields

		private static readonly object EmptyResult = new object ();
		internal static readonly ConcurrentDictionary<string, CachedResultLock> Locks = new ConcurrentDictionary<string, CachedResultLock> ();

		#endregion

		#region Static methods

		/// <summary>
		///     Gets cached object.
		///     Returns null if object was not found in cache.
		/// </summary>
		/// <typeparam name="T">Type of cached object.</typeparam>
		/// <param name="cache">Cache provider. Can not be null.</param>
		/// <param name="cachableObject">
		///     Instance of <see cref="ICachable&lt;T&gt;" /> that can provide data required
		///     for retrieval object value from <paramref name="cache" />. Can not be null.
		/// </param>
		public static T Get<T> ([NotNull] this ICacheProvider cache, [NotNull] ICachable<T> cachableObject)
		{
			if (cachableObject == null)
				throw new ArgumentNullException ("cachableObject");

			return cache.Get (cachableObject.GetCacheKey (), cachableObject.GetCachedValue);
		}


		/// <summary>
		///     Gets cached object by <paramref name="key" />.
		///     If object was not found in cache will call <paramref name="createResult" /> callback,
		///     store it's in the <paramref name="cache" /> and returns it's value.
		/// </summary>
		/// <typeparam name="T">Type of cached object.</typeparam>
		/// <param name="cache">Cache provider. Can not be null.</param>
		/// <param name="key">Cache key. Can not be null.</param>
		/// <param name="createResult">
		///     A callback that should return data used in caching object
		///     if it was not found in <paramref name="cache" />. Can not be null.
		/// </param>
		public static T Get<T> ([NotNull] this ICacheProvider cache, [NotNull] string key, [NotNull] Func<CachableResult<T>> createResult)
		{
			if (cache == null)
				throw new ArgumentNullException ("cache");

			if (string.IsNullOrEmpty (key))
				throw new ArgumentNullException ("key");

			if (createResult == null)
				throw new ArgumentNullException ("createResult");


			// first - try the real cache
			var res = cache.Get (key);
			if (res == null)
			{
				// cache miss - obtain lock to execute createResult
				// only once among possible concurrent requests
				var cached_result_lock = AcquireCachedResultLock (key);

				if (!cached_result_lock.IsExecuted)
				{
					try
					{
						var result = createResult ();
						ProceedResult (cache, key, cached_result_lock, result);
					}
					catch (Exception ex)
					{
						ProceedExceptionResult (cached_result_lock, ex);
						throw;
					}
					finally
					{
						ReleaseCachedResultLock (key, cached_result_lock);
					}
				}
				else
					ReleaseCachedResultLock (key, cached_result_lock);

				var exception_result = cached_result_lock.Result as ExceptionResult;
				if (exception_result != null)
					throw exception_result.Exception;

				res = cached_result_lock.Result;
			}

		    if (res != EmptyResult && res != null)
		        return (T) res;

            return default (T);
		}


		/// <summary>
		///     Gets cached object by <paramref name="key" />.
		///     If object was not found in cache will call <paramref name="createResult" /> callback,
		///     store it's in the <paramref name="cache" /> and returns it's value.
		/// </summary>
		/// <typeparam name="T">Type of cached object.</typeparam>
		/// <param name="cache">Cache provider. Can not be null.</param>
		/// <param name="key">Cache key. Can not be null.</param>
		/// <param name="createResult">
		///     A callback that should return data used in caching object
		///     if it was not found in <paramref name="cache" />. Can not be null.
		/// </param>
		public static async Task<T> GetAsync<T> ([NotNull] this ICacheProvider cache,
		                                         [NotNull] string key,
		                                         [NotNull] Func<Task<CachableResult<T>>> createResult)
		{
			if (cache == null)
				throw new ArgumentNullException ("cache");

			if (string.IsNullOrEmpty (key))
				throw new ArgumentNullException ("key");

			if (createResult == null)
				throw new ArgumentNullException ("createResult");


			// first - try the real cache
			var res = cache.Get (key);
			if (res == null)
			{
				// cache miss - obtain lock to execute createResult
				// only once among possible concurrent requests
				var cached_result_lock = AcquireCachedResultLock (key);

				if (!cached_result_lock.IsExecuted)
				{
					try
					{
						var result = await createResult ().ConfigureAwait (false);
						ProceedResult (cache, key, cached_result_lock, result);
					}
					catch (Exception ex)
					{
						ProceedExceptionResult (cached_result_lock, ex);
						throw;
					}
					finally
					{
						ReleaseCachedResultLock (key, cached_result_lock);
					}
				}
				else
					ReleaseCachedResultLock (key, cached_result_lock);

				var exception_result = cached_result_lock.Result as ExceptionResult;
				if (exception_result != null)
					throw exception_result.Exception;

				res = cached_result_lock.Result;
			}

			if (res != EmptyResult && res != null)
		        return (T) res;

            return default (T);
		}

		#endregion

		#region Private methods

		private static CachedResultLock AcquireCachedResultLock (string key)
		{
			var result = Locks.GetOrAdd (key, x => new CachedResultLock ());

			result.Mutex.Wait ();

			return result;
		}


		private static void ReleaseCachedResultLock (string key, CachedResultLock cachedResultLock)
		{
			cachedResultLock.Mutex.Release ();

			CachedResultLock removed_cached_result_lock;
			Locks.TryRemove (key, out removed_cached_result_lock);
		}


		private static void ProceedResult<T> (ICacheProvider cache, string key, CachedResultLock cachedResultLock, CachableResult<T> cachableResult)
		{
			object result = null;

			if (cachableResult != null)
			{
				// if cachableResult.Result is null, it's still a result. So we need to cache it as EmptyResult object
				// so the next time we didn't get null from cache and had to go create result
				// once again. But we can do it only if dependency keys are null/empty, or they don't include
				// cachableResult data, because otherwise will be caching EmptyResult object
				// with dependeny key calculated for null object which is incorrect behavior.

				result = cachableResult.Result;
				if (result == null && (cachableResult.Parameters.DependencyKeys == null ||
				                       !cachableResult.Parameters.DependencyKeys.Any () ||
				                       !cachableResult.DependencyKeysIncludeResult))
					result = EmptyResult;

				cache.Add (key, result, cachableResult.Parameters);
			}

			cachedResultLock.Result = result;
			cachedResultLock.IsExecuted = true;
		}


		private static void ProceedExceptionResult (CachedResultLock cachedResultLock, Exception exception)
		{
			var result = new ExceptionResult { Exception = exception };

			cachedResultLock.Result = result;
			cachedResultLock.IsExecuted = true;
		}

		#endregion
	}
}