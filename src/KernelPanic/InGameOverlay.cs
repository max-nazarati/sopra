using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    class InGameOverlay
    {
        private TimeSpan mPlayDuration;
        private IEntitySelection mSelection;
        private ScoreOverlay mScoreOvelay;
        //public BuildingBuyingMenu BuildingMenu { get; private set; }
        public Player PlayerA { get; set; }
        public Player PlayerB { get; set; }
        /*public BuildingBuyingMenu BuildingMenu { get; set; }
        public UnitBuyingMenu UnitMenu { get; set; }*/

        public InGameOverlay(Player player1, Player player2)
        {
            // TODO: Add SelectionManager and Button parameters
            PlayerA = player1;
            PlayerB = player2;
            mScoreOvelay = new ScoreOverlay(PlayerA, PlayerB);
            //BuildingMenu = new BuildingBuyingMenu(0, 50);
            mPlayDuration = mScoreOvelay.Time;
        }

        public void Update(GameTime gameTime)
        {
            mScoreOvelay.Update(gameTime);
            mPlayDuration = mScoreOvelay.Time;
        }
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // Should not be drawn in the same SpriteBatch as Camera2d
            spriteBatch.Begin();
            mScoreOvelay.Draw(spriteBatch, gameTime);
            //BuildingMenu.Draw(spriteBatch, gameTime);
            spriteBatch.End();
        }
    }
}
