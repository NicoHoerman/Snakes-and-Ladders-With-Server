using Shared.Contract;
using System;
using System.Collections.Generic;
using System.Threading;

namespace TCP_Server.Support
{
    public class PackageQueue
    {
        private Queue<IPackage> _queue;
        private readonly object _lock;
        private Semaphore _semaphore;

        public PackageQueue()
        {
            _queue = new Queue<IPackage>();
            _lock = new object();
            _semaphore = new Semaphore(0, 1000000);
        }

        public void Push(IPackage package)
        {
            lock (_lock)
            {
                //Console.WriteLine($"Enqueueing package number {package.Id}");
                _queue.Enqueue(package);
            }
            _semaphore.Release();
        }

        public IPackage WaitForNextPackage()
        {
            _semaphore.WaitOne();
            lock (_lock)
            {
                var package = _queue.Dequeue();
                //Console.WriteLine($"Dequeueing package number {package.Id}");
                return package;
            }
        }

        public int Count { get { lock (_lock) return _queue.Count; } }

    }
}
