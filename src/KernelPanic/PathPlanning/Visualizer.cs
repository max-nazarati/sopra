using System.Collections.Generic;
using System.Linq;
using KernelPanic.Data;
using KernelPanic.Sprites;
using KernelPanic.Table;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic.PathPlanning
{
    internal abstract class Visualizer
    {
        private readonly ImageSprite mImage;
        protected Grid Grid { get; }
        protected List<ISpriteSettings> Nodes { get; } = new List<ISpriteSettings>();

        protected interface ISpriteSettings
        {
            void Apply(ImageSprite sprite);
        }

        protected Visualizer(Grid grid, ImageSprite image)
        {
            Grid = grid;
            mImage = image;
        }

        internal void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (var node in Nodes)
            {
                node.Apply(mImage);
                //if (Grid.Contains(mImage.Position))
                    mImage.Draw(spriteBatch, gameTime);
            }
        }
    }

    internal sealed class TileVisualizer : Visualizer
    {
        private struct Node : ISpriteSettings
        {
            private Color mColor;
            private float mSize;
            private Vector2 mPosition;

            internal static ISpriteSettings New(Color color, Vector2 position, float size) =>
                new Node { mColor = color, mPosition = position, mSize = size };

            void ISpriteSettings.Apply(ImageSprite sprite)
            {
                sprite.Position = mPosition;
                sprite.TintColor = mColor;
                sprite.ScaleToWidth(mSize);
            }
        }

        private TileVisualizer(Grid grid, ImageSprite tile) : base(grid, tile)
        {
            tile.SetOrigin(RelativePosition.Center);
        }

        internal static TileVisualizer Border(Grid grid, SpriteManager spriteManager) =>
            new TileVisualizer(grid, Grid.CreateTileBorder(spriteManager));

        internal static TileVisualizer FullTile(Grid grid, SpriteManager spriteManager) =>
            new TileVisualizer(grid, Grid.CreateTile(spriteManager));

        internal void Append(ObstacleMatrix obstacleMatrix)
        {
            Append(obstacleMatrix.Obstacles, Color.Red);
        }

        internal void Append(IEnumerable<Point> points, Color color)
        {
            if (points != null)
                Append(points.Select(point => new TileIndex(point, 1)), color);
        }

        private void Append(IEnumerable<TileIndex> points, Color color)
        {
            Nodes.AddRange(points.Select(point =>
            {
                var (position, size) = Grid.GetTile(point);
                return Node.New(color, position, size);
            }));
        }
    }

    internal sealed class ArrowVisualizer : Visualizer
    {
        private struct Node : ISpriteSettings
        {
            private Vector2 mPosition;
            private float mRotation;

            internal static ISpriteSettings New(Vector2 position, Vector2 direction)
            {
                return new Node
                {
                    // The arrow from SpriteManager.CreateVectorArrow() points upwards by default. In a normal
                    // coordinate system where the y axis grows upwards we would have the subtract 0.5π, but because in
                    // XNA the Y axis grows downwards we have to add 0.5π.
                    mPosition = position,
                    mRotation = direction.Angle(0.5)
                };
            }

            void ISpriteSettings.Apply(ImageSprite sprite)
            {
                sprite.Position = mPosition;
                sprite.Rotation = mRotation;
            }
        }

        internal ArrowVisualizer(Grid grid, SpriteManager spriteManager)
            : base(grid, spriteManager.CreateVectorArrow())
        {
        }

        internal void Append(RelativePosition[,] vectors)
        {
            var rect = new Rectangle(-1, -1, 2, 2);
            for (var row = 0; row < vectors.GetLength(0); ++row)
            {
                for (var col = 0; col < vectors.GetLength(1); ++col)
                {
                    var rel = vectors[row, col];
                    if (rel == RelativePosition.Center)
                        continue;
                    var vec = rect.At(rel);
                    var position = Grid.GetTile(new TileIndex(row, col, 1)).Position;
                    Nodes.Add(Node.New(position, vec));
                }
            }
        }
    }
}
