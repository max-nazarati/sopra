using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    internal sealed class InGameOverlay
    {
        private IEntitySelection mSelection;
        private readonly ScoreOverlay mScoreOverlay;
        // public BuildingBuyingMenu BuildingMenu { get; set; }
        // public UnitBuyingMenu UnitMenu { get; set; }

        public InGameOverlay(Player player1, Player player2, SpriteManager spriteManager)
        {
            // TODO: Add SelectionManager and Button parameters
            mScoreOverlay = new ScoreOverlay(player1, player2, spriteManager);
            // BuildingMenu = new BuildingBuyingMenu(0, 50);
        }

        public void Update(GameTime gameTime)
        {
            mScoreOverlay.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // Should not be drawn in the same SpriteBatch as Camera2d
            spriteBatch.Begin();
            mScoreOverlay.Draw(spriteBatch, gameTime);
            // BuildingMenu.Draw(spriteBatch, gameTime);
            spriteBatch.End();
        }
    }
}
