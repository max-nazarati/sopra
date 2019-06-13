using System;
﻿using System.Runtime.Serialization;
using KernelPanic.Entities;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    [DataContract]
    internal sealed class EntityGraph
    {
        [DataMember]
        private readonly Quadtree<Entity> mQuadtree;

        [DataMember]
        private readonly ObstacleMatrix mObstacles;

        private readonly ImageSprite mSelectionBorder;

        public EntityGraph(Rectangle bounds, ObstacleMatrix obstacles, SpriteManager spriteManager)
        {
            // Adjust for bounds which might (due to float/int conversions) be slightly bigger than the containing lane.
            bounds.Inflate(10, 10);
            mQuadtree = new Quadtree<Entity>(bounds);
            mObstacles = obstacles;
            mSelectionBorder = spriteManager.CreateSelectionBorder();
        }

        public void Add(Entity entity)
        {
            mQuadtree.Add(entity);
        }

        public bool HasEntityAt(Vector2 point)
        {
            return mQuadtree.HasEntityAt(point);
        }

        public void Update(PositionProvider positionProvider, GameTime gameTime, InputManager inputManager)
        {
            foreach (var entity in mQuadtree)
            {
                entity.Update(positionProvider, gameTime, inputManager);
            }

            // Checks whether collision works
            foreach (var entity in mQuadtree)
            {
                foreach (var nearEntity in mQuadtree.NearObjects(entity))
                {
                    if (entity != nearEntity && entity.Bounds.Intersects(nearEntity.Bounds))
                    {
                        string collision = "[KOLLISION:] " + " UNIT " + entity + " AND UNIT " + nearEntity + " ARE COLLIDING! [TIME:] " + gameTime.TotalGameTime;
                        Console.WriteLine(collision);
                    }
                }
            }
            mQuadtree.Rebuild();
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (var entity in mQuadtree)
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
