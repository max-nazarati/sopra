using KernelPanic.Camera;
using KernelPanic.Entities;
using KernelPanic.Input;
using KernelPanic.Players;
using KernelPanic.Selection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace KernelPanic.Hud
{
    internal sealed class InGameOverlay: AGameState
    {
        internal ScoreOverlay ScoreOverlay { get; }

        private readonly UnitBuyingMenu mUnitBuyingMenu;
        private readonly BuildingBuyingMenu mBuildingBuyingMenu;

        private Entity mSelection;
        private readonly MinimapOverlay mMinimapOverlay;

        internal override bool IsOverlay => true;

        internal InGameOverlay(PlayerIndexed<Player> players,
            UnitBuyingMenu unitMenu,
            BuildingBuyingMenu buildingMenu,
            SelectionManager selectionManager,
            GameStateManager gameStateManager, TimeSpan time, ICamera camera)
            : base(new StaticCamera(), gameStateManager)
        {
            // TODO: Add Button parameters
            mSelection = selectionManager.Selection;
            selectionManager.SelectionChanged += (oldSelection, newSelection) => mSelection = newSelection;

            ScoreOverlay = new ScoreOverlay(players, gameStateManager.Sprite, time);
            mUnitBuyingMenu = unitMenu;
            mBuildingBuyingMenu = buildingMenu;
            mMinimapOverlay = new MinimapOverlay(players, gameStateManager.Sprite, camera);
        }

        public override void Update(InputManager inputManager,
            GameTime gameTime)
        {
            ScoreOverlay.Update(inputManager, gameTime);
            mUnitBuyingMenu.Update(inputManager, gameTime);
            mBuildingBuyingMenu.Update(inputManager, gameTime);
            mMinimapOverlay.Update();
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            ScoreOverlay.Draw(spriteBatch, gameTime);
            mUnitBuyingMenu.Draw(spriteBatch, gameTime);
            mBuildingBuyingMenu.Draw(spriteBatch, gameTime);
            mMinimapOverlay.Draw(spriteBatch, gameTime);
        }
    }
}
