using System;
using System.Collections.Generic;
using System.Linq;
using KernelPanic.Data;
using KernelPanic.Entities;
using KernelPanic.Entities.Buildings;
using KernelPanic.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    internal sealed class EntityGraph
    {
        #region Properties

        internal QuadTree<IGameObject> QuadTree { get; }

        private readonly SortedDictionary<int, List<IGameObject>> mDrawObjects =
            new SortedDictionary<int, List<IGameObject>>();

        private bool mMidUpdate;
        private readonly List<IGameObject> mMidUpdateBuffer = new List<IGameObject>();

        #endregion

        #region Constructor

        public EntityGraph(ICollection<LaneBorder> borders)
        {
            QuadTree = QuadTree<IGameObject>.Create(borders);
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

            if (QuadTree.Add(gameObject))
            {
                AddDrawObject(gameObject);
            }
            else
            {
                gameObject.WantsRemoval = true;
            }
        }

        private void AddDrawObject(IGameObject gameObject)
        {
            if (!(gameObject.DrawLevel is int level))
                return;

            if (mDrawObjects.TryGetValue(level, out var objects))
            {
                objects.Add(gameObject);
                return;
            }

            mDrawObjects[level] = new List<IGameObject> {gameObject};
        }
        
        #endregion
        
        #region Querying

        public bool HasEntityAt(Vector2 point, Func<IGameObject, bool> predicate = null)
        {
            return QuadTree.HasEntityAt(point, predicate);
        }

        internal bool HasIntersectingEntity(IGameObject gameObject, Func<IGameObject, bool> predicate = null)
        {
            var entities = QuadTree.EntitiesAt(gameObject.Bounds);
            return predicate == null ? entities.Any() : entities.Any(predicate);
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

            // Rebuild the quad-tree after movements and additions are done so that
            // overlaps and collisions can be determined correctly.
            MoveUnits(positionProvider, gameTime, inputManager);

            foreach (var gameObject in QuadTree.Rebuild(entity => entity.WantsRemoval))
            {
                gameObject.WantsRemoval = true;
            }

            foreach (var objects in mDrawObjects.Values)
            {
                objects.RemoveAll(@object => @object.WantsRemoval);
            }

            HandleCollisions(positionProvider);

            mMidUpdate = false;
            Add(mMidUpdateBuffer);
            mMidUpdateBuffer.Clear();
        }

        private void MoveUnits(PositionProvider positionProvider, GameTime gameTime, InputManager inputManager)
        {
            foreach (var entity in QuadTree)
            {
                if (!entity.WantsRemoval)
                    entity.Update(positionProvider, inputManager, gameTime);
            }
        }

        private void HandleCollisions(PositionProvider positionProvider)
        {
            foreach (var (a, b) in QuadTree.Overlaps())
            {
                CollisionManager.Handle(a, b, positionProvider);
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
