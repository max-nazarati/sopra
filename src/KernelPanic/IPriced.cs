using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KernelPanic
{
    interface IPriced
    {
        Currency Currency
        {
            get;
        }

        int Price
        {
            get;
        }
    }

    enum Currency { Bitcoin, Experience };
}
