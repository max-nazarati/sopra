using KernelPanic.Camera;
using KernelPanic.Entities;
using KernelPanic.Input;
using KernelPanic.Selection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    internal sealed class InGameOverlay: AGameState
    {
        private Entity mSelection;
        private readonly ScoreOverlay mScoreOverlay;
        private readonly UnitBuyingMenu mUnitBuyingMenu;
        // public BuildingBuyingMenu BuildingMenu { get; set; }
        // public UnitBuyingMenu UnitMenu { get; set; }

        internal override bool IsOverlay => true;

        public InGameOverlay(Player player1, Player player2, SelectionManager selectionManager, GameStateManager gameStateManager)
            : base(new StaticCamera(), gameStateManager)
        {
            // TODO: Add Button parameters

            mSelection = selectionManager.Selection;
            selectionManager.SelectionChanged += (oldSelection, newSelection) => mSelection = newSelection;
            
            mScoreOverlay = new ScoreOverlay(player1, player2, gameStateManager.Sprite);
            mUnitBuyingMenu = new UnitBuyingMenu(gameStateManager.Sprite, player1);
        }

        public override void Update(InputManager inputManager, GameTime gameTime)
        {
            mScoreOverlay.Update(gameTime);
            mUnitBuyingMenu.Update(inputManager, gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            mScoreOverlay.Draw(spriteBatch, gameTime);
            mUnitBuyingMenu.Draw(spriteBatch, gameTime);
        }
    }
}
