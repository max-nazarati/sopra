using System;
using System.Collections.Generic;
using KernelPanic.Data;
using KernelPanic.Input;
using KernelPanic.Interface;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace KernelPanic.Entities
{
    internal class StrategicTower : Tower
    {
        [JsonProperty]
        private TowerStrategy mStrategy = TowerStrategy.First;

        internal StrategicTower(int price, float radius, TimeSpan cooldown, Sprite sprite, SpriteManager sprites, SoundManager sounds)
            : base(price, radius, cooldown, sprite, sprites, sounds)
        {
        }

        internal StrategicTower(SpriteManager spriteManager, SoundManager soundManager)
            : this(0, 0, TimeSpan.Zero, spriteManager.CreateTower(), spriteManager, soundManager)
        {
            // TODO: The sprite in the constructor delegation has to be adjusted! Possibly by making subclasses
            // of this class and adding them in StorageManager.CreateContractResolver.
        }

        #region Actions

        protected override IEnumerable<IAction> Actions(Player owner) =>
            base.Actions(owner).Extend(
                new StrategyAction(this, TowerStrategy.First, SpriteManager),
                new StrategyAction(this, TowerStrategy.Strongest, SpriteManager),
                new StrategyAction(this, TowerStrategy.Weakest, SpriteManager)
            );

        private sealed class StrategyAction : IAction
        {
            public Button Button { get; }
            private readonly Func<bool> mIsCurrentStrategy;

            internal StrategyAction(StrategicTower tower, TowerStrategy strategy, SpriteManager spriteManager)
            {
                Button = new TextButton(spriteManager) {Title = strategy.ToString()};
                Button.Clicked += (button, input) => tower.mStrategy = strategy;
                mIsCurrentStrategy = () => tower.mStrategy == strategy;
            }

            void IUpdatable.Update(InputManager inputManager, GameTime gameTime)
            {
                Button.Enabled = !mIsCurrentStrategy();
                Button.Update(inputManager, gameTime);
            }

            void IDrawable.Draw(SpriteBatch spriteBatch, GameTime gameTime) =>
                Button.Draw(spriteBatch, gameTime);
        }

        #endregion
    }

    [JsonConverter(typeof(StringEnumConverter))]
    internal enum TowerStrategy { First, Strongest, Weakest };
}