using KernelPanic.Input;
using Microsoft.Xna.Framework;

namespace KernelPanic
{
    internal interface IUpdatable
    {
        void Update(InputManager inputManager, GameTime gameTime);
    }
}
