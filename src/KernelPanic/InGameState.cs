using KernelPanic.Camera;
using KernelPanic.Data;
using KernelPanic.Entities;
using KernelPanic.Input;
using KernelPanic.Interface;
using KernelPanic.Players;
using KernelPanic.Purchasing;
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
        private BuildingBuyer mBuildingBuyer;
        private InGameOverlay mHud;

        internal int SaveSlot { get; }

        // public SaveGame CurrentSaveGame { get; private set; } no such class yet
        // private HashSet<Wave> mActiveWaves;
        // private SelectionManager mSelectionManager;

        private InGameState(Storage? storage, int saveSlot, GameStateManager gameStateManager)
            : base(new Camera2D(Board.Bounds, gameStateManager.Sprite.ScreenSize), gameStateManager)
        {
            mBoard = storage?.Board ?? new Board(gameStateManager.Sprite, gameStateManager.Sound);
            mBuildingBuyer = new BuildingBuyer(mBoard.PlayerA, gameStateManager);
            mSelectionManager = new SelectionManager(mBoard.LeftLane, mBoard.RightLane);
            SaveSlot = saveSlot;

            mHud = new InGameOverlay(mBoard.WaveManager, mSelectionManager, gameStateManager);
            mBoard.PlayerB.InitializePlanners(
                mHud.UnitBuyingMenu.BuyingActions,
                upgradeId => mBoard.mUpgradePool[upgradeId]
            );
        }

        internal static void PushGameStack(int saveSlot, GameStateManager gameStateManager, Storage? storage = null)
        {
            var game = new InGameState(storage, saveSlot, gameStateManager);
            gameStateManager.Restart(game);
            gameStateManager.Push(game.mHud);
        }

        public override void Update(InputManager inputManager, GameTime gameTime, SoundManager soundManager
            , GraphicsDeviceManager graphics)
        {
            if (inputManager.KeyPressed(Keys.Escape) || !inputManager.IsActive || mHud.mScoreOverlay.Pause)
            {
                GameStateManager.Push(MenuState.CreatePauseMenu(GameStateManager, this, soundManager, graphics));
                mHud.mScoreOverlay.Pause = false;
                return;
            }
            mBuildingBuyer.Update(inputManager);
            mSelectionManager.Update(inputManager);
            mBoard.Update(gameTime, inputManager);
            var gameState = mBoard.CheckGameState();
            if (gameState == Board.GameState.Playing)
                return;

            GameStateManager.Restart(MenuState.CreateMainMenu(GameStateManager, soundManager, graphics));
            GameStateManager.Push(MenuState.CreateGameOverScreen(GameStateManager, gameState, soundManager, graphics));
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            mBoard.Draw(spriteBatch, gameTime);
            mSelectionManager.Selection?.DrawActions(spriteBatch, gameTime);
        }

        #region Serialization

        internal Storage Data => new Storage
        {
            Board = mBoard
        };

        #endregion

    }
}
