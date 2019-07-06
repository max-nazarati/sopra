using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using KernelPanic.Data;
using KernelPanic.Entities;
using KernelPanic.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace KernelPanic
{
    [JsonArray]
    internal sealed class EntityGraph
    {
        #region Properties

        [DataMember] internal QuadTree<IGameObject> QuadTree { get; }

        private readonly SortedDictionary<int, List<IGameObject>> mDrawObjects =
            new SortedDictionary<int, List<IGameObject>>();

        #endregion

        #region Constructor

        public EntityGraph(Rectangle bounds)
        {
            // Adjust for bounds which might (due to float/int conversions) be slightly bigger than the containing lane.
            bounds.Inflate(10, 10);
            QuadTree = new QuadTree<IGameObject>(bounds);
        }

        #endregion

        #region Modifying

        internal void Add(Entity entity)
        {
            QuadTree.Add(entity);
            AddDrawObject(entity);
        }

        internal void Add(IEnumerable<Entity> entities)
        {
            foreach (var entity in entities)
            {
                QuadTree.Add(entity);
                AddDrawObject(entity);
            }
        }

        private void AddDrawObject(IGameObject gameObject)
        {
            if (mDrawObjects.TryGetValue(gameObject.DrawLevel, out var objects))
            {
                objects.Add(gameObject);
                return;
            }

            mDrawObjects[gameObject.DrawLevel] = new List<IGameObject> {gameObject};
        }
        
        #endregion
        
        #region Querying

        public bool HasEntityAt(Vector2 point)
        {
            return QuadTree.HasEntityAt(point);
        }

        internal IEnumerable<Entity> EntitiesAt(Vector2 point)
        {
            return QuadTree.EntitiesAt(point).OfType<Entity>();
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

            var oldCount = QuadTree.Count;
            QuadTree.Rebuild(entity => !entity.WantsRemoval);
            var newCount = QuadTree.Count;

            if (oldCount == newCount)
            {
                // If there were no removals, there is no need to traverse the draw objects.
                return;
            }

            foreach (var objects in mDrawObjects.Values)
            {
                objects.RemoveAll(@object => @object.WantsRemoval);
            }
        }

        #endregion

        #region Drawing

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (var gameObject in mDrawObjects.SelectMany(kv => kv.Value))
            {
                gameObject.Draw(spriteBatch, gameTime);
            }
        }

        #endregion

        #region Enumerable

        internal IEnumerable<Entity> AllEntities => QuadTree.OfType<Entity>();

        internal IEnumerable<T> Entities<T>() where T : Entity => QuadTree.OfType<T>();

        #endregion
    }
}
