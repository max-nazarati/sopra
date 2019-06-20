using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace KernelPanic
{
    class BuyingMenuOverlay
    {
        internal IEnumerable<IDrawable> Drawables{ get; private set; }
        internal IEnumerable<IUpdatable> Updatables { get; private set; }
        protected Sprites.ImageSprite background;
    }
}
