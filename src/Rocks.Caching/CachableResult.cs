using System;

namespace Rocks.Caching
{
	/// <summary>
	/// A class that represent resulting data that can be cached with specified parameters.
	/// </summary>
	/// <typeparam name="T">Type of the data.</typeparam>
	public class CachableResult<T>
	{
		/// <summary>
		/// (GET) Resulting data.
		/// </summary>
		public T Result { get; private set; }


		/// <summary>
		/// (GET) Caching parameters.
		/// </summary>
		public CachingParameters Parameters { get; private set; }


		/// <summary>
		/// (GET) True if caching <see cref="Parameters"/>.DependencyKeys contains parts that depends on <see cref="Result"/> itself.
		/// </summary>
		public bool DependencyKeysIncludeResult { get; private set; }


		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="result">Resulting data. Can be null.</param>
		/// <param name="parameters">Caching parameters. Can not be null.</param>
		/// <param name="dependencyKeysIncludeResult">True if caching <paramref name="parameters"/>.DependencyKeys contains keys that depends on <paramref name="result"/> itself.</param>
		public CachableResult (T result, CachingParameters parameters, bool dependencyKeysIncludeResult = false)
		{
			if (parameters == null)
				throw new ArgumentNullException ("parameters");

			this.Result = result;
			this.Parameters = parameters;
			this.DependencyKeysIncludeResult = dependencyKeysIncludeResult;
		}
	}
}
