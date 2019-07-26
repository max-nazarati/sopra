using KernelPanic.Camera;
using KernelPanic.Entities;
using KernelPanic.Input;
using KernelPanic.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using KernelPanic.Waves;

namespace KernelPanic.Hud
{
    internal sealed class InGameOverlay: AGameState
    {
        internal ScoreOverlay ScoreOverlay { get; }
        internal UnitBuyingMenu UnitBuyingMenu { get; }
        private readonly BuildingBuyingMenu mBuildingBuyingMenu;

        private readonly MinimapOverlay mMinimapOverlay;

        internal override bool IsOverlay => true;

        internal InGameOverlay(WaveManager waveManager,
            UnitBuyingMenu unitMenu,
            BuildingBuyingMenu buildingMenu,
            GameStateManager gameStateManager, TimeSpan time, ICamera camera)
            : base(new StaticCamera(), gameStateManager)
        {
            ScoreOverlay = new ScoreOverlay(waveManager, gameStateManager.Sprite, time);
            UnitBuyingMenu = unitMenu;
            mBuildingBuyingMenu = buildingMenu;
            mMinimapOverlay = new MinimapOverlay(waveManager.Players, gameStateManager.Sprite, camera);
        }

        public override void Update(InputManager inputManager,
            GameTime gameTime)
        {
            ScoreOverlay.Update(inputManager, gameTime);
            UnitBuyingMenu.Update(inputManager, gameTime);
            mBuildingBuyingMenu.Update(inputManager, gameTime);
            mMinimapOverlay.Update();
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            ScoreOverlay.Draw(spriteBatch, gameTime);
            UnitBuyingMenu.Draw(spriteBatch, gameTime);
            mBuildingBuyingMenu.Draw(spriteBatch, gameTime);
            mMinimapOverlay.Draw(spriteBatch, gameTime);
        }
    }
}
