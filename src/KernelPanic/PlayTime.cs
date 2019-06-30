﻿using System;
using Microsoft.Xna.Framework;

namespace KernelPanic
{
    internal sealed class PlayTime
    {
        public TimeSpan Overall { get; private set; } = TimeSpan.Zero;
        public string Time { get; private set; } = "";

        public void Update(GameTime gameTime)
        {
            Overall = Overall.Add(gameTime.ElapsedGameTime);
            Time = Overall.ToString("hh':'mm':'ss");
        }
    }
}