using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;

namespace Rocks.Caching
{
    /// <summary>
    ///     Specifying parameters for caching an object.
    /// </summary>
    [DebuggerDisplay("{Expiration}, Sliding = {Sliding}, Priority = {Priority}")]
    public sealed class CachingParameters
    {
        public static readonly CachingParameters NoCache = new CachingParameters(TimeSpan.Zero);


        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public CachingParameters(TimeSpan expiration,
                                 bool sliding = false,
                                 CachePriority? priority = null)
        {
            this.Expiration = expiration;
            this.Sliding = sliding;
            this.Priority = priority;
        }


        private CachingParameters([NotNull] CachingParameters other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            this.Expiration = other.Expiration;
            this.Sliding = other.Sliding;
            this.Priority = other.Priority;
        }


        /// <summary>
        ///     (GET) Expiration time.
        /// </summary>
        public TimeSpan Expiration { get; }


        /// <summary>
        ///     (GET) True if current parameters represents sliding expiration.
        /// </summary>
        public bool Sliding { get; }

        /// <summary>
        ///     (GET) Cached object priority.
        /// </summary>
        public CachePriority? Priority { get; }


        /// <summary>
        ///     (GET) Returns true if no expiration set for current parameters.
        /// </summary>
        public bool NoCaching => this.Expiration == TimeSpan.Zero;


        /// <summary>
        ///     Creates <see cref="CachingParameters" /> instance with expiration of <paramref name="milliseconds" />.
        /// </summary>
        public static CachingParameters FromMilliseconds(double milliseconds,
                                                         bool sliding = false,
                                                         CachePriority? priority = null)
        {
            return new CachingParameters(TimeSpan.FromMilliseconds(milliseconds), sliding, priority);
        }


        /// <summary>
        ///     Creates <see cref="CachingParameters" /> instance with expiration of <paramref name="seconds" />.
        /// </summary>
        public static CachingParameters FromSeconds(double seconds,
                                                    bool sliding = false,
                                                    CachePriority? priority = null)
        {
            return new CachingParameters(TimeSpan.FromSeconds(seconds), sliding, priority);
        }


        /// <summary>
        ///     Creates <see cref="CachingParameters" /> instance with expiration of <paramref name="minutes" />.
        /// </summary>
        public static CachingParameters FromMinutes(double minutes,
                                                    bool sliding = false,
                                                    CachePriority? priority = null)
        {
            return new CachingParameters(TimeSpan.FromMinutes(minutes), sliding, priority);
        }


        /// <summary>
        ///     Creates <see cref="CachingParameters" /> instance with expiration of <paramref name="hours" />.
        /// </summary>
        public static CachingParameters FromHours(double hours,
                                                  bool sliding = false,
                                                  CachePriority? priority = null)
        {
            return new CachingParameters(TimeSpan.FromHours(hours), sliding, priority);
        }


        /// <summary>
        ///     Creates <see cref="CachingParameters" /> instance with expiration of <paramref name="days" />.
        /// </summary>
        public static CachingParameters FromDays(double days,
                                                 bool sliding = false,
                                                 CachePriority? priority = null)
        {
            return new CachingParameters(TimeSpan.FromDays(days), sliding, priority);
        }


        /// <summary>
        ///     Performs deep clone of the current object instance.
        /// </summary>
        public CachingParameters Clone()
        {
            return new CachingParameters(this);
        }


        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        ///     A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            if (this.NoCaching)
            {
                return "no caching";
            }

            var result = (this.Sliding ? "sliding expiration " : "absolute expiration ") +
                         this.Expiration + ", priority " + this.Priority;

            return result;
        }
    }
}