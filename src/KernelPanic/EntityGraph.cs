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

        #endregion

        #region Constructor

        public EntityGraph(Rectangle bounds)
        {
            // Adjust for bounds which might (due to float/int conversions) be slightly bigger than the containing lane.
            bounds.Inflate(10, 10);
            QuadTree = new QuadTree<Entity>(bounds);
        }

        #endregion

        #region Modifying

        internal void Add(Entity entity)
        {
            QuadTree.Add(entity);
        }

        internal void Add(IEnumerable<Entity> entities)
        {
            QuadTree.Add(entities);
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
            foreach (var entity in QuadTree)
            {
                entity.Update(positionProvider, inputManager, gameTime);
            }

            /*
            foreach (var (a, b) in QuadTree.Overlaps())
            {
                Console.WriteLine(
                    $"[COLLISION:]  UNIT {a} AND UNIT {b} ARE COLLIDING! [TIME:] {gameTime.TotalGameTime} [BOUNDS:] {a.Bounds} {b.Bounds}");
            }
            */
            QuadTree.Rebuild(entity => !entity.WantsRemoval);
        }

        #endregion

        #region Drawing

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (var entity in QuadTree)
            {
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
