using System.Runtime.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace KernelPanic
{
    [DataContract]
    internal sealed class Quadtree<T>: IEnumerable<T> where T: IBounded
    {
        #region Properties & Constants

        /// max number of objects in each Node
        private const int MaxObjects = 5;

        /// max depth of the Quadtree
        private const int MaximumDepth = 15;

        private readonly int mLevel;

        // size and position of the current node
        private Rectangle mBounds;

        [DataMember(Name = "Objects")]
        private readonly List<T> mObjects;

        [DataMember(Name = "Childs")]
        private Quadtree<T>[] mChilds;

        #endregion

        #region Constructor

        internal Quadtree(Rectangle bounds) : this(1, bounds)
        {
        }

        /// <summary>
        /// Creates a new <see cref="Quadtree{T}"/> which contains all elements from <paramref name="elements"/>.
        /// The bounds are inferred by the union of all <see cref="IBounded.Bounds"/>.
        /// </summary>
        /// <param name="elements">The elements for the <see cref="Quadtree{T}"/></param>
        /// <returns>A new <see cref="Quadtree{T}"/></returns>
        internal static Quadtree<T> Create(IEnumerable<T> elements)
        {
            var elementsArray = elements as List<T> ?? elements.ToList();
            var bounds =
                elementsArray.Aggregate(Rectangle.Empty, (rect, element) => Rectangle.Union(rect, element.Bounds));
            return new Quadtree<T>(bounds) {elementsArray};
        }

        private Quadtree(int level, Rectangle bounds)
        {
            mLevel = level;
            mBounds = bounds;
            mObjects = new List<T>();
        }

        #endregion

        #region Position Calculations
        
        private enum SquareIndex
        {
            TopLeft, TopRight, BottomLeft, BottomRight
        }

        /// <summary>
        /// Calculates in which of the 4 nodes a certain sprite fits
        /// </summary>
        private SquareIndex? CalculatePosition(T entity)
        {
            return CalculatePosition(entity.Bounds);
        }

        /// <summary>
        /// Calculates in which of the 4 squares a point is
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private SquareIndex? CalculatePosition(Vector2 point)
        {
            return CalculatePosition(Bounds.ContainingRectangle(point, Vector2.One));
        }

        private SquareIndex? CalculatePosition(Rectangle bounds)
        {
            var center = mBounds.Center;
            var isLeft = mBounds.Left <= bounds.Left && bounds.Right < center.X;
            var isRight = center.X < bounds.Left && bounds.Right <= mBounds.Right;
            var isTop = mBounds.Top <= bounds.Top && bounds.Bottom < center.Y;
            var isBottom = center.Y < bounds.Top && bounds.Bottom <= mBounds.Bottom;

            if (isLeft)
            {
                if (isTop) return SquareIndex.TopLeft;

                if (isBottom) return SquareIndex.BottomLeft;
            }

            if (isRight)
            {
                if (isTop) return SquareIndex.TopRight;

                if (isBottom) return SquareIndex.BottomRight;
            }

            return null;
        }

        #endregion

        #region Adding objects

        /// <summary>
        /// Splits the current node into 4 childs
        /// </summary>
        private void Split()
        {
            if (mChilds != null)
                throw new InvalidOperationException("Can't split an already split quad-tree.");

            int halfWidth = mBounds.Width / 2, halfHeight = mBounds.Height / 2;
            void Assign(SquareIndex square, int x, int y)
            {
                var bounds = new Rectangle(mBounds.X + x, mBounds.Y + y, halfWidth, halfHeight);
                mChilds[(int) square] = new Quadtree<T>(mLevel + 1, bounds);
            }

            mChilds = new Quadtree<T>[4];
            Assign(SquareIndex.TopLeft, 0, 0);
            Assign(SquareIndex.TopRight, halfWidth, 0);
            Assign(SquareIndex.BottomLeft, 0, halfHeight);
            Assign(SquareIndex.BottomRight, halfWidth, halfHeight);
        }

        /// <summary>
        /// Adds the given value to the quad-tree.
        /// </summary>
        /// <param name="entity">The value to be added.</param>
        /// <exception cref="InvalidOperationException">If <paramref name="entity"/> is outside this <see cref="Quadtree{T}"/>'s bounds.</exception>
        public void Add(T entity)
        {
            if (!mBounds.Contains(entity.Bounds))
                throw new InvalidOperationException(
                    $"Can't add {entity.Bounds} outside the quad-tree's bounds {mBounds}");

            if (mChilds != null)
            {
                if (CalculatePosition(entity) is SquareIndex index)
                {
                    mChilds[(int) index].Add(entity);
                    return;
                }
            }

            mObjects.Add(entity);

            if (mLevel >= MaximumDepth || mObjects.Count <= MaxObjects || mChilds != null)
            {
                // Don't split this level up if either
                //    * the maximum depth is reached
                //    * the maximum number of objects per level aren't reached
                //    * this level is already split.
                return;
            }
                
            Split();
            
            // Try to move all objects one level down.
            foreach (var value in new List<T>(mObjects))
            {
                if (CalculatePosition(value) is SquareIndex square)
                {
                    mChilds[(int) square].Add(value);
                    mObjects.Remove(value);
                }
            }
        }

        /*internal*/ private void Add(IEnumerable<T> elements)
        {
            foreach (var element in elements)
            {
                Add(element);
            }
        }
        
        internal void Rebuild()
        {
            var allEntities = new List<T>(this);
            mObjects.Clear();
            mChilds = null;
            foreach (var entity in allEntities)
            {
                Add(entity);
            }
        }

        #endregion

        #region Querying

        /// <summary>
        /// Checks, whether an entity exists at a given point
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        internal bool HasEntityAt(Vector2 point)
        {
            foreach (var @object in mObjects)
            {
                if (@object.Bounds.Contains(point))
                {
                    return true;
                }
            }

            if (mChilds != null && CalculatePosition(point) is SquareIndex square)
            {
                return mChilds[(int) square].HasEntityAt(point);
            }

            return false;
        }

        /// <summary>
        /// returns list of all entitys near to an entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        internal IEnumerable<T> NearObjects(T entity)
        {
            var entities = new List<T>();
            NearObjects(entity, entities);
            return entities;
        }

        private void NearObjects(T entity, List<T> nearEntities)
        {
            if (mChilds != null && CalculatePosition(entity) is SquareIndex index)
            {
                mChilds[(int) index].NearObjects(entity, nearEntities);
            }
            
            nearEntities.AddRange(mObjects);
        }

        #endregion

        #region Enumerating

        public IEnumerator<T> GetEnumerator()
        {
            // If mChilds is null enumerate only through mObjects, otherwise go through mObjects and then continue with the children.
            return (mChilds == null ? mObjects : mObjects.Concat(mChilds.SelectMany(c => c))).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}