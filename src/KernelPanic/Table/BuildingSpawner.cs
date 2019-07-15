using System;
using System.Collections.Generic;
using System.Linq;
using KernelPanic.Entities;
using KernelPanic.Data;
using KernelPanic.Entities.Buildings;
using KernelPanic.Entities.Units;

namespace KernelPanic.Table
{
    internal sealed class BuildingSpawner
    {
        private readonly Action<Building> mSpawnAction;
        private readonly Grid mGrid;
        private readonly HeatMap mHeatMap;
        private readonly List<Building> mInactive = new List<Building>();

        public BuildingSpawner(Grid grid, HeatMap heatMap, Action<Building> spawnAction, IEnumerable<Building> inactive)
        {
            mGrid = grid;
            mSpawnAction = spawnAction;
            mHeatMap = heatMap;

            if (inactive != null)
                mInactive.AddRange(inactive);
        }

        internal void Register(Building building, TileIndex tile)
        {
            building.State = BuildingState.Inactive;
            building.Sprite.Position = mGrid.GetTile(tile).Position;
            if (!(building is ShockField))
                mHeatMap.ObstacleMatrix[tile.ToPoint()] = true;
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
    }
}
