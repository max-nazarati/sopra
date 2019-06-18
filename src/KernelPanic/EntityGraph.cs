using System;
using System.Collections;
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
    internal sealed class EntityGraph : IEnumerable<Entity>
    {
        #region Properties

        [DataMember]
        private readonly QuadTree<Entity> mQuadTree;

        private readonly ImageSprite mSelectionBorder;

        #endregion

        #region Constructor

        public EntityGraph(Rectangle bounds, SpriteManager spriteManager)
        {
            // Adjust for bounds which might (due to float/int conversions) be slightly bigger than the containing lane.
            bounds.Inflate(10, 10);
            mQuadTree = new QuadTree<Entity>(bounds);
            mSelectionBorder = spriteManager.CreateSelectionBorder();
        }

        #endregion

        #region Modifying

        public void Add(Entity entity)
        {
            mQuadTree.Add(entity);
        }
        
        #endregion
        
        #region Querying

        public bool HasEntityAt(Vector2 point)
        {
            return mQuadTree.HasEntityAt(point);
        }

        internal IEnumerable<Entity> EntitiesAt(Vector2 point)
        {
            return mQuadTree.EntitiesAt(point);
        }
        
        #endregion

        #region Updating

        public void Update(PositionProvider positionProvider, GameTime gameTime, InputManager inputManager)
        {
            foreach (var entity in mQuadTree)
            {
                if (entity.GetType() != typeof(Tower))
                {
                    entity.Update(positionProvider, gameTime, inputManager);
                }
                else
                {
                    entity.Update(positionProvider, gameTime, inputManager, mQuadTree);
                }
            }

            foreach (var (a, b) in mQuadTree.Overlaps())
            {
                Console.WriteLine(
                    $"[COLLISION:]  UNIT {a} AND UNIT {b} ARE COLLIDING! [TIME:] {gameTime.TotalGameTime} [BOUNDS:] {a.Bounds} {b.Bounds}");
            }

            mQuadTree.Rebuild();
        }

        #endregion

        #region Drawing

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

        #endregion

        #region Enumerable

        public IEnumerator<Entity> GetEnumerator()
        {
            return mQuadTree.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) mQuadTree).GetEnumerator();
        }

        #endregion
    }
}
