using System;
using System.Linq;
using KernelPanic.Input;
using KernelPanic.Entities;
using KernelPanic.Entities.Buildings;
using KernelPanic.Events;
using KernelPanic.PathPlanning;
using KernelPanic.Players;
using KernelPanic.Purchasing;
using KernelPanic.Table;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    internal sealed class BuildingBuyer
    {
        private readonly Player mPlayer;
        private Building mBuilding;
        private TileIndex? mPosition;

        private Lane Lane => mPlayer.DefendingLane;

        internal BuildingBuyer(Player player)
        {
            mPlayer = player;
        }

        internal void SetBuilding(Building building)
        {
            mBuilding = building;
        }

        internal void Update(InputManager input)
        {
            if (mBuilding == null)
                return;

            UpdatePosition(input);
            CheckPath();

            if (!input.MousePressed(InputManager.MouseButton.Left))
                return;

            if (!TryPurchase())
            {
                Console.WriteLine("Gebäude kann nicht gekauft werden.");
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
                return;
            }

            var startTile =
                Lane.Grid.LaneSide == Lane.Side.Left
                        ? new Point(Lane.Grid.LaneRectangle.Width - 1, Grid.LaneWidthInTiles / 2)
                        : new Point(0, Lane.Grid.LaneRectangle.Height - Grid.LaneWidthInTiles / 2);

            var buildingMatrix = new ObstacleMatrix(Lane.Grid);
            buildingMatrix.Raster(Lane.EntityGraph.Entities<Building>(), b => b.GetType() != typeof(ShockField));
            buildingMatrix.Raster(new[] {mBuilding}, b => b.GetType() != typeof(ShockField));
            var pathFinder = new AStar(startTile, Lane.Target.HitBox, buildingMatrix);
            mBuilding.State = pathFinder.CalculatePath() ? BuildingState.Valid : BuildingState.Invalid;
        }

        private bool TryPurchase()
        {
            if (mBuilding == null
                || mBuilding.State != BuildingState.Valid
                || !(mPosition is TileIndex tile)
                || !PurchasableAction<Building>.TryPurchase(mPlayer, mBuilding))
            {
                return false;
            }

            var clone = mBuilding.Clone();
            mPlayer.DefendingLane.BuildingSpawner.Register(clone, tile);
            mPlayer.ApplyUpgrades(clone);

            // TODO: Play a different sound when the AI places a tower.
            EventCenter.Default.Send(Event.BuildingPlaced(mPlayer, clone));

            return true;
        }

        internal void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (mPosition.HasValue)
                mBuilding?.Draw(spriteBatch, gameTime);
        }

        internal static bool Buy(Player player, Building building, Point tile)
        {
            var buyer = new BuildingBuyer(player) {mBuilding = building};
            buyer.SetPosition(new TileIndex(tile, 1));
            buyer.CheckPath();
            return buyer.TryPurchase();
        }
    }
}
