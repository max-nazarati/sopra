using System;

namespace KernelPanic.Data
{
    internal sealed class WeakReference<T> : WeakReference where T : class
    {
        private WeakReference(T value) : base(value)
        {
        }

        internal new T Target => (T)base.Target;

        public static implicit operator WeakReference<T>(T value)
        {
            return new WeakReference<T>(value);
        }
    }
}
