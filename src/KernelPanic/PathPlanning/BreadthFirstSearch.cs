using System;
using System.Collections.Generic;
using System.Linq;
using KernelPanic.Data;
using Microsoft.Xna.Framework;

namespace KernelPanic.PathPlanning
{
    internal sealed class BreadthFirstSearch
    {
        private const int Blocked = HeatMap.Blocked;

        private readonly bool[,] mExplored;
        private HeatMap mMap;
        private VectorField mVectorField;
        private PriorityQueue mQueue;
        private readonly List<Point> mGoalPoints;

        public BreadthFirstSearch(HeatMap map, IEnumerable<Point> goalPoints)
        {
            mMap = map;
            mExplored = new bool[map.Height, map.Width];
            mVectorField = new VectorField(map.Width, map.Height);
            mQueue = new PriorityQueue();
            mGoalPoints = goalPoints.ToList();
        }

        private IEnumerable<Node> CreateNeighbours(Node node)
        {
            return node.EnumerateNeighbours(1, null).Where(n => mMap.IsWalkable(n.Position));
        }

        private void ExpandNode(Node node)
        {
            if (mExplored[node.Position.Y, node.Position.X])
                return;
            foreach (var neighbour in CreateNeighbours(node))
            {
                if (!(mExplored[node.Position.Y, node.Position.X])) mQueue.Insert(neighbour);
            }

            mMap.mMap[node.Position.Y, node.Position.X] = node.Cost;
            mExplored[node.Position.Y, node.Position.X] = true;
        }

        public void UpdateHeatMap()
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
            mVectorField.Update(mMap);
        }

        public VectorField VectorField { get => mVectorField;}
        public HeatMap HeatMap { get => mMap;}

        public void Test1()
        {
            HeatMap emptyHeatMap = new HeatMap(8, 8);
            emptyHeatMap.mMap[2, 1] = Blocked;
            emptyHeatMap.mMap[3, 1] = Blocked;
            emptyHeatMap.mMap[4, 1] = Blocked;
            emptyHeatMap.mMap[4, 2] = Blocked;
            emptyHeatMap.mMap[2, 3] = Blocked;
            emptyHeatMap.mMap[3, 3] = Blocked;
            emptyHeatMap.mMap[4, 3] = Blocked;

            mMap = emptyHeatMap;
            mVectorField = new VectorField(emptyHeatMap.Width, emptyHeatMap.Height);
            mQueue = new PriorityQueue();
            mGoalPoints.Add(new Point(2, 7));
            mGoalPoints.Add(new Point(2, 6));
            mGoalPoints.Add(new Point(3, 7));
            mGoalPoints.Add(new Point(3, 6));
            UpdateVectorField();
            string resultHeat = "";
            for (int i = 0; i < mMap.Height; i++)
            {
                for (int j = 0; j < mMap.Width; j++)
                {
                    resultHeat += mMap.mMap[i, j].ToString();
                    if (0 <= mMap.mMap[i, j] && mMap.mMap[i, j] < 10) resultHeat += "   ";
                    else resultHeat += "  ";
                }

                resultHeat += "\n";
            }

            Console.WriteLine(resultHeat);
            Console.WriteLine("\n ================================= \n");

            string resultVectors = "";
            for (int i = 0; i < mMap.Height; i++)
            {
                for (int j = 0; j < mMap.Width; j++)
                {
                    resultVectors += mVectorField.Vector(new Point(j, i)).ToString();
                    resultVectors += "  ";
                }

                resultVectors += "\n";
            }
            Console.WriteLine(resultVectors);
            Console.WriteLine("\n === \n");
        }
    }
}
