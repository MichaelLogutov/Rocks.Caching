using System;
using System.Diagnostics;

namespace Rocks.Caching
{
    /// <summary>
    ///     A class that represent resulting data that can be cached with specified parameters.
    /// </summary>
    /// <typeparam name="T">Type of the data.</typeparam>
    [DebuggerDisplay("{Result}, Expiration = {Parameters.Expiration}, Sliding = {Parameters.Sliding}, Priority = {Parameters.Priority}")]
    public class CachableResult<T>
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="result">Resulting data. Can be null.</param>
        /// <param name="parameters">Caching parameters. Can not be null.</param>
        public CachableResult(T result, CachingParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            this.Result = result;
            this.Parameters = parameters;
        }


        /// <summary>
        ///     (GET) Resulting data.
        /// </summary>
        public T Result { get; }


        /// <summary>
        ///     (GET) Caching parameters.
        /// </summary>
        public CachingParameters Parameters { get; }


        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        ///     A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            // ReSharper disable once CompareNonConstrainedGenericWithNull
            var str = this.Result == null ? "null" : this.Result.ToString();

            if (this.Parameters.NoCaching)
            {
                str += ", no caching";
            }
            else
            {
                str += ", " + this.Parameters;
            }

            return str;
        }
    }
}