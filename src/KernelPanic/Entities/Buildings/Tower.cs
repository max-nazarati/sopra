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

        protected Sprite mRadiusSprite;

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
            mRadiusSprite = spriteManager.CreateTowerRadiusIndicator(Radius);
        }

        [OnDeserialized]
        private void AfterDeserialization(StreamingContext context)
        {
            mRadiusSprite = SpriteManager.CreateTowerRadiusIndicator(Radius);
        }

        protected override void CompleteClone()
        {
            base.CompleteClone();
            mRadiusSprite = mRadiusSprite.Clone();
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
            base.DrawActions(spriteBatch, gameTime);

            mRadiusSprite.Position = Sprite.Position;
            mRadiusSprite.Draw(spriteBatch, gameTime);
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