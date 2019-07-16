using System;
using System.Collections.Generic;
using System.Linq;
using KernelPanic.Entities;
using KernelPanic.Entities.Buildings;
using KernelPanic.Entities.Units;
using Microsoft.Xna.Framework;

namespace KernelPanic.Table
{
    internal sealed class BuildingSpawner
    {
        private readonly Action<Building> mSpawnAction;
        private readonly Grid mGrid;
        private readonly List<Building> mInactive = new List<Building>();
        private readonly List<Point> mBuildingPoints = new List<Point>();

        public BuildingSpawner(Grid grid, Action<Building> spawnAction, IEnumerable<Building> inactive)
        {
            mGrid = grid;
            mSpawnAction = spawnAction;

            if (inactive != null)
                mInactive.AddRange(inactive);
        }

        internal void Register(Building building, TileIndex tile)
        {
            building.State = BuildingState.Inactive;
            building.Sprite.Position = mGrid.GetTile(tile).Position;
            if (!(building is ShockField))
                mBuildingPoints.Add(tile.ToPoint());
            mInactive.Add(building);
            mSpawnAction(building);
        }

        internal void Update(PositionProvider positionProvider)
        {
            foreach (var building in mInactive)
            {
                // When the only units overlapping with the new building are Thunderbirds we can activate it.
                if (positionProvider.EntitiesAt<Unit>(building).All(unit => unit is Thunderbird))
                    building.State = BuildingState.Active;
            }

            mInactive.RemoveAll(building => building.State != BuildingState.Inactive);
        }

        /// <summary>
        /// Enumerates through all points where a building was placed since the last call to this function.
        /// </summary>
        /// <returns>All points where a building was placed.</returns>
        internal IEnumerable<Point> NewBuildings()
        {
            try
            {
                foreach (var point in mBuildingPoints)
                    yield return point;
            }
            finally
            {
                mBuildingPoints.Clear();
            }
        }
    }
}
