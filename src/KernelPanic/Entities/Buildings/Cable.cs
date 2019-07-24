using System.Diagnostics.CodeAnalysis;
using KernelPanic.Input;
using Microsoft.Xna.Framework;

namespace KernelPanic.Entities.Buildings
{
    // This is instantiated via black magic originating from Building.Create.
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    internal sealed class Cable : Building
    {
        internal Cable(SpriteManager spriteManager)
            : base(10, spriteManager.CreateCable(), spriteManager)
        {
        }

        public override void Update(PositionProvider positionProvider, InputManager inputManager, GameTime gameTime)
        {
            // Nothing to do.
        }
    }
}
