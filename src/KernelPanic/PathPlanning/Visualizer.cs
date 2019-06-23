using System.Collections.Generic;
using System.Linq;
using KernelPanic.Data;
using KernelPanic.Sprites;
using KernelPanic.Table;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic.PathPlanning
{
    internal sealed class Visualizer
    {
        private readonly Grid mGrid;
        private readonly ImageSprite mTile;

        private struct Node
        {
            internal Color Color { get; }
            internal float Size { get; }
            internal Vector2 Position { get; }

            internal Node(Color color, Vector2 position, float size)
            {
                Color = color;
                Position = position;
                Size = size;
            }
        }

        private readonly List<Node> mNodes = new List<Node>();

        internal Visualizer(Grid grid, SpriteManager spriteManager, bool drawBorderOnly=true)
        {
            mGrid = grid;
            mTile = drawBorderOnly ? Grid.CreateTileBorder(spriteManager) : Grid.CreateTile(spriteManager);
        }

        internal void Append(ObstacleMatrix obstacleMatrix)
        {
            mNodes.AddRange(obstacleMatrix.Obstacles.Select(obstacle =>
            {
                var (position, size) = mGrid.GetTile(obstacle, RelativePosition.TopLeft);
                return new Node(Color.Red, position, size);
            }));
        }

        internal void Append(IEnumerable<Point> points, Color color, float tileSize = Grid.KachelSize)
        {
            if (points == null)
            {
                return;
                
            }
            mNodes.AddRange(points.Select(point =>
            {
                var position = Grid.ScreenPositionFromCoordinate(point).ToVector2();
                return new Node(color, position, tileSize);
            }));
        }

        internal void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (var node in mNodes)
            {
                if (!mGrid.Contains(node.Position))
                    continue;
                mTile.TintColor = node.Color;
                mTile.ScaleToWidth(node.Size);
                mTile.Position = node.Position;
                mTile.Draw(spriteBatch, gameTime);
            }
        }
    }
}
