using KernelPanic.Entities.Buildings;
using KernelPanic.Input;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;

namespace KernelPanic.Entities.Projectiles
{
    internal sealed class Wifi : Projectile
    {
        WifiRouter mTower;
        internal Wifi(Tower origin, Vector2 direction, Sprite sprite, WifiRouter tower, float offset = 0) : base(origin, direction, sprite, offset)
        {
            mTower = tower;
            Sprite.Scale = 0;
        }
        
        public override void Update(PositionProvider positionProvider, InputManager inputManager, GameTime gameTime)
        {
            base.Update(positionProvider, inputManager, gameTime);
            var dist = Vector2.Distance(mTower.Sprite.Position, Sprite.Position);
            Sprite.Scale = dist * 0.015f;
        }
    }
}