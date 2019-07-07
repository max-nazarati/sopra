using KernelPanic.Camera;
using KernelPanic.Events;
using KernelPanic.Input;
using KernelPanic.Selection;
using KernelPanic.Serialization;
using KernelPanic.Table;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace KernelPanic
{
    internal sealed class InGameState : AGameState
    {
        private readonly Board mBoard;
        private readonly SelectionManager mSelectionManager;
        private readonly BuildingBuyer mBuildingBuyer;
        private readonly InGameOverlay mHud;

        internal int SaveSlot { get; }

        private InGameState(Storage? storage, int saveSlot, GameStateManager gameStateManager)
            : base(new Camera2D(Board.Bounds, gameStateManager.Sprite.ScreenSize), gameStateManager)
        {
            mBoard = storage?.Board ?? new Board(gameStateManager.Sprite);
            mBuildingBuyer = new BuildingBuyer(mBoard.PlayerA, gameStateManager.Sound);
            mSelectionManager = new SelectionManager(mBoard.LeftLane, mBoard.RightLane, gameStateManager.Sprite);
            SaveSlot = saveSlot;

            var unitMenu = UnitBuyingMenu.Create(mBoard.WaveManager, gameStateManager.Sprite);
            var buildingMenu = BuildingBuyingMenu.Create(mBuildingBuyer, gameStateManager.Sprite, gameStateManager.Sound);
            mHud = new InGameOverlay(mBoard.WaveManager.Players, unitMenu, buildingMenu, mSelectionManager, gameStateManager);

            mBoard.PlayerB.InitializePlanners(
                unitMenu.BuyingActions, // TODO implement this, just added it like this so i can build :)
                upgradeId => mBoard.mUpgradePool[upgradeId],
                gameStateManager.Sprite,
                gameStateManager.Sound);
        }

        internal static void PushGameStack(int saveSlot, GameStateManager gameStateManager, Storage? storage = null)
        {
            var game = new InGameState(storage, saveSlot, gameStateManager);
            gameStateManager.Restart(game);
            gameStateManager.Push(game.mHud);
        }

        public override void Update(InputManager inputManager, GameTime gameTime, SoundManager soundManager)
        {
            if (inputManager.KeyPressed(Keys.Escape) || !inputManager.IsActive || mHud.ScoreOverlay.Pause)
            {
                GameStateManager.Push(MenuState.CreatePauseMenu(GameStateManager, this, soundManager));
                mHud.ScoreOverlay.Pause = false;
                return;
            }

            // Update the overall play time.
            GameStateManager.Statistics.Update(gameTime);

            mBuildingBuyer.Update(inputManager);
            mSelectionManager.Update(inputManager);

            mBoard.Update(gameTime, inputManager);
            var gameState = mBoard.CheckGameState();
            if (gameState == Board.GameState.Playing)
                return;

            EventCenter.Default.Send(gameState == Board.GameState.AWon ? Event.GameWon() : Event.GameLost());
            GameStateManager.Restart(MenuState.CreateMainMenu(GameStateManager, soundManager));
            GameStateManager.Push(MenuState.CreateGameOverScreen(GameStateManager, gameState, soundManager));
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            mBoard.Draw(spriteBatch, gameTime);
            mBuildingBuyer.Draw(spriteBatch, gameTime);
            mSelectionManager.Draw(spriteBatch, gameTime);
        }

        #region Serialization

        internal Storage Data => new Storage
        {
            Board = mBoard,
            GameTime = mHud.ScoreOverlay.Time
        };

        #endregion

    }
}
