using System;
using System.Runtime.Serialization;
using System.Threading;
using Autofac.Util;
using KernelPanic.ArtificialIntelligence;
using KernelPanic.Data;
using KernelPanic.Entities;
using KernelPanic.Input;
using KernelPanic.Players;
using KernelPanic.Selection;
using KernelPanic.Sprites;
using KernelPanic.Upgrades;
using KernelPanic.Waves;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace KernelPanic.Table
{
    internal sealed class Board : Disposable
    {
        internal Lane LeftLane => PlayerA.DefendingLane;
        internal Lane RightLane => PlayerA.AttackingLane;

        [JsonProperty]
        internal Player PlayerA { get; /* required for deserialization */ private set; }
        [JsonProperty]
        internal ArtificialPlayer PlayerB { get; /* required for deserialization */ private set; }
        private PlayerIndexed<Player> Players => new PlayerIndexed<Player>(PlayerA, PlayerB);

        [JsonProperty]
        internal UpgradePool mUpgradePool;
        
        [JsonProperty]
        internal WaveManager WaveManager { get; /* required for deserialization */ private set; }

        [JsonProperty]
        private readonly BitcoinManager mBitcoinManager;

        private readonly Sprite mBase;

        private readonly Sprite mBackground;


        private readonly Thread mLeftLaneThread = new Thread(UpdateQueue);
        private readonly Thread mRightLaneThread = new Thread(UpdateQueue);

        private readonly SingleQueue<Lane.UpdateData> mLeftLaneUpdateData = new SingleQueue<Lane.UpdateData>("Board.LeftLane");
        private readonly SingleQueue<Lane.UpdateData> mRightLaneUpdateData = new SingleQueue<Lane.UpdateData>("Board.RightLane");
        private readonly Semaphore mUpdateDoneSemaphore = new Semaphore(0, 2, "Board.UpdateDone");

        internal enum GameState
        {
            Playing,
            AWon,
            BWon
        }

        internal static Rectangle Bounds
        {
            get
            {
                var bounds = Rectangle.Union(Lane.LeftBounds, Lane.RightBounds);
                bounds.Inflate(600, 600);
                return bounds;
            }
        }

        #region Constructing a Board

        internal Board(SpriteManager spriteManager, bool deserializing = false)
        {
            mBase = CreateBase(spriteManager);
            var backgroundBounds = Bounds;
            // add border around lanes to background bounds
            backgroundBounds.X -= 100;
            backgroundBounds.Y -= 100;
            backgroundBounds.Width += 200;
            backgroundBounds.Height += 200;
            mBackground = spriteManager.CreateBoardBackground(backgroundBounds, 100);

            if (deserializing)
            {
                // If this is object is deserialized the other properties are set automatically later.
                return;
            }

            var leftLane = new Lane(Lane.Side.Left, spriteManager);
            var rightLane = new Lane(Lane.Side.Right, spriteManager);
            
            PlayerA = new Player(leftLane, rightLane, 200);
            PlayerB = new ArtificialPlayer(rightLane, leftLane, 200);

            mUpgradePool = new UpgradePool(PlayerA, spriteManager);
            LayOutUpgradePool();

            WaveManager = new WaveManager(Players);
            mBitcoinManager = new BitcoinManager(Players);

            StartThreads();
        }

        [OnDeserialized]
        private void AfterDeserialization(StreamingContext context)
        {
            LayOutUpgradePool();
            StartThreads();
        }
        
        private void LayOutUpgradePool()
        {
            var centerLeft = Lane.LeftBounds.At(RelativePosition.CenterRight);
            var centerRight = Lane.RightBounds.At(RelativePosition.CenterLeft);
            var middle = centerLeft + (centerRight - centerLeft) / 2;
            mUpgradePool.Position = middle - mUpgradePool.Size / 2;
        }

        private void StartThreads()
        {
            mLeftLaneThread.Priority = ThreadPriority.AboveNormal;
            mLeftLaneThread.Name = "Board.LeftLane.Update";
            mRightLaneThread.Priority = ThreadPriority.AboveNormal;
            mRightLaneThread.Name = "Board.RightLane.Update";
            mLeftLaneThread.Start(Tuple.Create(PlayerA.DefendingLane, mLeftLaneUpdateData, mUpdateDoneSemaphore));
            mRightLaneThread.Start(Tuple.Create(PlayerA.AttackingLane, mRightLaneUpdateData, mUpdateDoneSemaphore));
        }

        private static Sprite CreateBase(SpriteManager spriteManager)
        {
            var position = Lane.LeftBounds.At(RelativePosition.TopRight);
            var size = new Vector2(Lane.RightBounds.Left - Lane.LeftBounds.Right, Lane.LeftBounds.Height);
            return spriteManager.CreateBases(position, size);
        }

        #endregion

        private static void UpdateQueue(object arg)
        {
            var (lane, dataAtomic, doneSemaphore) = (Tuple<Lane, SingleQueue<Lane.UpdateData>, Semaphore>)arg;

            while (dataAtomic.Take() is Lane.UpdateData updateData)
            {
                updateData.Update(lane);
                doneSemaphore.Release();
            }
        }

        internal void Update(SelectionManager selectionManager, GameTime gameTime, InputManager inputManager)
        {
            var leftOwner = new Owner(PlayerB, PlayerA);
            var rightOwner = new Owner(PlayerA, PlayerB);

            // Start lane updates.
            mLeftLaneUpdateData.Put(new Lane.UpdateData(gameTime, inputManager, leftOwner, WaveManager.LastIndex));
            mRightLaneUpdateData.Put(new Lane.UpdateData(gameTime, inputManager, rightOwner, WaveManager.LastIndex));

            // Wait until both lanes are updated.
            mUpdateDoneSemaphore.WaitOne();
            mUpdateDoneSemaphore.WaitOne();
            
            // Do the action update here, because it might require the SpriteManager.
            if (selectionManager.Selection is Entity selection)
            {
                var owner = selectionManager.SelectionSide == Lane.Side.Left ? leftOwner : rightOwner;
                selection.UpdateOverlay(owner[selection], inputManager, gameTime);
            }

            PlayerB.Update(gameTime);
            mUpgradePool.Update(inputManager, gameTime);
            WaveManager.Update(gameTime);
            mBitcoinManager.Update(gameTime);
        }

        internal GameState CheckGameState()
        {
            if (PlayerB.Base.Power <= 0)
            {
                return GameState.AWon;
            }
            if (PlayerA.Base.Power <= 0)
            {
                return GameState.BWon;
            }
            return GameState.Playing;
        }

        internal void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            mBackground.Draw(spriteBatch, gameTime);
            LeftLane.Draw(spriteBatch, gameTime);
            mBase.Draw(spriteBatch, gameTime);
            RightLane.Draw(spriteBatch, gameTime);
            mUpgradePool.Draw(spriteBatch, gameTime);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            mLeftLaneUpdateData.Put(null);
            mRightLaneUpdateData.Put(null);
            mLeftLaneThread.Join();
            mRightLaneThread.Join();
            mBitcoinManager.Dispose();
            PlayerB.Dispose();
        }
    }
}