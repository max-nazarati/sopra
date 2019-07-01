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
        internal Entity mSelection;
        internal readonly ScoreOverlay mScoreOverlay;
        private readonly UnitBuyingMenu mUnitBuyingMenu;
        private readonly BuildingBuyingMenu mBuildingBuyingMenu;
        private readonly MinimapOverlay mMinimapOverlay;
        // public BuildingBuyingMenu BuildingMenu { get; set; }
        // public UnitBuyingMenu UnitMenu { get; set; }

        internal override bool IsOverlay => true;

        public InGameOverlay(WaveManager waveManager, SelectionManager selectionManager, GameStateManager gameStateManager)
            : base(new StaticCamera(), gameStateManager)
        {
            // TODO: Add Button parameters
            mSelection = selectionManager.Selection;
            selectionManager.SelectionChanged += (oldSelection, newSelection) => mSelection = newSelection;
            
            mScoreOverlay = new ScoreOverlay(waveManager.Players, gameStateManager.Sprite);
            mUnitBuyingMenu = UnitBuyingMenu.Create(waveManager, gameStateManager.Sprite);
            mBuildingBuyingMenu = BuildingBuyingMenu.Create(waveManager.Players.B, gameStateManager.Sprite, mSelection, gameStateManager.Sound);
            mMinimapOverlay = new MinimapOverlay(waveManager.Players, gameStateManager.Sprite);
        }

        public override void Update(InputManager inputManager, GameTime gameTime, SoundManager soundManager
            , GraphicsDeviceManager mGraphics)
        {
            mScoreOverlay.Update(inputManager, gameTime);
            mUnitBuyingMenu.Update(inputManager, gameTime);
            mBuildingBuyingMenu.Update(inputManager, gameTime);
            mMinimapOverlay.Update(inputManager, gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            mScoreOverlay.Draw(spriteBatch, gameTime);
            mUnitBuyingMenu.Draw(spriteBatch, gameTime);
            mBuildingBuyingMenu.Draw(spriteBatch, gameTime);
            mMinimapOverlay.Draw(spriteBatch, gameTime);
        }
    }
}
