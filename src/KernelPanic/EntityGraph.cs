﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    internal sealed class EntityGraph
    {
        private readonly Quadtree mQuadtree;
        private readonly ImageSprite mSelectionBorder;

        public EntityGraph(SpriteManager spriteManager)
        {
            mQuadtree = new Quadtree(1, new Rectangle(0, 0, 2000, 2000));
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

        public void Update(PositionProvider positionProvider, GameTime gameTime, Matrix invertedViewMatrix)
        {
            foreach (var entity in mQuadtree)
            {
                entity.Update(positionProvider, gameTime, invertedViewMatrix);
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
