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

        internal Rectangle PixelCutout
        {
            get
            {
                var cutout = TileCutout;
                cutout.X *= KachelSize;
                cutout.Y *= KachelSize;
                cutout.Width *= KachelSize;
                cutout.Height *= KachelSize;
                cutout.Location += Bounds.Location;
                return cutout;
            }
        }

        /// <summary>
        /// The size of a single tile in pixels.
        /// </summary>
        internal const int KachelSize = 100; // TODO

        private static int TileCountPixelSize(int tiles) => tiles * KachelSize;

        private readonly Sprite mSprite;

        public Rectangle Bounds => new Rectangle(
            LaneRectangle.X * KachelSize,
            LaneRectangle.Y * KachelSize,
            LaneRectangle.Width * KachelSize,
            LaneRectangle.Height * KachelSize);

        internal Grid(Rectangle laneBounds, SpriteManager sprites, Lane.Side laneSide)
        {
            LaneRectangle = laneBounds;
            LaneSide = laneSide;

            var mainPart = sprites.CreateLaneMiddle(LaneWidthInTiles, LaneRectangle.Height, KachelSize);
            mainPart.Y = LaneWidthInTiles * KachelSize;

            var topPart = sprites.CreateLaneTop(laneSide, LaneWidthInTiles, LaneRectangle.Width, KachelSize);
            var bottomPart = sprites.CreateLaneBottom(laneSide, LaneWidthInTiles, LaneRectangle.Width, KachelSize);
            bottomPart.Y = (LaneRectangle.Height - LaneWidthInTiles)*KachelSize;

            switch (laneSide)
            {
                case Lane.Side.Left:
                    topPart.X = 0;
                    bottomPart.X = 0;
                    break;

                case Lane.Side.Right:
                    mainPart.X = (LaneRectangle.Width-LaneWidthInTiles)*KachelSize;
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
        /// Tests if the given <paramref name="tile"/> lies in this grid.
        /// </summary>
        /// <param name="tile">The tile to test for.</param>
        /// <returns><c>true</c> if the tile is inside, <c>false</c> otherwise.</returns>
        internal bool Contains(TileIndex tile)
        {
            var baseTile = tile.BaseTile;
            var inMain = 0 <= baseTile.Column && baseTile.Column < LaneRectangle.Width
                      && 0 <= baseTile.Row && baseTile.Row < LaneRectangle.Height;

            if (!inMain)
                return false;

            var cutout = TileCutout;
            var inCutout = cutout.Left <= tile.Column && tile.Column < cutout.Right
                        && cutout.Top <= tile.Row && tile.Row < cutout.Bottom;

            return !inCutout;
        }

        /// <summary>
        /// Calculates the origin of the tile identified by <paramref name="index"/>.
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
