using KernelPanic.Data;
using KernelPanic.Input;
using Microsoft.Xna.Framework;

namespace KernelPanic.Entities
{
    internal interface IGameObject : IBounded, IDrawable
    {
        bool WantsRemoval { get; }
    
        void Update(PositionProvider positionProvider, InputManager inputManager, GameTime gameTime);
    }
}
