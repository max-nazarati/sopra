using System.Runtime.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KernelPanic.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    [DataContract]
    internal sealed class Quadtree: IEnumerable<Entity>
    {
        // max number of objects in each Node
        private static readonly int mMaxObjects = 5;

        // max depth of the Quadtree
        private static readonly int mMaximumDepth = 15;

        private readonly int mLevel;

        // size and position of the current node
        private readonly Rectangle mBounds;

        [DataMember(Name = "Objects")]
        private readonly List<Entity> mObjects;

        [DataMember(Name = "Childs")]
        private readonly List<Quadtree> mChilds;
        
        public Quadtree(int level, Rectangle size)
        {
            mLevel = level;
            mBounds = size;
            mObjects = new List<Entity>();
            mChilds = new List<Quadtree>();
        }
/*
        /// <summary>
        /// Deletes recursively all nodes of the QuadTree
        /// </summary>
        private void Clear(List<Entity> entityList)
        {
            foreach (var child in mChilds)
            {
                child.Clear(entityList);
            }

            entityList.AddRange(mObjects);
            mObjects.Clear();
        }
*/
        internal void Rebuild()
        {
            var allEntities = new List<Entity>(this);
            mObjects.Clear();
            mChilds.Clear();
            foreach (var entity in allEntities)
            {
                Add(entity);
            }
        }

        /// <summary>
        /// Calculates in which of the 4 nodes a certain sprite fits
        /// </summary>
        private int CalculatePosition(Entity entity)
        {
            var height = entity.Sprite.Height;
            var width = entity.Sprite.Width;
            var posX = entity.Sprite.X;
            var posY = entity.Sprite.Y;

            bool boolLeft, boolRight, boolTop, boolBottom;
            boolLeft = boolRight = boolTop = boolBottom = false;
            
            
            // 0 for topLeft, 1 for topRight, 2 for bottomRight, 3 for bottomLeft
            var index = 0;
            
            // left or right part
            if (posX > mBounds.X && (posX + width) < (mBounds.X + (mBounds.Width / 2)))
            {
                // texture fits in left part of node Bounds
                boolLeft = true;
            }
            if (posX > (mBounds.X + (mBounds.Width / 2)) && (posX + width) < (mBounds.X + mBounds.Width))
            {
                // texture fits in right part of node Bounds
                boolRight = true;
            }

            if (posY > mBounds.Y && (posY + height) < (mBounds.Y + (mBounds.Height / 2)))
            {
                boolTop = true;
            }
            
            if (posY > (mBounds.Y + (mBounds.Height / 2)) && (posY + height) < (mBounds.Y + mBounds.Height))
            {
                boolBottom = true;
            }

            if (boolLeft)
            {
                if (boolTop) return 0;

                if (boolBottom) return 3;
            }
            
            if (boolRight)
            {
                if (boolTop) return 1;

                if (boolBottom) return 2;
            }

            return -1;
        }
        
        /// <summary>
        /// Calculates in which of the 4 squares a point is
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private int CalculatePosition(Vector2 point)
        {
            var posX = point.X;
            var posY = point.Y;

            bool boolLeft, boolRight, boolTop, boolBottom;
            boolLeft = boolRight = boolTop = boolBottom = false;
            
            
            // 0 for topLeft, 1 for topRight, 2 for bottomRight, 3 for bottomLeft
            var index = 0;
            
            // left or right part
            if (posX > mBounds.X && posX < (mBounds.X + (mBounds.Width / 2)))
            {
                // texture fits in left part of node Bounds
                boolLeft = true;
            }
            if (posX > (mBounds.X + (mBounds.Width / 2)) && posX < (mBounds.X + mBounds.Width))
            {
                // texture fits in right part of node Bounds
                boolRight = true;
            }

            if (posY > mBounds.Y && posY < (mBounds.Y + (mBounds.Height / 2)))
            {
                boolTop = true;
            }
            
            if (posY > (mBounds.Y + (mBounds.Height / 2)) && posY < (mBounds.Y + mBounds.Height))
            {
                boolBottom = true;
            }

            if (boolLeft)
            {
                if (boolTop) return 0;

                if (boolBottom) return 3;
            }
            
            if (boolRight)
            {
                if (boolTop) return 1;

                if (boolBottom) return 2;
            }

            return -1;
        }

        
        /// <summary>
        /// Splits the current node into 4 childs
        /// </summary>
        private void Split()
        {
            var halfWidth = mBounds.Width / 2;
            var halfHeight = mBounds.Height / 2;
            
            mChilds.Add(new Quadtree(mLevel+1, new Rectangle(mBounds.X, mBounds.Y, halfWidth, halfHeight)));
            mChilds.Add(new Quadtree(mLevel+1, new Rectangle(mBounds.X+halfWidth, mBounds.Y, halfWidth, halfHeight)));
            mChilds.Add(new Quadtree(mLevel+1, new Rectangle(mBounds.X+halfWidth, mBounds.Y+halfHeight, halfWidth, halfHeight)));
            mChilds.Add(new Quadtree(mLevel+1, new Rectangle(mBounds.X, mBounds.Y+halfHeight, halfWidth, halfHeight)));
        }

        public void Add(Entity entity)
        {
            if (mChilds.Count > 0)
            {
                var index = CalculatePosition(entity);
                if (index != -1)
                {
                    mChilds[index].Add(entity);

                    return;
                }
            }
            
            mObjects.Add(entity);
            
            if (mLevel < mMaximumDepth && mObjects.Count > mMaxObjects)
            {
                if (mChilds.Count == 0)
                {
                    Split();
                }
                
                // insert all elements from mObjects to the newly added Childs
                foreach (var @object in new List<Entity>(mObjects))
                {
                    var index = CalculatePosition(@object);
                    if (index != -1)
                    {
                        mChilds[index].Add(@object);
                        mObjects.Remove(@object);
                    }
                }
            }
        }

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

            if (mChilds.Count == 0) return false;
            var index = CalculatePosition(point);
            return index != -1 && mChilds[index].HasEntityAt(point);
        }

        /// <summary>
        /// returns list of all entitys near to an entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        internal List<Entity> NearObjects(Entity entity)
        {
            var entities = new List<Entity>();
            NearObjects(entity, entities);
            return entities;
        }

        private void NearObjects(Entity entity, List<Entity> nearEntities)
        {
            var index = CalculatePosition(entity);
            if (index != -1 && mChilds.Count != 0)
            {
                mChilds[index].NearObjects(entity, nearEntities);
            }
            
            nearEntities.AddRange(mObjects);
        }

        public IEnumerator<Entity> GetEnumerator()
        {
            return mObjects.Concat(mChilds.SelectMany(c => c)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}