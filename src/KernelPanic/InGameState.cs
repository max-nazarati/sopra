﻿using System;
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
<<<<<<< HEAD
=======
        private InGameOverlay mHud;
        private Sprites.AnimatedSprite mTestSprite;
>>>>>>> 94f495667e95c052ec9402917fb6795668612fac

        public InGameState(GameStateManager gameStateManager) : base(gameStateManager)
        {
            Camera = new Camera2D(gameStateManager.Sprite.GraphicsDevice.Viewport);
            mBoard = new Board(gameStateManager.Sprite);
            mGameStateManager = gameStateManager;
            mPlayerA = new Player();
            mPlayerB = new Player();
            mHud = new InGameOverlay(mPlayerA, mPlayerB);

            // testing movable objects and collision
            // TODO: move to Lane class
            mEntityGraph = new EntityGraph();
            mCollisionManager = new CollisionManager();
            Texture2D texture = new Texture2D(SpriteManager.Default.GraphicsDevice, 1, 1);
            texture.SetData(new[] { Color.Green });
            mUnit1 = new Unit(0, 0, 100, 100, texture);
            Texture2D texture2 = new Texture2D(SpriteManager.Default.GraphicsDevice, 1, 1);
            texture2.SetData(new[] { Color.Red });
            mUnit2 = new Unit(200, 200, 100, 100, texture2);
            mEntityGraph.Add(mUnit1);
            mEntityGraph.Add(mUnit2);
            mCollisionManager.CreatedObject(mUnit1);
            mCollisionManager.CreatedObject(mUnit2);

            mTestSprite = new Sprites.AnimatedSprite(gameStateManager.Sprite.ContentManager.Load<Texture2D>("trojan"), 400, 400, 100, 100);

            // testing cooldown component
            // TODO: see where it fits into the Architecture and move it there
            mCoolDown = new CooldownComponent(new TimeSpan(0, 0, 5));
            mCoolDown.CooledDown += mUnit1.CooledDownDelegate;
        }

        public override void Update(GameTime gameTime, bool isOverlay)
        {
            if (InputManager.Default.KeyPressed(Keys.Escape))
            {
                mGameStateManager.Push(MenuState.CreateMainMenu(mGameStateManager.Game.Exit,
                    SpriteManager.Default.GraphicsDevice.Viewport.Bounds.Size, mGameStateManager));
     
            }
            mEntityGraph.Update(Camera.GetViewMatrix());
            mCollisionManager.Update();
            mTestSprite.Update(gameTime);
            Camera.Update();
            mHud.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime, bool isOverlay)
        {
            var viewMatrix = Camera.GetViewMatrix();
            //spriteBatch.Begin(transformMatrix: viewMatrix);
            spriteBatch.Begin(SpriteSortMode.Immediate,
                    BlendState.AlphaBlend,
                    SamplerState.PointClamp,
                    null,
                    null,
                    null,
                    viewMatrix);
            mBoard.DrawLane(spriteBatch, viewMatrix, gameTime);
            mEntityGraph.Draw(spriteBatch);
            mTestSprite.Draw(spriteBatch, gameTime);
            spriteBatch.End();
            mHud.Draw(spriteBatch, gameTime);
        }
    }
}
