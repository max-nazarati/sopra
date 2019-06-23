using System.Collections.Generic;
using System.Linq;
using KernelPanic.Data;
using Microsoft.Xna.Framework;

namespace KernelPanic.PathPlanning
{
    internal sealed class BreadthFirstSearch
    {
        private readonly bool[,] mExplored;
        private readonly PriorityQueue mQueue;
        private readonly List<Point> mGoalPoints;

        public VectorField VectorField { get; }
        public HeatMap HeatMap { get; }

        public BreadthFirstSearch(HeatMap map, IEnumerable<Point> goalPoints)
        {
            HeatMap = map;
            mExplored = new bool[map.Height, map.Width];
            VectorField = new VectorField(map.Width, map.Height);
            mQueue = new PriorityQueue();
            mGoalPoints = goalPoints.ToList();
        }

        private IEnumerable<Node> CreateNeighbours(Node node) =>
            node.EnumerateNeighbours(1, null).Where(n => HeatMap.IsWalkable(n.Position));

        private void ExpandNode(Node node)
        {
            if (mExplored[node.Position.Y, node.Position.X])
                return;
            foreach (var neighbour in CreateNeighbours(node))
            {
                if (!(mExplored[node.Position.Y, node.Position.X])) mQueue.Insert(neighbour);
            }

            HeatMap.SetCost(node.Position, node.Cost);
            mExplored[node.Position.Y, node.Position.X] = true;
        }

        private void UpdateHeatMap()
        {
            foreach (var goalNode in mGoalPoints)
            {
                mQueue.Insert(new Node(goalNode, null, 0, 0));
            }

            while (!mQueue.IsEmpty())
            {
                var heapNode = mQueue.RemoveMin();
                ExpandNode(heapNode);
                // Console.Write("I am currently expanding: ");
                // Console.WriteLine(heapNode.Position);
            }
        }

        public void UpdateVectorField()
        {
            UpdateHeatMap();
            VectorField.Update(HeatMap);
        }
    }
}
