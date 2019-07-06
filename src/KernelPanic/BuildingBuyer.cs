using System.Linq;
using KernelPanic.Input;
using KernelPanic.Entities;
using KernelPanic.PathPlanning;
using KernelPanic.Players;
using KernelPanic.Purchasing;
using KernelPanic.Sprites;
using KernelPanic.Table;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    internal sealed class BuildingBuyer
    {
        private readonly Player mPlayer;
        private readonly SoundManager mSoundManager;
        private bool mBlocked;

        private Building mBuilding;

        internal Building Building
        {
            set
            {
                mBuilding = value;

                if (mBuilding != null)
                {
                    TintEntity(mBuilding, Color.Gray);
                }
            }
        }

        private TileIndex? mPosition;

        private Lane Lane => mPlayer.DefendingLane;

        internal BuildingBuyer(Player player, SoundManager soundManager)
        {
            mPlayer = player;
            mSoundManager = soundManager;
        }

        internal void Update(InputManager input)
        {
            if (mBuilding == null)
                return;

            UpdatePosition(input);
            CheckPath();

            if (!input.MousePressed(InputManager.MouseButton.Left))
                return;

            if (TryPurchase())
            {
                mSoundManager.PlaySound(SoundManager.Sound.TowerPlacement);
            }

            // TODO: Play failure sound if the purchase couldn't be completed.
        }

        private void UpdatePosition(InputManager inputManager)
        {
            var mouse = inputManager.TranslatedMousePosition;
            if (!(Lane.Grid.TileFromWorldPoint(mouse) is TileIndex tile))
            {
                mPosition = null;
                return;
            }

            SetPosition(tile);
        }

        private void SetPosition(TileIndex tile)
        {
            var tilePoint = Lane.Grid.GetTile(tile).Position;
            if (Lane.EntityGraph.EntitiesAt(tilePoint).Any(entity => entity is Building))
            {
                mPosition = null;
                return;
            }

            mPosition = tile;
            mBuilding.Sprite.Position = tilePoint;
        }

        private void CheckPath()
        {
            if (mBuilding == null || mPosition == null)
            {
                mBlocked = false;
                return;
            }

            var startTile =
                Lane.Grid.LaneSide == Lane.Side.Left
                        ? new Point(Lane.Grid.LaneRectangle.Width - 1, Grid.LaneWidthInTiles / 2)
                        : new Point(0, Lane.Grid.LaneRectangle.Height - Grid.LaneWidthInTiles / 2);

            var endPosition = Lane.Target.Position;

            var buildingMatrix = new ObstacleMatrix(Lane.Grid);
            buildingMatrix.Raster(Lane.EntityGraph.Entities<Building>());
            buildingMatrix.Raster(new[] {mBuilding});

            var pathFinder = new AStar(startTile, endPosition, buildingMatrix);
            mBlocked = !pathFinder.CalculatePath();
        }

        private bool TryPurchase()
        {
            if (mBlocked || !(mPosition is TileIndex tile) || !PurchasableAction<Building>.TryPurchase(mPlayer, mBuilding))
                return false;

            var clone = mBuilding.Clone();
            TintEntity(clone, Color.White);
            mPlayer.DefendingLane.BuildingSpawner.Register(clone, tile);
            return true;
        }

        internal void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (mPosition.HasValue && !mBlocked)
                mBuilding?.Draw(spriteBatch, gameTime);
        }

        private static void TintEntity(Entity building, Color color)
        {
            ((ImageSprite) building.Sprite).TintColor = color;
        }

        internal static bool Buy(Player player, Building building, Point tile, SoundManager soundManager)
        {
            var buyer = new BuildingBuyer(player, soundManager) {Building = building};
            buyer.SetPosition(new TileIndex(tile, 1));
            buyer.CheckPath();
            if (!buyer.TryPurchase())
                return false;

            // TODO: Play a different sound when the AI places a tower.
            soundManager.PlaySound(SoundManager.Sound.TowerPlacement);
            return true;
        }
    }
}
