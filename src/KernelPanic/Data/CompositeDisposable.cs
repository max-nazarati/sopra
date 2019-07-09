using System;
using System.Collections.Generic;

namespace KernelPanic.Data
{
/*
    internal struct CompositeDisposable : IDisposable
    {
        private List<IDisposable> mDisposables;

        private CompositeDisposable Add(IDisposable disposable)
        {
            if (mDisposables == null)
                mDisposables = new List<IDisposable>();
            mDisposables.Add(disposable);
            return this;
        }

        public static CompositeDisposable operator +(CompositeDisposable compositeDisposable, IDisposable disposable)
        {
            return compositeDisposable.Add(disposable);
        }

        public void Dispose()
        {
            if (mDisposables == null)
                return;

            foreach (var disposable in mDisposables)
            {
                disposable.Dispose();
            }

            mDisposables.Clear();
        } 
    }
    */
}