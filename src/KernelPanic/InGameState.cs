using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace KernelPanic
{
    [DataContract]
    internal sealed class InGameState : AGameState
    {
        private readonly GameStateManager mGameStateManager;
        
        [DataMember]
        public new Camera2D Camera { get; set; }

        private readonly InGameOverlay mHud;
        private readonly Board mBoard;
        private readonly Player mPlayerA;
        private readonly Player mPlayerB;

        private PurchaseButton<Unit, PurchasableAction<Unit>> mPurchaseDemoButton1;
        private PurchaseButton<Tower, SinglePurchasableAction<Tower>> mPurchaseDemoButton2;
        private Button mPurchaseDemoReset;

        // public int SaveSlot { get; set; }
        // public SaveGame CurrentSaveGame { get; private set; } no such class yet
        // private HashSet<Wave> mActiveWaves;
        // private SelectionManager mSelectionManager;

        public InGameState(GameStateManager gameStateManager) : base(gameStateManager)
        {
            mGameStateManager = gameStateManager;
            
            Camera = new Camera2D(gameStateManager.Sprite.ScreenSize);
            mBoard = new Board(gameStateManager.Sprite);
            mPlayerA = new Player(mBoard.RightLane, mBoard.LeftLane);
            mPlayerB = new Player(mBoard.LeftLane, mBoard.RightLane);
            mHud = new InGameOverlay(mPlayerA, mPlayerB, gameStateManager.Sprite);

            var entityGraph = mBoard.LeftLane.EntityGraph;
            entityGraph.Add(Troupe.CreateTrojan(new Point(450), gameStateManager.Sprite));
            entityGraph.Add(Troupe.CreateFirefox(new Point(350), gameStateManager.Sprite));
            entityGraph.Add(Troupe.CreateFirefoxJump(new Point(250), gameStateManager.Sprite));
            InitializePurchaseButtonDemo(entityGraph, gameStateManager.Sprite);
        }

        private void InitializePurchaseButtonDemo(EntityGraph entityGraph, SpriteManager sprites)
        {
            var nextPosition = new Vector2(50, 150);

            mPurchaseDemoButton1 = new PurchaseButton<Unit, PurchasableAction<Unit>>(mPlayerA,
                new PurchasableAction<Unit>(Troupe.CreateFirefox(Point.Zero, sprites)),
                sprites)
            {
                Button = {Title = "Buy Unit"}
            };

            mPurchaseDemoButton2 = new PurchaseButton<Tower, SinglePurchasableAction<Tower>>(mPlayerA,
                new SinglePurchasableAction<Tower>(Tower.Create(Vector2.Zero, Grid.KachelSize, sprites)),
                sprites)
            {
                Button = {Title = "Buy Tower"}
            };

            mPurchaseDemoReset = new Button(sprites);
            mPurchaseDemoReset.Clicked += button =>
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
                mPurchaseDemoButton1.Action.ResetResource(Troupe.CreateFirefox(Point.Zero, sprites));
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

        public override void Update(GameTime gameTime)
        {
            if (InputManager.Default.KeyPressed(Keys.Escape))
            {
                mGameStateManager.Push(MenuState.CreatePauseMenu(mGameStateManager, this));
                return;
            }

            var invertedViewMatrix = Matrix.Invert(Camera.GetViewMatrix());

            mBoard.Update(gameTime, invertedViewMatrix);
            Camera.Update();
            mHud.Update(gameTime);

            mPurchaseDemoButton1.Update(gameTime, invertedViewMatrix);
            mPurchaseDemoButton2.Update(gameTime, invertedViewMatrix);
            mPurchaseDemoReset.Update(gameTime, invertedViewMatrix);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate,
                transformMatrix: Camera.GetViewMatrix(),
                samplerState: SamplerState.PointClamp);

            mBoard.Draw(spriteBatch, gameTime);
            mPurchaseDemoButton1.Draw(spriteBatch, gameTime);
            mPurchaseDemoButton2.Draw(spriteBatch, gameTime);
            mPurchaseDemoReset.Draw(spriteBatch, gameTime);
            
            spriteBatch.End();

            mHud.Draw(spriteBatch, gameTime);
        }
    }
}
