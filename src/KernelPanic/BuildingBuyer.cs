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
        private bool mBlocked = false;

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

        private Point? mPosition;

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
            CheckPath(mPlayer.DefendingLane, mBuilding);

            if (!input.MousePressed(InputManager.MouseButton.Left))
                return;

            if (mBlocked || !(mPosition is Point point) || !PurchasableAction<Building>.TryPurchase(mPlayer, mBuilding))
            {
                // TODO: Play failure sound.
                return;
            }

            var clone = mBuilding.Clone();
            TintEntity(clone, Color.White);
            mPlayer.DefendingLane.BuildingSpawner.Register(clone, point);
            mSoundManager.PlaySound(SoundManager.Sound.TowerPlacement);
        }

        private void UpdatePosition(InputManager inputManager)
        {
            var lane = mPlayer.DefendingLane;
            var mouse = inputManager.TranslatedMousePosition;
            if (!(lane.Grid.TileFromWorldPoint(mouse) is TileIndex tile))
            {
                mPosition = null;
                return;
            }

            var tilePoint = lane.Grid.GetTile(tile).Position;
            if (lane.EntityGraph.EntitiesAt(tilePoint).Any(entity => entity is Building))
            {
                mPosition = null;
                return;
            }

            mPosition = tile.ToPoint();
            mBuilding.Sprite.Position = tilePoint;
        }

        private void CheckPath(Lane lane, Building building)
        {
            if (mBuilding == null)
            {
                mBlocked = false;
                return;
            }

            var startTile =
                lane.Grid.LaneSide == Lane.Side.Left
                        ? new Point(Grid.LaneWidthInTiles / 2, lane.Grid.LaneRectangle.Width - 1)
                        : new Point(lane.Grid.LaneRectangle.Height - Grid.LaneWidthInTiles / 2, 0);

            var endPosition = lane.Target.Position;

            var buildingMatrix = new ObstacleMatrix(lane.Grid);
            buildingMatrix.Raster(lane.EntityGraph.Entities<Building>());
            buildingMatrix.Raster(new[] {building});

            var pathFinder = new AStar(startTile, endPosition, buildingMatrix);
            mBlocked = !pathFinder.CalculatePath();
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
    }
}
