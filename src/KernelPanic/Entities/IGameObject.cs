using KernelPanic.Data;
using KernelPanic.Input;
using Microsoft.Xna.Framework;

namespace KernelPanic.Entities
{
    internal interface IGameObject : IBounded, IDrawable
    {
        /// <summary>
        /// In which layer should this game object be drawn? Lower values are drawn first, returning <c>null</c>
        /// won't draw this object.
        /// </summary>
        int? DrawLevel { get; }

        bool WantsRemoval { get; }
    
        void Update(PositionProvider positionProvider, InputManager inputManager, GameTime gameTime);
    }
}
