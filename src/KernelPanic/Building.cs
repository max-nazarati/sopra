using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KernelPanic
{
    internal class Building :Entity
    {

        public Building(int param) : base(param)
        {

        }
        public Building(TimeSpan timeSpan) : base(timeSpan)
        {

        }

        public int BitcoinWorth { get; set; }

        public State StateProperty { get; set; }
        
        public enum State { Active, Inactive, Invalid, Valid };
    }
}
