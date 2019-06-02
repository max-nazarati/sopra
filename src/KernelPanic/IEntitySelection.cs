using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KernelPanic
{
    interface IEntitySelection
    {
        void Match<T>(Func<T> param1, Func<Entity, T> param2, Func<Entity, T> param3);
    }
}
