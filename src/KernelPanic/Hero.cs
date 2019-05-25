using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KernelPanic
{
    class Hero : Unit
    {
        public Hero(int param) : base(param)
        {

        }
        //public CooldownComponent Cooldown { get; set; }
        public bool AbilityAvailable()
        {
            return false;
        }

        public void ActivateAbility()
        {

        }
    }
}
