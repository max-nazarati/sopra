using System;

namespace KernelPanic
{
    internal interface IPlayerDistinction
    {
        T Select<T>(T ifActive, T ifPassive);
    }
}