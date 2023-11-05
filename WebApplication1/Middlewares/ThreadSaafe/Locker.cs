using System.Collections.Concurrent;

namespace WebApplication1.Middlewares.ThreadSaafe
{
    public class KeyedLock<TKey>
    {
        private readonly ConcurrentDictionary<TKey, LockInfo> _locks = new();

        public int Count => _locks.Count;

        public async Task<IDisposable> WaitAsync(TKey key, CancellationToken cancellationToken = default)
        {
            // Get the current info or create a new one.
            var info = _locks.AddOrUpdate(key,
                // Add
                k => new LockInfo(),
                // Update
                (k, v) => v.Enter() ? v : new LockInfo());

            try
            {
                await info.Semaphore.WaitAsync(cancellationToken);

                return new Releaser(() => Release(key, info, true));
            }
            catch (OperationCanceledException)
            {
                // The semaphore wait was cancelled, release the lock.
                Release(key, info, false);
                throw;
            }
        }

        private void Release(TKey key, LockInfo info, bool isCurrentlyLocked)
        {
            if (info.Leave())
            {
                // This was the last lock for the key.

                // Only remove this exact info, in case another thread has
                // already put its own info into the dictionary
                // Note that this call to Remove(entry) is in fact thread safe.
                var entry = new KeyValuePair<TKey, LockInfo>(key, info);
                if (((ICollection<KeyValuePair<TKey, LockInfo>>)_locks).Remove(entry))
                {
                    // This exact info was removed.
                    info.Dispose();
                }
            }
            else if (isCurrentlyLocked)
            {
                // There is another waiter.
                info.Semaphore.Release();
            }
        }

        private class LockInfo : IDisposable
        {
            private SemaphoreSlim _semaphore = null;
            private int _refCount = 1;

            public SemaphoreSlim Semaphore
            {
                get
                {
                    // Lazily create the semaphore.
                    var s = _semaphore;
                    if (s is null)
                    {
                        s = new SemaphoreSlim(1, 1);

                        // Assign _semaphore if its current value is null.
                        var original = Interlocked.CompareExchange(ref _semaphore, s, null);

                        // If someone else already created a semaphore, return that one
                        if (original is not null)
                        {
                            s.Dispose();
                            return original;
                        }
                    }
                    return s;
                }
            }

            // Returns true if successful
            public bool Enter()
            {
                if (Interlocked.Increment(ref _refCount) > 1)
                {
                    return true;
                }

                // This lock info is not valid anymore - its semaphore is or will be disposed.
                return false;
            }

            // Returns true if this lock info is now ready for removal
            public bool Leave()
            {
                if (Interlocked.Decrement(ref _refCount) <= 0)
                {
                    // This was the last lock
                    return true;
                }

                // There is another waiter
                return false;
            }

            public void Dispose() => _semaphore?.Dispose();
        }

        private sealed class Releaser : IDisposable
        {
            private readonly Action _dispose;

            public Releaser(Action dispose) => _dispose = dispose;

            public void Dispose() => _dispose();
        }
    }
}
