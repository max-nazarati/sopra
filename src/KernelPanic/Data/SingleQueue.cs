using System.Threading;

namespace KernelPanic.Data
{
    internal sealed class SingleQueue<T> where T : class
    {
        private T mValue;
        private readonly Semaphore mSemaphore;

        internal SingleQueue(string name)
        {
            mSemaphore = new Semaphore(0, 1, name);
        }

        internal T Take()
        {
            mSemaphore.WaitOne();
            return mValue;
        }

        internal void Put(T value)
        {
            Interlocked.Exchange(ref mValue, value);
            mSemaphore.Release();
        }
    }
}