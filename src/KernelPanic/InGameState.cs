using KernelPanic.Camera;
using KernelPanic.Data;
using KernelPanic.Entities;
using KernelPanic.Input;
using KernelPanic.Interface;
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

        private PurchaseButton<TextButton, Unit, PurchasableAction<Unit>> mPurchaseDemoButton1;
        private PurchaseButton<TextButton, Tower, SinglePurchasableAction<Tower>> mPurchaseDemoButton2;
        private TextButton mPurchaseDemoReset;

        internal int SaveSlot { get; }

        // public SaveGame CurrentSaveGame { get; private set; } no such class yet
        // private HashSet<Wave> mActiveWaves;
        // private SelectionManager mSelectionManager;

        private InGameState(Storage? storage, int saveSlot, GameStateManager gameStateManager)
            : base(new Camera2D(Board.Bounds, gameStateManager.Sprite.ScreenSize), gameStateManager)
        {
            mBoard = storage?.Board ?? new Board(gameStateManager.Sprite, gameStateManager.Sound);
            mSelectionManager = new SelectionManager(mBoard.LeftLane, mBoard.RightLane);
            SaveSlot = saveSlot;

            var entityGraph = mBoard.LeftLane.EntityGraph;
            InitializePurchaseButtonDemo(entityGraph, gameStateManager.Sprite, gameStateManager.Sound);
        }

        internal static void PushGameStack(int saveSlot, GameStateManager gameStateManager, Storage? storage = null)
        {
            var game = new InGameState(storage, saveSlot, gameStateManager);
            var hud = new InGameOverlay(game.mBoard.PlayerA, game.mBoard.PlayerB, game.mSelectionManager, gameStateManager);
            gameStateManager.Restart(game);
            gameStateManager.Push(hud);
        }

        private void InitializePurchaseButtonDemo(EntityGraph entityGraph, SpriteManager sprites, SoundManager sounds)
        {
            var player = mBoard.PlayerB;
            var nextPosition = new Vector2(50, 150);

            mPurchaseDemoButton1 = new PurchaseButton<TextButton, Unit, PurchasableAction<Unit>>(player,
                new PurchasableAction<Unit>(new Firefox(sprites)),
                new TextButton(sprites))
            {
                Button = { Title = "Firefox" }
            };

            mPurchaseDemoButton2 = new PurchaseButton<TextButton, Tower, SinglePurchasableAction<Tower>>(player,
                new SinglePurchasableAction<Tower>(Tower.CreateStrategic(Vector2.Zero, Grid.KachelSize, sprites, sounds)),
                new TextButton(sprites))
            {
                Button = { Title = "Turm" }
            };

            mPurchaseDemoReset = new TextButton(sprites);
            mPurchaseDemoReset.Clicked += (button, input) =>
            {
                player.Bitcoins = 9999;
                UpdateResetTitle();
            };

            void UpdateResetTitle()
            {
                mPurchaseDemoReset.Title = player.Bitcoins.ToString();
            }

            void OnPurchase(Player buyer, Entity resource)
            {
                UpdateResetTitle();
                resource.Sprite.Position = nextPosition;
                entityGraph.Add(resource);
                nextPosition.Y += 100;
                mPurchaseDemoButton1.Action.ResetResource(new Firefox(sprites));
            }

            mPurchaseDemoButton1.Action.Purchased += OnPurchase;
            mPurchaseDemoButton2.Action.Purchased += OnPurchase;

            UpdateResetTitle();

            var sprite1 = mPurchaseDemoButton1.Button.Sprite;
            sprite1.SetOrigin(RelativePosition.BottomLeft);
            sprite1.Position = Vector2.Zero;

            var sprite2 = mPurchaseDemoButton2.Button.Sprite;
            sprite2.SetOrigin(RelativePosition.BottomLeft);
            sprite2.Position = sprite1.Position + new Vector2(sprite1.Width, 0);

            var sprite3 = mPurchaseDemoReset.Sprite;
            sprite3.SetOrigin(RelativePosition.BottomLeft);
            sprite3.Position = sprite2.Position + new Vector2(sprite2.Width, 0);
        }

        public override void Update(InputManager inputManager, GameTime gameTime, SoundManager soundManager
            , GraphicsDeviceManager mGraphics)
        {
            if (inputManager.KeyPressed(Keys.Escape) || !inputManager.IsActive)
            {
                GameStateManager.Push(MenuState.CreatePauseMenu(GameStateManager, this, soundManager, mGraphics));
                return;
            }

            mSelectionManager.Update(inputManager);
            mBoard.Update(gameTime, inputManager);

            mPurchaseDemoButton1.Update(inputManager, gameTime);
            mPurchaseDemoButton2.Update(inputManager, gameTime);
            mPurchaseDemoReset.Update(inputManager, gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            mBoard.Draw(spriteBatch, gameTime);
            mSelectionManager.Selection?.DrawActions(spriteBatch, gameTime);
            mPurchaseDemoButton1.Draw(spriteBatch, gameTime);
            mPurchaseDemoButton2.Draw(spriteBatch, gameTime);
            mPurchaseDemoReset.Draw(spriteBatch, gameTime);
        }

        #region Serialization

        internal Storage Data => new Storage
        {
            Board = mBoard
        };

        #endregion

    }
}
