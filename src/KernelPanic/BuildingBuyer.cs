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
        private TileIndex? mPosition;

        internal Building Building { get; set; }

        private Lane Lane => mPlayer.DefendingLane;

        internal BuildingBuyer(Player player)
        {
            mPlayer = player;
        }

        internal void Update(InputManager input)
        {
            if (Building == null)
                return;

            UpdatePosition(input);
            CheckPath();
            if (mPlayer.Bitcoins < Building.Price)
                Building.State = BuildingState.Invalid;

            if (!input.MousePressed(InputManager.MouseButton.Left) && !input.KeyPressed(input.mInputState.mPlaceTower))
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
            Building.Sprite.Position = tilePoint;
        }

        private void CheckPath()
        {
            if (Building == null || mPosition == null)
            {
                return;
            }

            var buildingMatrix = new ObstacleMatrix(Lane.Grid);
            buildingMatrix.Raster(Lane.EntityGraph.Entities<Building>(), b => b.GetType() != typeof(ShockField));
            buildingMatrix.Raster(new[] {Building}, b => b.GetType() != typeof(ShockField));
            var pathFinder = new AStar(Lane.SpawnPoints[0], Lane.TargetPoints, buildingMatrix);
            var baseBuffer = Lane.Grid.LaneSide == Lane.Side.Left ? Lane.Grid.LaneRectangle.Width - mPosition?.Column - 2 > 0
                : mPosition?.Column - 2 > 0;
            Building.State = (pathFinder.CalculatePath() && baseBuffer) ? BuildingState.Valid : BuildingState.Invalid;
        }

        private bool TryPurchase()
        {
            if (Building == null
                || Building.State != BuildingState.Valid
                || !(mPosition is TileIndex tile)
                || !PurchasableAction<Building>.TryPurchase(mPlayer, Building))
            {
                return false;
            }

            var clone = Building.Clone();
            mPlayer.DefendingLane.BuildingSpawner.Register(clone, tile);
            mPlayer.ApplyUpgrades(clone);

            // TODO: Play a different sound when the AI places a tower.
            EventCenter.Default.Send(Event.BuildingPlaced(mPlayer, clone));

            return true;
        }

        internal void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (mPosition.HasValue)
                Building?.Draw(spriteBatch, gameTime);
        }

        internal static bool Buy(Player player, Building building, Point tile)
        {
            var buyer = new BuildingBuyer(player) {Building = building};
            buyer.SetPosition(new TileIndex(tile, 1));
            buyer.CheckPath();
            return buyer.TryPurchase();
        }
    }
}
