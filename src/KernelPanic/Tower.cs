using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace KernelPanic
{
    class Tower : Building
    {        
        public Tower(TimeSpan timeSpan):base(timeSpan)
        {

        }
        /*public new void Update(GameTime gameTime, IPositionProvider positionProvider)
        {

        }*/

        //public CooldownComponent Cooldown { get; set; }
        public int Level { get; set; }
        public float? Radius { get; set; }
    }
}
