﻿using KernelPanic.Camera;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    internal sealed class InGameOverlay: AGameState
    {
        private IEntitySelection mSelection;
        private readonly ScoreOverlay mScoreOverlay;
        // public BuildingBuyingMenu BuildingMenu { get; set; }
        // public UnitBuyingMenu UnitMenu { get; set; }

        internal override bool IsOverlay => true;

        public InGameOverlay(Player player1, Player player2, GameStateManager gameStateManager)
            : base(new StaticCamera(), gameStateManager)
        {
            // TODO: Add SelectionManager and Button parameters
            mScoreOverlay = new ScoreOverlay(player1, player2, gameStateManager.Sprite);
            // BuildingMenu = new BuildingBuyingMenu(0, 50);
        }

        public override void Update(InputManager inputManager, GameTime gameTime)
        {
            mScoreOverlay.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            mScoreOverlay.Draw(spriteBatch, gameTime);
            // BuildingMenu.Draw(spriteBatch, gameTime);
        }
    }
}
