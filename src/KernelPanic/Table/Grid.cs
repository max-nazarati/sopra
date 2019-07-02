using System.Runtime.Serialization;
using System;
using System.ComponentModel;
using KernelPanic.Data;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic.Table
{
    [DataContract]
    internal sealed class Grid : IBounded
    {
        internal const int LaneWidthInTiles = 10;
        internal Lane.Side LaneSide { get; }
        internal Rectangle LaneRectangle { get; }

        private Rectangle TileCutout => new Rectangle(
            LaneSide == Lane.Side.Left ? LaneWidthInTiles : 0,
            LaneWidthInTiles,
            LaneRectangle.Width - LaneWidthInTiles,
            LaneRectangle.Height - 2 * LaneWidthInTiles);

        private Rectangle PixelCutout
        {
            get
            {
                var cutout = TileCutout;
                cutout.X *= KachelSize;
                cutout.Y *= KachelSize;
                cutout.Width *= KachelSize;
                cutout.Height *= KachelSize;
                return cutout;
            }
        }

        /// <summary>
        /// The size of a single tile in pixels.
        /// </summary>
        internal const int KachelSize = 100; // TODO
        
        private const int TilesPerSprite = 1; // per Dimension
        private const int SingleTileSizePixel = KachelSize / TilesPerSprite;

        private static int TileCountPixelSize(int tiles) => tiles * KachelSize;

        private readonly Sprite mSprite;

        public Rectangle Bounds => mSprite.Bounds;

        internal int TileCount
        {
            get
            {
                var rectangle = LaneRectangle;
                var cutout = TileCutout;
                return rectangle.Width * rectangle.Height - cutout.Width * cutout.Height;
            }
        }

        internal Grid(Rectangle laneBounds, SpriteManager sprites, Lane.Side laneSide)
        {
            LaneRectangle = laneBounds;
            LaneSide = laneSide;

            var tile = CreateTile(sprites);
            var mainPart = new PatternSprite(tile, LaneRectangle.Height, LaneWidthInTiles);

            var topPart = new PatternSprite(tile, LaneWidthInTiles, LaneRectangle.Width - LaneWidthInTiles);
            var bottomPart = new PatternSprite(tile, LaneWidthInTiles, LaneRectangle.Width - LaneWidthInTiles);
            bottomPart.Y = mainPart.Height - bottomPart.Height;

            switch (laneSide)
            {
                case Lane.Side.Left:
                    topPart.X = mainPart.Width;
                    bottomPart.X = mainPart.Width;
                    break;

                case Lane.Side.Right:
                    mainPart.X = topPart.Width;
                    break;

                default:
                    throw new InvalidEnumArgumentException(nameof(laneSide), (int)laneSide, typeof(Lane.Side));
            }

            mSprite = new CompositeSprite
            {
                X = TileCountPixelSize(LaneRectangle.X),
                Y = TileCountPixelSize(LaneRectangle.Y),
                Children = {mainPart, bottomPart, topPart}
            };
        }

        internal static ImageSprite CreateTile(SpriteManager spriteManager)
        {
            var tile = spriteManager.CreateLaneTile();
            tile.ScaleToWidth(KachelSize);
            return tile;
        }

        internal static ImageSprite CreateTileBorder(SpriteManager spriteManager)
        {
            var tile = spriteManager.CreateLaneBorder();
            tile.ScaleToWidth(KachelSize);
            return tile;
        }

        /// <summary>
        /// calculates the position on the screen for a grid coordinate point,
        /// so things can be drawn correctly
        /// </summary>
        /// <param name="upperLeft"></param>
        /// <returns></returns>
        // TODO: Replace calls to this function with Grid.GetTile.
        public static Point ScreenPositionFromCoordinate(Point upperLeft)
        {
            var xPositionGlobal = upperLeft.X; // this is now saved in the grid + mLaneRectangle.X;
            var yPositionGlobal = upperLeft.Y; // this is now saved in the grid + mLaneRectangle.Y;
            var x = (SingleTileSizePixel * TilesPerSprite * xPositionGlobal);
            var y = (SingleTileSizePixel * TilesPerSprite * yPositionGlobal);
            var result = new Point(x, y);
            return result;

        }

        /// <summary>
        /// Tests if the given point lies on this lane and if successful returns information about the hit tile.
        /// </summary>
        /// <param name="point">The point to test for.</param>
        /// <param name="subTileCount">The number of sub-tiles to segment </param>
        /// <param name="origin">If a tile is hit return its position according to this.</param>
        /// <returns>
        /// <c>null</c> if <paramref name="point"/> does not lie on this lane, otherwise the position of the hit
        /// tile and the tiles size.
        /// </returns>
        internal (Vector2 Position, float Size)? GridPointFromWorldPoint(
            Vector2 point,
            int subTileCount = 1,
            RelativePosition origin = RelativePosition.Center)
        {
            if (TileFromWorldPoint(point, subTileCount) is TileIndex tile)
                return GetTile(tile, origin);

            return null;
        }

        internal TileIndex? TileFromWorldPoint(Vector2 point, int subTileCount = 1)
        {
            if (!Contains(point))
                return null;

            var subTileSize = (float) KachelSize / subTileCount;
            return new TileIndex(((point - mSprite.Position) / subTileSize).ToPoint(), subTileCount);
        }

        /// <summary>
        /// Tests if the given <paramref name="point"/> lies in this grid.
        /// </summary>
        /// <param name="point">The point to test for.</param>
        /// <returns><c>true</c> if the point is inside, <c>false</c> otherwise.</returns>
        internal bool Contains(Vector2 point)
        {
            return Bounds.Contains(point) && !PixelCutout.Contains(point);
        }

        /// <summary>
        /// Calculates the origin the tile identified by <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index of the tile.</param>
        /// <param name="origin">Where to put the origin in the rectangle, <see cref="RelativePosition.Center"/> by default.</param>
        /// <returns>The position of the origin, and the length of the square's sides.</returns>
        /// <exception cref="IndexOutOfRangeException">If <paramref name="index"/> is outside the bounds of <see cref="LaneRectangle"/>.</exception>
        internal (Vector2 Position, float Size) GetTile(TileIndex index, RelativePosition origin = RelativePosition.Center)
        {
            if (index.Column / index.SubTileCount >= LaneRectangle.Width || index.Row / index.SubTileCount >= LaneRectangle.Height)
            {
                throw new IndexOutOfRangeException($"TileIndex {index} is out of bounds {LaneRectangle.Size}");
            }

            var subTileSize = (float) KachelSize / index.SubTileCount;
            var position = mSprite.Position + index.ToPoint().ToVector2() * subTileSize + origin.RectangleOrigin(new Vector2(subTileSize));
            return (position, subTileSize);
        }

        /// <summary>
        /// calling the different draw function
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="gameTime"></param>
        internal void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            mSprite.Draw(spriteBatch, gameTime);
        }
    }
}
