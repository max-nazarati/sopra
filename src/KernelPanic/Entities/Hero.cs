﻿
using System;
using Microsoft.Xna.Framework;

namespace KernelPanic
{
    internal class Hero : Unit
    {
        //public CooldownComponent Cooldown { get; set; }

        public Hero(int price, int speed, int life, int attackStrength, Sprite sprite) : base(price, speed, life, attackStrength, sprite)
        {
        }

        public bool AbilityAvailable()
        {
            return false;
        }

        public void ActivateAbility()
        {
            throw new NotImplementedException();
        }

        internal override void Update(GameTime gameTime, Matrix invertedViewMatrix)
        {
            throw new NotImplementedException();
        }
    }
}
