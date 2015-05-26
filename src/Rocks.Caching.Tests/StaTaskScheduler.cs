using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rocks.Caching.Tests
{
    /// <summary>
    ///     Provides a scheduler that uses STA threads.
    /// </summary>
    public sealed class StaTaskScheduler : TaskScheduler, IDisposable
    {
        #region Static fields

        private static readonly TaskFactory defaultTaskFactory = new TaskFactory
            (CancellationToken.None,
             TaskCreationOptions.DenyChildAttach,
             TaskContinuationOptions.None,
             new StaTaskScheduler (Environment.ProcessorCount));

        #endregion

        #region Private readonly fields

        /// <summary>
        ///     The STA threads used by the scheduler.
        /// </summary>
        private readonly IReadOnlyList<Thread> threads;

        #endregion

        #region Private fields

        /// <summary>
        ///     Stores the queued tasks to be executed by our pool of STA threads.
        /// </summary>
        private ConcurrentQueue<Task> tasks;

        private bool disposing;

        #endregion

        #region Construct

        /// <summary>
        ///     Initializes a new instance of the StaTaskScheduler class with the specified concurrency level.
        /// </summary>
        /// <param name="numberOfThreads">The number of threads that should be created and used by this scheduler.</param>
        public StaTaskScheduler (int? numberOfThreads = null)
        {
            if (numberOfThreads == null || numberOfThreads < 1)
                numberOfThreads = Environment.ProcessorCount;

            this.tasks = new ConcurrentQueue<Task> ();


            this.threads = Enumerable.Range (0, numberOfThreads.Value)
                                     .Select (i => this.CreateWorkThread ())
                                     .ToList ();


            foreach (var thread in this.threads)
                thread.Start ();
        }

        #endregion

        #region Public properties

        public static TaskFactory DefaultTaskFactory
        {
            get { return defaultTaskFactory; }
        }

        /// <summary>
        ///     Gets the maximum concurrency level supported by this scheduler.
        /// </summary>
        public override int MaximumConcurrencyLevel
        {
            get { return this.threads.Count; }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        ///     Cleans up the scheduler by indicating that no more tasks will be queued.
        ///     This method blocks until all threads successfully shutdown.
        /// </summary>
        public void Dispose ()
        {
            if (this.tasks != null)
            {
                this.disposing = true;

                // Wait for all threads to finish processing tasks
                foreach (var thread in this.threads)
                    thread.Join ();

                this.tasks = null;
            }
        }

        #endregion

        #region Protected methods

        /// <summary>
        ///     Queues a Task to be executed by this scheduler.
        /// </summary>
        /// <param name="task">The task to be executed.</param>
        protected override void QueueTask (Task task)
        {
            this.tasks.Enqueue (task);
        }


        /// <summary>
        ///     Provides a list of the scheduled tasks for the debugger to consume.
        /// </summary>
        /// <returns>An enumerable of all tasks currently scheduled.</returns>
        protected override IEnumerable<Task> GetScheduledTasks ()
        {
            // Serialize the contents of the blocking collection of tasks for the debugger
            return this.tasks.ToArray ();
        }


        /// <summary>
        ///     Determines whether a Task may be inlined.
        /// </summary>
        /// <param name="task">The task to be executed.</param>
        /// <param name="taskWasPreviouslyQueued">Whether the task was previously queued.</param>
        /// <returns>true if the task was successfully inlined; otherwise, false.</returns>
        protected override bool TryExecuteTaskInline (Task task, bool taskWasPreviouslyQueued)
        {
            // Try to inline if the current thread is STA
            return Thread.CurrentThread.GetApartmentState () == ApartmentState.STA && this.TryExecuteTask (task);
        }

        #endregion

        #region Private methods

        private Thread CreateWorkThread ()
        {
            var thread = new Thread (() =>
                                     {
                                         while (!this.disposing)
                                         {
                                             Task task;
                                             if (this.tasks.TryDequeue (out task))
                                                 this.TryExecuteTask (task);
                                         }
                                     });

            thread.IsBackground = true;
            thread.SetApartmentState (ApartmentState.STA);

            return thread;
        }

        #endregion
    }
}