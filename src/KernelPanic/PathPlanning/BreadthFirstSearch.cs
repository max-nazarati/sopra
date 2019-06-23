using System.Collections.Generic;
using System.Linq;
using KernelPanic.Data;
using Microsoft.Xna.Framework;

namespace KernelPanic.PathPlanning
{
    internal sealed class BreadthFirstSearch : PathPlanner
    {
        public VectorField VectorField { get; }
        public HeatMap HeatMap { get; }

        private readonly Point[] mGoalPoints;

        public BreadthFirstSearch(HeatMap map, IEnumerable<Point> goalPoints)
        {
            HeatMap = map;
            VectorField = new VectorField(map.Width, map.Height);
            mGoalPoints = goalPoints.ToArray();
        }

        public void UpdateVectorField()
        {
            foreach (var node in Run(mGoalPoints))
            {
                HeatMap.SetCost(node.Position, node.Cost);
            }

            VectorField.Update(HeatMap);
        }

        protected override bool IsWalkable(Point point) => HeatMap.IsWalkable(point);
        protected override double EstimateCost(Point point) => 0;
        protected override double CostIncrease => 1;
    }
}
