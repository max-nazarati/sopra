using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace KernelPanic
{
    class PlayTime
    {
        public TimeSpan Overall { get; private set; }
        public string Time { get; private set; }
        
        public PlayTime()
        {
            Time = "";
        }
        public void Update(GameTime gameTime)
        {
            Overall = Overall.Add(gameTime.ElapsedGameTime);
            Console.WriteLine(Time);
            Time = Overall.ToString("h':'m':'s");
        }
    }
}
