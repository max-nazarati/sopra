using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KernelPanic.Data;
using KernelPanic.Entities;
using KernelPanic.Entities.Projectiles;
using KernelPanic.Entities.Units;
using KernelPanic.PathPlanning;
using KernelPanic.Table;
using Microsoft.Xna.Framework;

namespace KernelPanic
{
    internal sealed class PositionProvider
    {
        private readonly SpriteManager mSpriteManager;
        private readonly EntityGraph mEntities;

        internal Grid Grid { get; }
        internal Owner Owner { get; }
        internal Base Target { get; }
        internal TroupePathData TroupeData { get; }
        private readonly Hashtable mCache = new Hashtable(); 

        internal PositionProvider(Base target,
            Owner owner,
            Grid grid,
            EntityGraph entities,
            TroupePathData troupeData,
            SpriteManager spriteManager)
        {
            Grid = grid;
            mEntities = entities;
            mSpriteManager = spriteManager;
            TroupeData = troupeData;
            Target = target;
            Owner = owner;
        }

        #region Position Calculations

        internal TileIndex RequireTile(Entity entity)
        {
            return RequireTile(entity.Sprite.Position, entity is Troupe troupe && troupe.IsSmall ? 2 : 1);
        }

        internal TileIndex RequireTile(Vector2 position, int subTileCount = 1)
        {
            if (Grid.TileFromWorldPoint(position, subTileCount) is TileIndex tile)
                return tile;

            // pls dont change the exception name, hero.SlowPush depends on it
            throw new InvalidOperationException(
                $"Required a tile for {position} but it is not inside the lane {Grid.Bounds}");
        }

        #endregion

        #region Querying Entities

        internal IEnumerable<T> NearEntities<T>(Entity entity, float radius) where T : Entity
        {
            return mEntities.QuadTree
                .NearEntities(entity.Bounds.Center.ToVector2(), radius)
                .OfType<T>();
        }

        internal IEnumerable<T> NearEntities<T>(Vector2 position, float radius) where T : Entity
        {
            return mEntities.QuadTree
                .NearEntities(position, radius)
                .OfType<T>();
        }

        /// <summary>
        /// Enumerates through the entities overlapping with <paramref name="entity"/>.
        /// </summary>
        /// <param name="entity">The entity with which the overlaps are required.</param>
        /// <typeparam name="T">The entities are filtered by this type.</typeparam>
        /// <returns>An enumerable through entities of type <typeparamref name="T"/> overlapping with <paramref name="entity"/>.</returns>
        internal IEnumerable<T> EntitiesAt<T>(Entity entity) where T : Entity
        {
            return mEntities.QuadTree.EntitiesAt(entity.Bounds).OfType<T>();
        }

        internal List<IGameObject> EntitiesAt(Rectangle rectangle)
        {
            if (mCache[(rectangle.X / 25, rectangle.Y / 25)] is List<IGameObject> enumerable)
            {
                return enumerable;
            }
            var result = mEntities.QuadTree.EntitiesAt(rectangle).ToList();
            mCache[(rectangle.X / 25, rectangle.Y / 25)] = result;
            return result;
        }

        internal bool HasEntityAt(Vector2 point, Func<IGameObject, bool> predicate = null)
        {
            return mEntities.HasEntityAt(point, predicate);
        }

        #endregion

        #region Path Finding

        internal AStar MakePathFinding(Hero hero, Point[] target)
        {
            if (!(Grid.TileFromWorldPoint(hero.Sprite.Position) is TileIndex start))
                return new AStar();

            var otherHeroes = mEntities.AllEntities
                .Where(entity => entity is Hero && entity != hero)
                .SelectMaybe(entity => Grid.TileFromWorldPoint(entity.Sprite.Position)?.ToPoint());
            var aStar = new AStar(start.ToPoint(), target, TroupeData.BuildingMatrix, new HashSet<Point>(otherHeroes));
            aStar.CalculatePath();
            return aStar;
        }

        #endregion

        #region Visualization

        internal Visualizer Visualize(AStar pathPlanner)
        {
            return pathPlanner.CreateVisualization(Grid, mSpriteManager);
        }

        #endregion

        internal void AddProjectile(Projectile projectile)
        {
            mEntities.Add(projectile);
        }
    }
}
