using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic; // for the HashSet

namespace KernelPanic
{
    [DataContract]
    internal sealed class InGameState : AGameState
    {
        [DataMember]
        public new Camera2D Camera { get; set; }
        //private Camera2D mCamera;
        public int SaveSlot { get; set; }
        //public SaveGame CurrentSaveGame { get; private set; } no such class yet
        //private HashSet<Wave> mActiveWaves;
        //private SelectionManager mSelectionManager;
        private Board mBoard;
        private Player mPlayerA;
        private Player mPlayerB;
        private GameStateManager mGameStateManager;
        private EntityGraph mEntityGraph;
        private CollisionManager mCollisionManager;
        private Unit mUnit1;
        private Unit mUnit2;
        private CooldownComponent mCoolDown;

        private InGameOverlay mHud;
        private Sprites.AnimatedSprite mTestSprite;
        public InGameState(GameStateManager gameStateManager) : base(gameStateManager)
        {
            Camera = new Camera2D(gameStateManager.Sprite.ScreenSize);
            mBoard = new Board(gameStateManager.Sprite);
            mGameStateManager = gameStateManager;
            mPlayerA = new Player();
            mPlayerB = new Player();
            mHud = new InGameOverlay(mPlayerA, mPlayerB, gameStateManager.Sprite);

            // testing movable objects and collision
            // TODO: move to Lane class
            mEntityGraph = new EntityGraph();
            mCollisionManager = new CollisionManager();
            mUnit1 = Troupe.Create(new Point(0), Color.Green, gameStateManager.Sprite);
            mUnit2 = Troupe.Create(new Point(200), Color.Red, gameStateManager.Sprite);
            mEntityGraph.Add(mUnit1);
            mEntityGraph.Add(mUnit2);
            mCollisionManager.CreatedObject(mUnit1);
            mCollisionManager.CreatedObject(mUnit2);

            mTestSprite = gameStateManager.Sprite.CreateTrojan();
            mTestSprite.ScaleToWidth(Grid.KachelSize);
        }

        public override void Update(GameTime gameTime, bool isOverlay)
        {
            if (InputManager.Default.KeyPressed(Keys.Escape))
            {
                mGameStateManager.Push(MenuState.CreateMainMenu(mGameStateManager.Game.Exit, mGameStateManager));
            }

            var viewMatrix = Camera.GetViewMatrix();
            var invertedViewMatrix = Matrix.Invert(viewMatrix);

            mEntityGraph.Update(gameTime, viewMatrix);
            mCollisionManager.Update();
            mBoard.Update(gameTime, invertedViewMatrix);
            Camera.Update();
            mHud.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime, bool isOverlay)
        {
            var viewMatrix = Camera.GetViewMatrix();
            spriteBatch.Begin(SpriteSortMode.Immediate,
                transformMatrix: viewMatrix,
                samplerState: SamplerState.PointClamp);
            mBoard.Draw(spriteBatch, gameTime);
            mEntityGraph.Draw(spriteBatch, gameTime);
            mTestSprite.Draw(spriteBatch, gameTime);
            spriteBatch.End();
            mHud.Draw(spriteBatch, gameTime);
        }
    }
}
