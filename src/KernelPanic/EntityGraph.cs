using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KernelPanic.Data;
using KernelPanic.Entities;
using KernelPanic.Input;
using KernelPanic.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace KernelPanic
{
    [JsonArray]
    internal sealed class EntityGraph : IEnumerable<Entity>
    {
        #region Properties

        [DataMember] internal QuadTree<Entity> QuadTree { get; }

        private readonly ImageSprite mSelectionBorder;

        #endregion

        #region Constructor

        public EntityGraph(Rectangle bounds, SpriteManager spriteManager)
        {
            // Adjust for bounds which might (due to float/int conversions) be slightly bigger than the containing lane.
            bounds.Inflate(10, 10);
            QuadTree = new QuadTree<Entity>(bounds);
            mSelectionBorder = spriteManager.CreateSelectionBorder();
        }

        #endregion

        #region Modifying

        internal void Add(Entity entity)
        {
            QuadTree.Add(entity);
        }

        internal void Add(IEnumerable<Entity> entities)
        {
            foreach (var entity in entities)
            {
                Add(entity);
            }
        }
        
        #endregion
        
        #region Querying

        public bool HasEntityAt(Vector2 point)
        {
            return QuadTree.HasEntityAt(point);
        }

        internal IEnumerable<Entity> EntitiesAt(Vector2 point)
        {
            return QuadTree.EntitiesAt(point);
        }
        
        #endregion

        #region Updating

        public void Update(PositionProvider positionProvider, GameTime gameTime, InputManager inputManager)
        {
            foreach (var entity in new List<Entity>(QuadTree))
            {
                if (entity is Unit)
                {
                    if (entity.mDidDie)
                        QuadTree.Remove(entity);
                    else
                        entity.Update(positionProvider, gameTime, inputManager);
                }
                else
                {
                    entity.Update(positionProvider, gameTime, inputManager);
                }
            }

            /*
            foreach (var (a, b) in QuadTree.Overlaps())
            {
                Console.WriteLine(
                    $"[COLLISION:]  UNIT {a} AND UNIT {b} ARE COLLIDING! [TIME:] {gameTime.TotalGameTime} [BOUNDS:] {a.Bounds} {b.Bounds}");
            }
            */
            QuadTree.Rebuild();
        }

        #endregion

        #region Drawing

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (var entity in QuadTree)
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
            return QuadTree.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) QuadTree).GetEnumerator();
        }

        #endregion
    }
}
