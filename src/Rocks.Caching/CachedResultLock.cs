using System.Threading;
using JetBrains.Annotations;

namespace Rocks.Caching
{
    internal class CachedResultLock
    {
        public CachedResultLock()
        {
            this.Mutex = new SemaphoreSlim(1, 1);
        }


        [NotNull]
        public SemaphoreSlim Mutex { get; }

        public bool IsExecuted { get; set; }
        public object Result { get; set; }
    }
}