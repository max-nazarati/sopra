using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace KernelPanic
{
    class BreadthFirstSearch
    {
        public const int Blocked = -1;

        private bool[,] mExplored;
        private Heatmap mMap;
        private Vectorfield mVectorfield;
        private PriorityQueue mQueue;
        private Point mGoalPoint;

        public BreadthFirstSearch(Heatmap map, Point goalPoint)
        {
            mMap = map;
            mExplored = new bool[map.Height, map.Width];
            mVectorfield = new Vectorfield(map.Width, map.Height);
            mQueue = new PriorityQueue();
            mGoalPoint = goalPoint;
        }

        private List<Node> CreateNeighbours(Node node)
        {
            var neighbours = new List<Node>();
            var x = node.Position.X;
            var y = node.Position.Y;
            var up = new Point(x, y - 1);
            var left = new Point(x - 1, y);
            var down = new Point(x, y + 1);
            var right = new Point(x + 1, y);
            var cost = node.Cost + 1; // if we put an higher value as summand we get a 'dumb' search

            if (mMap.IsWalkable(up))
            {
                Node nodeUp = new Node(up, node, cost, 0, false, false);
                neighbours.Add(nodeUp);
            }

            if (mMap.IsWalkable(down))
            {
                Node nodeUp = new Node(down, node, cost, 0, false, false);
                neighbours.Add(nodeUp);
            }

            if (mMap.IsWalkable(left))
            {
                Node nodeUp = new Node(left, node, cost, 0, false, false);
                neighbours.Add(nodeUp);
            }

            if (mMap.IsWalkable(right))
            {
                Node nodeUp = new Node(right, node, cost, 0, false, false);
                neighbours.Add(nodeUp);
            }
            return neighbours;
        }
        private void ExpandNode(Node node)
        {
            if (mExplored[node.Position.Y, node.Position.X]) return;
            List<Node> neighbours = CreateNeighbours(node);
            foreach (var neighbour in neighbours)
            {
                if (!(mExplored[node.Position.Y, node.Position.X])) mQueue.Insert(neighbour);
            }

            mMap.mMap[node.Position.Y, node.Position.X] = node.Cost;
            mExplored[node.Position.Y, node.Position.X] = true;
        }

        public void UpdateHeatMap()
        {
            var startNode = new Node(mGoalPoint, null, 0, 0, true);
            // double heuristicValue = startNode.Key;
            mQueue.Insert(startNode);

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
            mVectorfield.UpdateVectorfield(mMap);
        }

        public void test1()
        {
            Heatmap emptyHeatmap = new Heatmap(8, 8);
            emptyHeatmap.mMap[2, 1] = Blocked;
            emptyHeatmap.mMap[3, 1] = Blocked;
            emptyHeatmap.mMap[4, 1] = Blocked;
            emptyHeatmap.mMap[4, 2] = Blocked;
            emptyHeatmap.mMap[2, 3] = Blocked;
            emptyHeatmap.mMap[3, 3] = Blocked;
            emptyHeatmap.mMap[4, 3] = Blocked;

            mMap = emptyHeatmap;
            mVectorfield = new Vectorfield(emptyHeatmap.Width, emptyHeatmap.Height);
            mQueue = new PriorityQueue();
            mGoalPoint = new Point(2, 7);
            UpdateVectorField();
            string resultHeat = "";
            for (int i = 0; i < mMap.Height; i++)
            {
                for (int j = 0; j < mMap.Width; j++)
                {
                    resultHeat += mMap.mMap[i, j].ToString();
                    resultHeat += "  ";
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
                    resultVectors += mVectorfield.Vector(new Point(j, i)).ToString();
                    resultVectors += "  ";
                }

                resultVectors += "\n";
            }
            Console.WriteLine(resultVectors);
            Console.WriteLine("\n === \n");
        }
    }
}
