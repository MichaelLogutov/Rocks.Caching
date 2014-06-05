using System;
using System.Collections.Generic;

namespace Rocks.Caching
{
	/// <summary>
	/// Specifying parameters for caching an object.
	/// </summary>
	public class CachingParameters
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
		/// Initializes a new instance of the <see cref="T:System.Object"/> class.
		/// </summary>
		public CachingParameters (TimeSpan expiration, bool sliding = false, IEnumerable<string> dependencyKeys = null, CachePriority? priority = null)
		{
			this.expiration = expiration;
			this.sliding = sliding;
			this.dependencyKeys = dependencyKeys;
			this.priority = priority;
		}

		#endregion

		#region Public properties

		/// <summary>
		/// (GET) Expiration time.
		/// </summary>
		public TimeSpan Expiration
		{
			get { return this.expiration; }
		}


		/// <summary>
		/// (GET) True if current parameters represents sliding expiration.
		/// </summary>
		public bool Sliding
		{
			get { return this.sliding; }
		}

		/// <summary>
		/// (GET) Cached object priority.
		/// </summary>
		public CachePriority? Priority
		{
			get { return this.priority; }
		}


		/// <summary>
		/// (GET) List of dependency keys.
		/// </summary>
		public IEnumerable<string> DependencyKeys
		{
			get { return this.dependencyKeys; }
		}


		/// <summary>
		/// (GET) Returns true if no expiration set for current parameters.
		/// </summary>
		public bool NoCaching
		{
			get
			{
				return this.Expiration == TimeSpan.Zero;
			}
		}

		#endregion
	}
}
