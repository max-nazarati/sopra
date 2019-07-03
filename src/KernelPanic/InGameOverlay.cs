using KernelPanic.Camera;
using KernelPanic.Entities;
using KernelPanic.Input;
using KernelPanic.Selection;
using KernelPanic.Waves;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    internal sealed class InGameOverlay: AGameState
    {
        internal ScoreOverlay ScoreOverlay { get; }
        internal UnitBuyingMenu UnitBuyingMenu { get; }
        internal BuildingBuyingMenu BuildingBuyingMenu { get; }

        private Entity mSelection;
        private readonly MinimapOverlay mMinimapOverlay;

        internal override bool IsOverlay => true;

        public InGameOverlay(WaveManager waveManager, SelectionManager selectionManager, GameStateManager gameStateManager)
            : base(new StaticCamera(), gameStateManager)
        {
            // TODO: Add Button parameters
            mSelection = selectionManager.Selection;
            selectionManager.SelectionChanged += (oldSelection, newSelection) => mSelection = newSelection;
            
            ScoreOverlay = new ScoreOverlay(waveManager.Players, gameStateManager.Sprite);
            UnitBuyingMenu = UnitBuyingMenu.Create(waveManager, gameStateManager.Sprite);
            BuildingBuyingMenu = BuildingBuyingMenu.Create(waveManager.Players.B, gameStateManager.Sprite, mSelection, gameStateManager.Sound);
            mMinimapOverlay = new MinimapOverlay(waveManager.Players, gameStateManager.Sprite);
        }

        public override void Update(InputManager inputManager,
            GameTime gameTime,
            SoundManager soundManager,
            GraphicsDeviceManager graphics)
        {
            ScoreOverlay.Update(inputManager, gameTime);
            UnitBuyingMenu.Update(inputManager, gameTime);
            BuildingBuyingMenu.Update(inputManager, gameTime);
            mMinimapOverlay.Update(inputManager, gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            ScoreOverlay.Draw(spriteBatch, gameTime);
            UnitBuyingMenu.Draw(spriteBatch, gameTime);
            BuildingBuyingMenu.Draw(spriteBatch, gameTime);
            mMinimapOverlay.Draw(spriteBatch, gameTime);
        }
    }
}
