using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using KernelPanic.Data;
using KernelPanic.Entities;
using KernelPanic.Entities.Buildings;
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

        private bool mMidUpdate;
        private readonly List<IGameObject> mMidUpdateBuffer = new List<IGameObject>();

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

        internal void Add(IGameObject gameObject)
        {
            if (mMidUpdate)
            {
                mMidUpdateBuffer.Add(gameObject);
            }
            else
            {
                RealAdd(gameObject);
            }
        }

        internal void Add(IEnumerable<IGameObject> gameObjects)
        {
            if (mMidUpdate)
            {
                mMidUpdateBuffer.AddRange(gameObjects);
                return;
            }

            foreach (var entity in gameObjects)
            {
                RealAdd(entity);
            }
        }

        private void RealAdd(IGameObject gameObject)
        {
            if (gameObject is Tower tower)
                tower.FireAction = Add;

            QuadTree.Add(gameObject);
            AddDrawObject(gameObject);
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
            // ABOUT UPDATING, DRAWING & REMOVAL OF GAME OBJECTS
            //
            // It seems we have to rebuild the quad-tree twice:
            //     1. After the movements are complete to determine the overlaps correctly.
            //     2. After handling the overlaps, because some units might have died.
            //
            // For now we can save us the second rebuilding step but in loops in the Update & Draw functions we have
            // to skip those game objects which have the WantsRemoval flag set. During the next quad-tree-rebuilding in
            // Update they get removed completely.

            mMidUpdate = true;

            foreach (var entity in QuadTree.Where(entity => !entity.WantsRemoval))
            {
                entity.Update(positionProvider, inputManager, gameTime);
            }

            // Rebuild the quad-tree after movements are done so that the overlaps can be determined correctly.
            QuadTree.Rebuild(entity => !entity.WantsRemoval);

            mMidUpdate = false;
            Add(mMidUpdateBuffer);
            mMidUpdateBuffer.Clear();
            mMidUpdate = true;

            foreach (var (a, b) in QuadTree.Overlaps())
            {
                CollisionManager.Handle(a, b);
            }

            foreach (var objects in mDrawObjects.Values)
            {
                objects.RemoveAll(@object => @object.WantsRemoval);
            }

            mMidUpdate = false;
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
