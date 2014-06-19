using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Rocks.Caching
{
	/// <summary>
	///     Specifying parameters for caching an object.
	/// </summary>
	public sealed class CachingParameters
	{
		#region Constants

		public static readonly CachingParameters NoCache = new CachingParameters (TimeSpan.Zero);

		#endregion

		#region Private fields

		private readonly TimeSpan expiration;
		private readonly bool sliding;
		private readonly IEnumerable<string> dependencyKeys;
		private readonly CachePriority? priority;

		#endregion

		#region Construct

		/// <summary>
		///     Initializes a new instance of the <see cref="T:System.Object" /> class.
		/// </summary>
		public CachingParameters (TimeSpan expiration, bool sliding = false, IEnumerable<string> dependencyKeys = null, CachePriority? priority = null)
		{
			this.expiration = expiration;
			this.sliding = sliding;
			this.dependencyKeys = dependencyKeys;
			this.priority = priority;
		}


		private CachingParameters ([NotNull] CachingParameters other)
		{
			if (other == null)
				throw new ArgumentNullException ("other");

			this.expiration = other.expiration;
			this.sliding = other.sliding;

			if (other.dependencyKeys != null)
				this.dependencyKeys = other.dependencyKeys.ToList ();

			this.priority = other.priority;
		}

		#endregion

		#region Public properties

		/// <summary>
		///     (GET) Expiration time.
		/// </summary>
		public TimeSpan Expiration { get { return this.expiration; } }


		/// <summary>
		///     (GET) True if current parameters represents sliding expiration.
		/// </summary>
		public bool Sliding { get { return this.sliding; } }

		/// <summary>
		///     (GET) Cached object priority.
		/// </summary>
		public CachePriority? Priority { get { return this.priority; } }


		/// <summary>
		///     (GET) List of dependency keys.
		/// </summary>
		public IEnumerable<string> DependencyKeys { get { return this.dependencyKeys; } }


		/// <summary>
		///     (GET) Returns true if no expiration set for current parameters.
		/// </summary>
		public bool NoCaching { get { return this.Expiration == TimeSpan.Zero; } }

		#endregion

		#region Public methods

		/// <summary>
		///     Performs deep clone of the current object instance.
		/// </summary>
		public CachingParameters Clone ()
		{
			return new CachingParameters (this);
		}


		/// <summary>
		///     Returns a string that represents the current object.
		/// </summary>
		/// <returns>
		///     A string that represents the current object.
		/// </returns>
		public override string ToString ()
		{
			if (this.NoCaching)
				return "no caching";

			var result = this.expiration + (this.sliding ? " sliding" : " absolute");

			return result;
		}

		#endregion
	}
}