using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace KernelPanic
{
    public class Base
    {
        public int Power { get; private set; }

        public Base()
        {
            Power = 50;
        }
    }
}