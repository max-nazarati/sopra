using System.Runtime.Serialization;
using System;
// using System.Data;
using KernelPanic.Data;
using KernelPanic.Entities.Projectiles;
// using KernelPanic.Events;
using KernelPanic.Input;
using KernelPanic.Sprites;
using KernelPanic.Table;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic.Entities.Buildings
{
    internal abstract class Tower : Building
    {
        [DataMember] internal float Radius { get; set; }
        [DataMember] internal int Damage { get; set; }

        [DataMember] internal int Speed { get; set; }
        [DataMember] internal CooldownComponent FireTimer { get; private set; }
        internal Action<Projectile> FireAction { get; set; }

        private Sprite RadiusSprite { get; set; }

        internal Tower(int price,
            float radius,
            int damage,
            int speed,
            TimeSpan cooldown,
            Sprite sprite,
            SpriteManager spriteManager) : base(price, sprite, spriteManager)
        {
            FireTimer = new CooldownComponent(cooldown, false);
            Radius = radius * Grid.KachelSize;
            Damage = damage;
            Speed = speed;
            sprite.ScaleToHeight(64);
            sprite.SetOrigin(RelativePosition.Center);
            RadiusSprite = CreateRadiusSprite();
        }

        [OnDeserialized]
        private void AfterDeserialization(StreamingContext context)
        {

            RadiusSprite = CreateRadiusSprite();
        }

        private Sprite CreateRadiusSprite()
        {
            return Radius > 0 ? SpriteManager.CreateTowerRadiusIndicator(Radius) : null;
        }

        protected override void CompleteClone()
        {
            base.CompleteClone();
            RadiusSprite = RadiusSprite?.Clone();
            FireTimer = new CooldownComponent(FireTimer.Cooldown, !FireTimer.Ready) { Enabled = FireTimer.Enabled };
        }

        public override void Update(PositionProvider positionProvider, InputManager inputManager, GameTime gameTime)
        {
            base.Update(positionProvider, inputManager, gameTime);

            if (State == BuildingState.Active)
                FireTimer.Update(gameTime);
        }

        internal override void DrawActions(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (RadiusSprite != null)
            {
                RadiusSprite.Position = Sprite.Position;
                RadiusSprite.Draw(spriteBatch, gameTime);
            }

            base.DrawActions(spriteBatch, gameTime);
        }
        
        internal void HitByEmp()
        {
            State = BuildingState.Disabled;
        }

        internal void EmpIsGone()
        {
            State = BuildingState.Active;
        }
    }
}