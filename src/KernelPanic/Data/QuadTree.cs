using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace KernelPanic.Data
{
    internal sealed class QuadTree<T>: IEnumerable<T> where T: IBounded
    {
        #region Properties & Constants

        /// max number of objects in each Node
        private const int MaxObjects = 5;

        /// max depth of the QuadTree
        private const int MaximumDepth = 15;

        private readonly int mLevel;

        // size and position of the current node
        private Rectangle mBounds;

        private readonly List<T> mObjects;
        
        private QuadTree<T>[] mChildren;

        /*internal*/ private int Count { get; /*private*/ set; }

        #endregion

        #region Constructor

        /*internal*/ private QuadTree(Rectangle bounds) : this(1, bounds)
        {
        }

        /// <summary>
        /// Creates a new <see cref="QuadTree{T}"/> which contains all elements from <paramref name="elements"/>.
        /// The bounds are inferred by the union of all <see cref="IBounded.Bounds"/>.
        /// </summary>
        /// <param name="elements">The elements for the <see cref="QuadTree{T}"/></param>
        /// <returns>A new <see cref="QuadTree{T}"/></returns>
        internal static QuadTree<T> Create<TInitial>(ICollection<TInitial> elements) where TInitial : T
        {
            return elements.Count == 0 ? Empty : new QuadTree<T>(elements.Union()) {elements.Cast<T>()};
        }

        /// <summary>
        /// Returns a new <see cref="QuadTree{T}"/> without any elements inside. The way this
        /// <see cref="QuadTree{T}"/> is constructed it is not possible to add any values to it.
        /// </summary>
        internal static QuadTree<T> Empty => new QuadTree<T>(Rectangle.Empty);

        private QuadTree(int level, Rectangle bounds)
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
            if (mChildren == null)
                throw new InvalidOperationException("Can't use CalculatePosition before Split.");

            return Squares
                .Where(square => square.Tree.mBounds.Contains(bounds))
                .Select(square => (SquareIndex?) square.Index)
                .FirstOrDefault();
        }

        private IEnumerable<(SquareIndex Index, Rectangle Rectangle)> OverlappingSquareIndices(Rectangle bounds)
        {
            return Squares
                .Select(square => (square.Index, Rectangle: Rectangle.Intersect(square.Tree.mBounds, bounds)))
                .Where(square => square.Rectangle.Size != Point.Zero);
        }

        private IEnumerable<(SquareIndex Index, QuadTree<T> Tree)> Squares =>
            mChildren == null
                ? Enumerable.Empty<(SquareIndex, QuadTree<T>)>()
                : Enumerable.Range(0, mChildren.Length).Select(index => ((SquareIndex) index, mChildren[index]));

        #endregion

        #region Adding & Removing

        /// <summary>
        /// Splits the current node into 4 children
        /// </summary>
        private void Split()
        {
            if (mChildren != null)
                throw new InvalidOperationException("Can't split an already split quad-tree.");

            int halfWidth = mBounds.Width / 2, halfHeight = mBounds.Height / 2;
            void Assign(SquareIndex square, int x, int y)
            {
                var bounds = new Rectangle(mBounds.X + x, mBounds.Y + y, halfWidth, halfHeight);
                mChildren[(int) square] = new QuadTree<T>(mLevel + 1, bounds);
            }

            mChildren = new QuadTree<T>[4];
            Assign(SquareIndex.TopLeft, 0, 0);
            Assign(SquareIndex.TopRight, halfWidth, 0);
            Assign(SquareIndex.BottomLeft, 0, halfHeight);
            Assign(SquareIndex.BottomRight, halfWidth, halfHeight);
        }

        /// <summary>
        /// Adds the given value to the quad-tree.
        /// </summary>
        /// <param name="entity">The value to be added.</param>
        /// <exception cref="InvalidOperationException">If <paramref name="entity"/> is outside this <see cref="QuadTree{T}"/>'s bounds.</exception>
        public void Add(T entity)
        {
            if (!mBounds.Contains(entity.Bounds))
                throw new InvalidOperationException(
                    $"Can't add {entity.Bounds} outside the quad-tree's bounds {mBounds}: {entity}");

            Count++;

            if (mChildren != null && CalculatePosition(entity) is SquareIndex index)
            {
                mChildren[(int) index].Add(entity);
                return;
            }

            mObjects.Add(entity);

            if (mLevel >= MaximumDepth || mObjects.Count <= MaxObjects || mChildren != null)
            {
                // Don't split this level up if either
                //    * the maximum depth is reached
                //    * the maximum number of objects per level aren't reached
                //    * this level is already split.
                return;
            }
                
            Split();
            
            // Try to move all objects one level down.
            var indicesToRemove = new List<int>(mObjects.Count);
            for (var i = 0; i < mObjects.Count; i++)
            {
                var value = mObjects[i];
                if (!(CalculatePosition(value) is SquareIndex square))
                    continue;

                mChildren[(int) square].Add(value);
                indicesToRemove.Add(i);
            }

            for (var i = indicesToRemove.Count - 1; i >= 0; i--)
            {
                mObjects.RemoveAt(indicesToRemove[i]);
            }
        }

        /*internal*/ private void Add(IEnumerable<T> elements)
        {
            foreach (var element in elements)
            {
                Add(element);
            }
        }

        #endregion

        #region Rebuilding
        
        /// <summary>
        /// Rebuilds this <see cref="QuadTree{T}"/> using the updated bounds. If <paramref name="removePredicate"/> is
        /// given, all objects are removed for which <c>true</c> is returned.
        /// </summary>
        /// <param name="removePredicate">Used to filter the objects.</param>
        internal void Rebuild(Func<T, bool> removePredicate = null)
        {
            var sink = new List<T>();
            RebuildImpl(removePredicate, sink);
            Add(sink);
        }

        private void RebuildImpl(Func<T, bool> removePredicate, List<T> parentSink)
        {
            RebuildObjects(removePredicate, parentSink);
            
            if (mChildren != null)
                RebuildChildren(removePredicate, parentSink);
        }

        private void RebuildObjects(Func<T, bool> removePredicate, ICollection<T> parentSink)
        {
            var bounds = mBounds;
            var removed = mObjects.RemoveAll(value =>
            {
                if (removePredicate != null && removePredicate(value))
                    return true;
                if (bounds.Contains(value.Bounds))
                    return false;
                parentSink.Add(value);
                return true;
            });

            Count -= removed;
        }

        private void RebuildChildren(Func<T, bool> removePredicate, List<T> parentSink)
        {
            var index = parentSink.Count;
            foreach (var child in mChildren)
                child.RebuildImpl(removePredicate, parentSink);

            var count = parentSink.Count;
            Count -= count - index;

            for (var i = index; i < count; ++i)
            {
                var value = parentSink[i];
                if (!mBounds.Contains(value.Bounds))
                {
                    if (i != index)
                        parentSink[index] = value;

                    ++index;
                    continue;
                }

                Add(value);
            }
            
            parentSink.RemoveRange(index, count - index);
        }

        #endregion

        #region Querying

        /// <summary>
        /// Returns every value in the <see cref="QuadTree{T}"/> that has <see cref="IBounded.Bounds"/>
        /// containing the given point.
        /// </summary>
        /// <param name="point">The point to look at.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with all values that match.</returns>
        internal IEnumerable<T> EntitiesAt(Vector2 point)
        {
            var tree = this;

            while (true)
            {
                // Yield all objects in the current level.
                foreach (var value in tree.mObjects)
                {
                    if (value.Bounds.Contains(point))
                        yield return value;
                }

                // Move one level down.
                if (tree.mChildren != null && tree.CalculatePosition(point) is SquareIndex square)
                {
                    tree = tree.mChildren[(int) square];
                }
                else
                {
                    yield break;
                }
            }
        }

        /// <summary>
        /// Returns every value in the <see cref="QuadTree{T}"/> that has <see cref="IBounded.Bounds"/>
        /// intersecting with the given rectangle.
        /// </summary>
        /// <param name="rectangle">The rectangle to look at.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with all matching values.</returns>
        internal IEnumerable<T> EntitiesAt(Rectangle rectangle)
        {
            IEnumerable<T> Locals(QuadTree<T> quadTree) =>
                quadTree.mObjects.Where(value => rectangle.Intersects(value.Bounds));

            IEnumerable<T> Children((SquareIndex Index, Rectangle Rectangle) square) =>
                mChildren[(int) square.Index].EntitiesAt(square.Rectangle);

            return Locals(this).Concat(OverlappingSquareIndices(rectangle).SelectMany(Children));
        }

        /// <summary>
        /// Checks whether an entity exists in the <see cref="QuadTree{T}"/> that has <see cref="IBounded.Bounds"/>
        /// containing the given point.
        /// </summary>
        /// <param name="point">The point to check for.</param>
        /// <param name="predicate">An optional filter for the entities at <paramref name="point"/>.</param>
        /// <returns><c>true</c> if such an entity is found, <c>false</c> otherwise.</returns>
        internal bool HasEntityAt(Vector2 point, Func<T, bool> predicate = null)
        {
            return predicate == null ? EntitiesAt(point).Any() : EntitiesAt(point).Any(predicate);
        }

        internal IEnumerable<T> NearEntities(Vector2 point, float radius)
        {
            var rectangle = Bounds.ContainingRectangle(point - new Vector2(radius), new Vector2(radius*2));
            return Overlapping(point, radius, rectangle);
        }

        private IEnumerable<T> Overlapping(Vector2 point, float radius, Rectangle rectangle)
        {
            if (!mBounds.Intersects(rectangle))
                yield break;
            foreach (var o in mObjects)
            {
                if (Geometry.CircleIntersect(point, radius, o.Bounds))
                    yield return o;
            }

            if (mChildren == null)
                yield break;

            foreach (var child in mChildren)
            {
                foreach (var o in child.Overlapping(point, radius, rectangle))
                {
                    yield return o;
                }
            }
        }

        /// <summary>
        /// Enumerates through all overlapping elements in the <see cref="QuadTree{T}"/>.
        ///
        /// <para>
        /// This function defines a irreflexive, asymmetric relation. Note that the symmetric closure of this relation
        /// would still be correct in terms of overlapping elements but incurs a usage overhead because one doesn't care
        /// about the order of two overlapping elements.
        /// </para>
        /// </summary>
        /// <returns>All pairs of overlapping elements.</returns>
        internal IEnumerable<(T, T)> Overlaps()
        {
            return LocalOverlaps(Array.Empty<T>()).Concat(ChildOverlaps(mObjects));
        }

        private IEnumerable<(T, T)> ChildOverlaps(IReadOnlyCollection<T> parentElements)
        {
            if (mChildren == null)
                return Enumerable.Empty<(T, T)>();

            var parentElementsCount = parentElements.Count;
            var parentElementsCopy = parentElementsCount == 0 ? null : new List<T>(parentElements);

            return mChildren.Where(tree => tree.Count > 0).SelectMany(tree =>
            {
                if (parentElementsCopy != null)
                {
                    var removeCount = parentElementsCopy.Count - parentElementsCount;
                    parentElementsCopy.RemoveRange(parentElementsCount, removeCount);
                    parentElementsCopy.AddRange(tree.mObjects);
                }

                var locals = tree.LocalOverlaps(parentElements);
                var children = tree.ChildOverlaps(parentElementsCopy ?? tree.mObjects);
                return locals.Concat(children);
            });
        }

        /// <summary>
        /// Enumerates through all overlaps between elements in <see cref="mObjects"/> and through overlaps between
        /// elements in <see cref="mObjects"/> and <paramref name="parentElements"/>.
        /// </summary>
        /// <param name="parentElements">Elements from upper levels which might overlap with elements from this level.</param>
        /// <returns>All overlaps.</returns>
        private IEnumerable<(T, T)> LocalOverlaps(IReadOnlyCollection<T> parentElements)
        {
            for (var i = 0; i < mObjects.Count; ++i)
            {
                var x = mObjects[i];
                for (var j = i + 1; j < mObjects.Count; ++j)
                {
                    var y = mObjects[j];
                    if (x.Bounds.Intersects(y.Bounds))
                        yield return (x, y);
                }

                foreach (var z in parentElements)
                {
                    if (x.Bounds.Intersects(z.Bounds))
                        yield return (x, z);
                }
            }
        }

        #endregion

        #region Enumerating

        public IEnumerator<T> GetEnumerator()
        {
            // If mChildren is null we will enumerate only through mObjects, otherwise we will go through mObjects and
            // then continue with the children.
            return (mChildren == null ? mObjects : mObjects.Concat(mChildren.Flatten())).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Debug Visualisation

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            ToString("", stringBuilder);
            return stringBuilder.ToString();
        }

        private void ToString(string indent, StringBuilder output)
        {
            output
                .Append(indent)
                .Append("- Count: ")
                .Append(Count)
                .Append("  Bounds: ")
                .Append(mBounds)
                .Append('\n');

            indent += "   ";

            foreach (var o in mObjects)
            {
                output
                    .Append(indent)
                    .Append("- ")
                    .Append(o.Bounds)
                    .Append(' ')
                    .Append(o)
                    .Append('\n');
            }

            foreach (var child in mChildren?.AsEnumerable() ?? Enumerable.Empty<QuadTree<T>>())
            {
                child.ToString(indent, output);
            }
        }

        #endregion
    }
}
