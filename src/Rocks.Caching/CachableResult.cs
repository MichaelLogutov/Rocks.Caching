using System;
using System.Diagnostics;

namespace Rocks.Caching
{
	/// <summary>
	///     A class that represent resulting data that can be cached with specified parameters.
	/// </summary>
	/// <typeparam name="T">Type of the data.</typeparam>
	[DebuggerDisplay ("{Result}, Expiration = {Parameters.Expiration}, Sliding = {Parameters.Sliding}, Priority = {Parameters.Priority}")]
	public class CachableResult<T>
	{
		#region Private fields

		private readonly T result;
		private readonly CachingParameters parameters;
		private readonly bool dependencyKeysIncludeResult;

		#endregion

		#region Construct

		/// <summary>
		///     Constructor.
		/// </summary>
		/// <param name="result">Resulting data. Can be null.</param>
		/// <param name="parameters">Caching parameters. Can not be null.</param>
		/// <param name="dependencyKeysIncludeResult">
		///     True if caching <paramref name="parameters" />.DependencyKeys contains keys
		///     that depends on <paramref name="result" /> itself.
		/// </param>
		public CachableResult (T result, CachingParameters parameters, bool dependencyKeysIncludeResult = false)
		{
			if (parameters == null)
				throw new ArgumentNullException ("parameters");

			this.result = result;
			this.parameters = parameters;
			this.dependencyKeysIncludeResult = dependencyKeysIncludeResult;
		}

		#endregion

		#region Public properties

		/// <summary>
		///     (GET) Resulting data.
		/// </summary>
		public T Result { get { return this.result; } }


		/// <summary>
		///     (GET) Caching parameters.
		/// </summary>
		public CachingParameters Parameters { get { return this.parameters; } }


		/// <summary>
		///     (GET) True if caching <see cref="Parameters" />.DependencyKeys contains parts that depends on <see cref="Result" />
		///     itself.
		/// </summary>
		public bool DependencyKeysIncludeResult { get { return this.dependencyKeysIncludeResult; } }

		#endregion

		#region Public methods

		/// <summary>
		///     Returns a string that represents the current object.
		/// </summary>
		/// <returns>
		///     A string that represents the current object.
		/// </returns>
		public override string ToString ()
		{
			// ReSharper disable once CompareNonConstrainedGenericWithNull
			var str = this.Result == null ? "null" : this.Result.ToString ();

			if (this.Parameters.NoCaching)
				str += ", no caching";
			else
				str += ", " + this.Parameters;

			return str;
		}

		#endregion
	}
}