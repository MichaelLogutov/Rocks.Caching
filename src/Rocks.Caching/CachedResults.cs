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
	public static class CachedResults
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
				var cached_result_lock = Locks.GetOrAdd (key, x => new CachedResultLock ());

				if (cached_result_lock.Executed)
					res = cached_result_lock.Result;
				else if (cached_result_lock.TryStartExecuting ())
				{
					try
					{
						var result = createResult ();
						res = ProceedResult (cache, key, result, cached_result_lock);
					}
					catch (Exception ex)
					{
						ProceedExceptionResult (key, ex, cached_result_lock);
						throw;
					}
				}
				else
				{
					cached_result_lock.WaitForCompletion ();

					var exception_result = cached_result_lock.Result as ExceptionResult;
					if (exception_result != null)
						throw exception_result.Exception;

					res = cached_result_lock.Result;
				}
			}

			return res != EmptyResult ? (T) res : default (T);
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
				var cached_result_lock = Locks.GetOrAdd (key, x => new CachedResultLock ());

				if (cached_result_lock.Executed)
					res = cached_result_lock.Result;
				else if (cached_result_lock.TryStartExecuting ())
				{
					try
					{
						var result = await createResult ();
						res = ProceedResult (cache, key, result, cached_result_lock);
					}
					catch (Exception ex)
					{
						ProceedExceptionResult (key, ex, cached_result_lock);
						throw;
					}
				}
				else
				{
					cached_result_lock.WaitForCompletion ();

					var exception_result = cached_result_lock.Result as ExceptionResult;
					if (exception_result != null)
						throw exception_result.Exception;

					res = cached_result_lock.Result;
				}
			}

			return res != EmptyResult ? (T) res : default (T);
		}

		#endregion

		#region Private methods

		private static object ProceedResult<T> (ICacheProvider cache, string key, CachableResult<T> result, CachedResultLock cachedResultLock)
		{
			object res = null;

			if (result != null)
			{
				// if result.Result is null, it's still a result. So we need to cache it as EmptyResult object
				// so the next time we didn't get null from cache and had to go create result
				// once again. But we can do it only if dependency keys are null/empty, or they don't include
				// result data, because otherwise will be caching EmptyResult object
				// with dependeny key calculated for null object which is incorrect behavior.

				res = result.Result;
				if (res == null && (result.Parameters.DependencyKeys == null ||
				                    !result.Parameters.DependencyKeys.Any () ||
				                    !result.DependencyKeysIncludeResult))
					res = EmptyResult;

				cache.Add (key, res, result.Parameters);
			}

			cachedResultLock.EndExecution (res);

			// remove the lock object
			CachedResultLock removed_cached_result_lock;
			Locks.TryRemove (key, out removed_cached_result_lock);

			return res;
		}


		private static void ProceedExceptionResult (string key, Exception exception, CachedResultLock cachedResultLock)
		{
			var result = new ExceptionResult { Exception = exception };

			cachedResultLock.EndExecution (result);

			// remove the lock object
			CachedResultLock removed_cached_result_lock;
			Locks.TryRemove (key, out removed_cached_result_lock);
		}

		#endregion
	}
}