using System;

namespace KernelPanic.Data
{
    internal sealed class WeakReference<T> : WeakReference where T : class
    {
        internal WeakReference(T value) : base(value)
        {
        }

        internal new T Target
        {
            get => (T)base.Target;
            set => base.Target = value;
        }

        public static implicit operator WeakReference<T>(T value)
        {
            return new WeakReference<T>(value);
        }
    }
}
