using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KernelPanic.Data;
using KernelPanic.Entities;
using KernelPanic.Input;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    [DataContract]
    internal sealed class EntityGraph
    {
        [DataMember]
        private readonly QuadTree<Entity> mQuadTree;

        // We don't serialize this as it can be reconstructed from mQuadTree.
        private readonly ObstacleMatrix mObstacles;

        private readonly ImageSprite mSelectionBorder;

        public EntityGraph(Rectangle bounds, ObstacleMatrix obstacles, SpriteManager spriteManager)
        {
            // Adjust for bounds which might (due to float/int conversions) be slightly bigger than the containing lane.
            bounds.Inflate(10, 10);
            mQuadTree = new QuadTree<Entity>(bounds);
            mObstacles = obstacles;
            mSelectionBorder = spriteManager.CreateSelectionBorder();
        }

        public void Add(Entity entity)
        {
            mQuadTree.Add(entity);
        }

        public bool HasEntityAt(Vector2 point)
        {
            return mQuadTree.HasEntityAt(point);
        }

        internal IEnumerable<Entity> EntitiesAt(Vector2 point)
        {
            return mQuadTree.EntitiesAt(point);
        }

        public void Update(PositionProvider positionProvider, GameTime gameTime, InputManager inputManager)
        {
            foreach (var entity in mQuadTree)
            {
                entity.Update(positionProvider, gameTime, inputManager);
            }

            foreach (var (a, b) in mQuadTree.Overlaps())
            {
                Console.WriteLine(
                    $"[COLLISION:]  UNIT {a} AND UNIT {b} ARE COLLIDING! [TIME:] {gameTime.TotalGameTime}");
            }

            mQuadTree.Rebuild();
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (var entity in mQuadTree)
            {
                if (entity.Selected)
                {
                    mSelectionBorder.Position = entity.Sprite.Position;
                    mSelectionBorder.Draw(spriteBatch, gameTime);
                }
                entity.Draw(spriteBatch, gameTime);
            }
        }
    }
}
