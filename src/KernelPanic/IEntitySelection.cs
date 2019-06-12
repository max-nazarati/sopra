using System;
using KernelPanic.Entities;

namespace KernelPanic
{
    interface IEntitySelection
    {
        void Match<T>(Func<T> param1, Func<Entity, T> param2, Func<Entity, T> param3);
    }
}
