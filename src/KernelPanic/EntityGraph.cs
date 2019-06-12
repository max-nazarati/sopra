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
        private readonly Quadtree mQuadtree;
        private readonly ImageSprite mSelectionBorder;

        public EntityGraph(SpriteManager spriteManager)
        {
            mQuadtree = new Quadtree(1, new Rectangle(0, 0, 5000, 5000));
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
                        Console.WriteLine("Kollision");
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
