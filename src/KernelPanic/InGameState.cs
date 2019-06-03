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

            var eg = mBoard.LeftLane.EntityGraph;
            eg.Add(Troupe.CreateTrojan(new Point(400), gameStateManager.Sprite));
            eg.Add(Troupe.CreateFirefox(new Point(300), gameStateManager.Sprite));
        }

        public override void Update(GameTime gameTime, bool isOverlay)
        {
            if (InputManager.Default.KeyPressed(Keys.Escape))
            {
                mGameStateManager.Push(MenuState.CreateMainMenu(mGameStateManager));
            }

            var viewMatrix = Camera.GetViewMatrix();
            var invertedViewMatrix = Matrix.Invert(viewMatrix);

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
            spriteBatch.End();
            mHud.Draw(spriteBatch, gameTime);
        }
    }
}
