using System;
using KernelPanic.Entities;

namespace KernelPanic.Waves
{
    internal struct WaveReference
    {
        internal int Index { get; }
        internal Action<Unit> SpawnChild { get; }

        internal WaveReference(int index, Action<Unit> spawnChild)
        {
            Index = index;
            SpawnChild = spawnChild;
        }
    }
}
