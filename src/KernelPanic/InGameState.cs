using System.Runtime.Serialization;
using KernelPanic.Camera;
using KernelPanic.Data;
using KernelPanic.Entities;
using KernelPanic.Input;
using KernelPanic.Interface;
using KernelPanic.Selection;
using KernelPanic.Table;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace KernelPanic
{
    [DataContract]
    internal sealed class InGameState : AGameState
    {
        private readonly GameStateManager mGameStateManager;

        [DataMember(Name = "Board")]
        private readonly Board mBoard;
        [DataMember(Name = "PlayerA")]
        private Player mPlayerA;
        [DataMember(Name = "PlayerB")]
        private Player mPlayerB;

        [DataMember]
        private SelectionManager mSelectionManager;

        private PurchaseButton<Unit, PurchasableAction<Unit>> mPurchaseDemoButton1;
        private PurchaseButton<Tower, SinglePurchasableAction<Tower>> mPurchaseDemoButton2;
        private Button mPurchaseDemoReset;

        // public int SaveSlot { get; set; }
        // public SaveGame CurrentSaveGame { get; private set; } no such class yet
        // private HashSet<Wave> mActiveWaves;
        // private SelectionManager mSelectionManager;

        
        private InGameState(DataStorage storage, GameStateManager gameStateManager)
            : base(new Camera2D(Board.Bounds, gameStateManager.Sprite.ScreenSize), gameStateManager)
        {
            mGameStateManager = gameStateManager;

            mBoard = storage?.Board ?? new Board(gameStateManager.Sprite, gameStateManager.Sound);
            mPlayerA = storage?.PlayerA ?? new Player(mBoard.RightLane, mBoard.LeftLane);
            mPlayerB = storage?.PlayerB ?? new Player(mBoard.LeftLane, mBoard.RightLane);
            mSelectionManager = new SelectionManager(mPlayerA.AttackingLane, mPlayerA.DefendingLane);

            var entityGraph = mBoard.LeftLane.EntityGraph;
            InitializePurchaseButtonDemo(entityGraph, gameStateManager.Sprite, gameStateManager.Sound);
        }

        internal static void PushGameStack(GameStateManager gameStateManager, DataStorage storage=null)
        {
            var game = new InGameState(storage, gameStateManager);
            var hud = new InGameOverlay(game.mPlayerA, game.mPlayerB, game.mSelectionManager, gameStateManager);
            gameStateManager.Restart(game);
            gameStateManager.Push(hud);
        }

        private void InitializePurchaseButtonDemo(EntityGraph entityGraph, SpriteManager sprites, SoundManager sounds)
        {
            var nextPosition = new Vector2(50, 150);

            mPurchaseDemoButton1 = new PurchaseButton<Unit, PurchasableAction<Unit>>(mPlayerA,
                new PurchasableAction<Unit>(Firefox.CreateFirefox(Point.Zero, sprites)),
                sprites)
            {
                Button = {Title = "Buy Unit"}
            };

            mPurchaseDemoButton2 = new PurchaseButton<Tower, SinglePurchasableAction<Tower>>(mPlayerA,
                new SinglePurchasableAction<Tower>(Tower.CreateStrategic(Vector2.Zero, Grid.KachelSize, sprites, sounds)),
                sprites)
            {
                Button = {Title = "Buy Tower"}
            };

            mPurchaseDemoReset = new Button(sprites);
            mPurchaseDemoReset.Clicked += (button, input) =>
            {
                mPlayerA.Bitcoins = 50;
                UpdateResetTitle();
            };

            void UpdateResetTitle()
            {
                mPurchaseDemoReset.Title = mPlayerA.Bitcoins.ToString();
            }

            void OnPurchase(Player buyer, Entity resource)
            {
                UpdateResetTitle();
                resource.Sprite.Position = nextPosition;
                entityGraph.Add(resource);
                nextPosition.Y += 100;
                mPurchaseDemoButton1.Action.ResetResource(Firefox.CreateFirefox(Point.Zero, sprites));
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

        public override void Update(InputManager inputManager, GameTime gameTime)
        {
            if (inputManager.KeyPressed(Keys.Escape) || !inputManager.IsActive)
            {
                mGameStateManager.Push(MenuState.CreatePauseMenu(mGameStateManager, this));
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
            mPurchaseDemoButton1.Draw(spriteBatch, gameTime);
            mPurchaseDemoButton2.Draw(spriteBatch, gameTime);
            mPurchaseDemoReset.Draw(spriteBatch, gameTime);
        }

        public DataStorage toDataStorage()
        {
            return new DataStorage
            {
                PlayerA = mPlayerA,
                PlayerB = mPlayerB,
                Board = mBoard,

            };
        }
    }
}
