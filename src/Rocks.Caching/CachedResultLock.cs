using System.Threading;
using JetBrains.Annotations;

namespace Rocks.Caching
{
	internal class CachedResultLock
	{
		#region Private fields

		private readonly SemaphoreSlim mutex;

		#endregion

		#region Construct

		public CachedResultLock ()
		{
			this.mutex = new SemaphoreSlim (1, 1);
		}

		#endregion

		#region Public properties

		[NotNull]
		public SemaphoreSlim Mutex { get { return this.mutex; } }

		public bool IsExecuted { get; set; }
		public object Result { get; set; }

		#endregion
	}
}