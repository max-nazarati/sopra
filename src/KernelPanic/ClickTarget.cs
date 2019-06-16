using System;
using KernelPanic.Data;
using Microsoft.Xna.Framework;

namespace KernelPanic
{
    /// <summary>
    /// Associates a rectangle with an action which is invoked when the rectangle is clicked.
    /// </summary>
    internal sealed class ClickTarget : IBounded
    {
        public Rectangle Bounds { get; }
        public Action Action { get; }

        internal ClickTarget(Rectangle bounds, Action action)
        {
            Bounds = bounds;
            Action = action;
        }
    }
}
