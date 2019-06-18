using System;
using System.Collections.Generic;
using KernelPanic.Data;
using KernelPanic.Input;
using KernelPanic.Interface;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;

namespace KernelPanic.Entities
{
    internal class StrategicTower : Tower
    {
        private TowerStrategy mStrategy = TowerStrategy.First;

        /*internal*/ private TowerStrategy Strategy
        {
            get => mStrategy;
            set { Console.WriteLine("Changing Tower strategy from {0} to {1}", mStrategy, value); mStrategy = value; }
        }

        internal StrategicTower(int price, float radius, TimeSpan cooldown, Sprite sprite, SpriteManager sprites, SoundManager sounds)
            : base(price, radius, cooldown, sprite, sprites, sounds)
        {
        }

        #region Actions

        protected override IEnumerable<IAction> Actions =>
            base.Actions.Extend(
                new StrategyAction(this, TowerStrategy.First, SpriteManager),
                new StrategyAction(this, TowerStrategy.Strongest, SpriteManager),
                new StrategyAction(this, TowerStrategy.Weakest, SpriteManager)
            );

        private sealed class StrategyAction : BaseAction<Button>
        {
            internal StrategyAction(StrategicTower tower, TowerStrategy strategy, SpriteManager spriteManager)
                : base(new Button(spriteManager) {Title = strategy.ToString(), Enabled = tower.Strategy != strategy})
            {
                Provider.Clicked += (button, input) => tower.Strategy = strategy;
            }

            public override void MoveTo(Vector2 position)
            {
                Provider.Sprite.Position = position;
            }
        }

        #endregion
    }

    internal enum TowerStrategy { First, Strongest, Weakest };
}