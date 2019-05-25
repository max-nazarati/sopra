using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    class StaticComponent : UIComponent
    {
        public StaticComponent(Sprite sprite)
        {
            Sprite = sprite;
        }
    }
}
