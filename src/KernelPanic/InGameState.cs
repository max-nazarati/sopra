using KernelPanic.Camera;
using KernelPanic.Events;
using KernelPanic.Hud;
using KernelPanic.Input;
using KernelPanic.Selection;
using KernelPanic.Serialization;
using KernelPanic.Table;
using KernelPanic.Tracking;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using KernelPanic.Entities.Units;

namespace KernelPanic
{
    internal sealed class InGameState : AGameState
    {
        private readonly Board mBoard;
        private readonly SelectionManager mSelectionManager;
        private readonly BuildingBuyer mBuildingBuyer;
        private readonly InGameOverlay mHud;
        private readonly AchievementPool mAchievementPool;
        private bool mIsTechDemo;

        internal int SaveSlot { get; }

        private InGameState(Storage? storage, int saveSlot, GameStateManager gameStateManager)
            : base(new Camera2D(Board.Bounds, gameStateManager.Sprite.ScreenSize), gameStateManager)
        {
            mBoard = storage?.Board ?? new Board(gameStateManager.Sprite);
            mBuildingBuyer = new BuildingBuyer(mBoard.PlayerA);
            mSelectionManager = new SelectionManager(mBoard.LeftLane, mBoard.RightLane, gameStateManager.Sprite);
            mAchievementPool = new AchievementPool(gameStateManager.AchievementPool, storage?.AchievementData);
            SaveSlot = saveSlot;

            var unitMenu = UnitBuyingMenu.Create(mBoard.WaveManager, gameStateManager.Sprite);
            var buildingMenu = BuildingBuyingMenu.Create(mBuildingBuyer, gameStateManager.Sprite, mBoard.PlayerA);
            mHud = new InGameOverlay(mBoard.WaveManager, unitMenu, buildingMenu, gameStateManager, storage?.GameTime ?? TimeSpan.Zero, Camera);

            mBoard.PlayerB.InitializePlanners(
                unitMenu.BuyingActions, // TODO implement this, just added it like this so i can build :)
                upgradeId => mBoard.mUpgradePool[upgradeId],
                gameStateManager.Sprite);
        }

        internal static void PushGameStack(int saveSlot, GameStateManager gameStateManager, Storage? storage = null)
        {
            var game = new InGameState(storage, saveSlot, gameStateManager);
            gameStateManager.Restart(game);
            gameStateManager.Push(game.mHud);
        }

        internal static void PushGameStack(GameStateManager gameStateManager)
        {
            // for pushing main-menu first
            gameStateManager.Restart(MenuState.CreateMainMenu(gameStateManager));
        }

        internal static void PushTechDemo(GameStateManager gameStateManager)
        {
            var game = new InGameState(null, 5, gameStateManager);
            game.InitializeTechDemo();
            gameStateManager.Restart(game);
            gameStateManager.Push(game.mHud);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing)
                return;

            mBoard.Dispose();

            if (mAchievementPool != null)
                EventCenter.Default.Send(Event.CloseAchievementPool(mAchievementPool));
            if (mIsTechDemo)
                EventCenter.Default.Send(Event.TechDemoClosed());
        }

        private void InitializeTechDemo()
        {
            mIsTechDemo = true;
            EventCenter.Default.Send(Event.TechDemoStarted());

            foreach (var kv in mHud.UnitBuyingMenu.BuyingActions)
            {
                mBoard.PlayerA.Base.Power = 9999;
                mBoard.PlayerB.Base.Power = 9999;
                if (typeof(Hero).IsAssignableFrom(kv.Key))
                {
                    // Skip heroes.
                    continue;
                }

                for (var i = 0; i < 200; i++)
                {
                    mBoard.PlayerA.Bitcoins = mBoard.PlayerB.Bitcoins = 9999;
                    kv.Value.TryPurchase(mBoard.PlayerA);
                    kv.Value.TryPurchase(mBoard.PlayerB);
                }
            }
        }

        public override void Update(InputManager inputManager, GameTime gameTime)
        {
            if (inputManager.KeyPressed(Keys.Escape) || !inputManager.IsActive || mHud.ScoreOverlay.Pause)
            {
                GameStateManager.Push(MenuState.CreatePauseMenu(GameStateManager, this));
                mHud.ScoreOverlay.Pause = false;
                return;
            }

            // Update the overall play time.
            GameStateManager.Statistics.Update(gameTime);

            mSelectionManager.Update(inputManager, mBuildingBuyer.Building != null);
            mBuildingBuyer.Update(inputManager);

            mBoard.Update(mSelectionManager, gameTime, inputManager);
            var gameState = mBoard.CheckGameState();
            if (gameState == Board.GameState.Playing)
                return;

            EventCenter.Default.Send(gameState == Board.GameState.AWon
                ? Event.GameWon(mBoard.PlayerA, mBoard.PlayerB)
                : Event.GameLost(mBoard.PlayerB, mBoard.PlayerA));

            GameStateManager.Restart(MenuState.CreateMainMenu(GameStateManager));
            GameStateManager.Push(MenuState.CreateGameOverScreen(GameStateManager, gameState));
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
            GameTime = mHud.ScoreOverlay.Time,
            AchievementData = mAchievementPool?.AchievementData
        };

        #endregion

    }
}
