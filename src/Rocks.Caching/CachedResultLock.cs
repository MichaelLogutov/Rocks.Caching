using System;
using System.Threading;

namespace Rocks.Caching
{
	internal class CachedResultLock
	{
		#region Private fields

		private ManualResetEventSlim executing;
		private int executed;
		private object result;

		#endregion

		#region Public properties

		public bool Executed { get { return this.executed == 1; } }
		public object Result { get { return this.result; } }

		#endregion

		#region Public methods

		public bool TryStartExecuting ()
		{
			var previous_value = Interlocked.CompareExchange (ref this.executing, new ManualResetEventSlim (), null);
			return previous_value == null;
		}


		public void EndExecution (object res)
		{
			var e = this.executing;
			if (e == null)
				throw new InvalidOperationException ("Executing event is null");

			if (Interlocked.CompareExchange (ref this.executed, 1, 0) != 0)
				throw new InvalidOperationException ("Unexpected value change: Executed");

			if (Interlocked.CompareExchange (ref this.result, res, null) != null)
				throw new InvalidOperationException ("Unexpected value change: Result");

			e.Set ();

			if (!ReferenceEquals (Interlocked.CompareExchange (ref this.executing, null, e), e))
				throw new InvalidOperationException ("Unexpected value change: executing");
		}


		public void WaitForCompletion ()
		{
			if (this.executing == null)
				throw new InvalidOperationException ("Executing was not started");

			this.executing.Wait ();
		}

		#endregion
	}
}