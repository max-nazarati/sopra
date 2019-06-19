using System.Runtime.Serialization;
﻿using KernelPanic.Sprites;
using KernelPanic.Table;
using Microsoft.Xna.Framework;

namespace KernelPanic.Entities
{
    internal abstract class Troupe : Unit
    {
        protected Troupe(int price, int speed, int life, int attackStrength, Sprite sprite, SpriteManager spriteManager)
            : base(price, speed, life, attackStrength, sprite, spriteManager)
        {
        }
    }
}
