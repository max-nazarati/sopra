using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KernelPanic
{
    public class Quadtree
    {
        // max number of objects in each Node
        private static readonly int mMaxObjects = 10;
        
        // max depth of the Quadtree
        private static readonly int mMaximumDepth = 15;

        private readonly int mLevel;
        
        // size and position of the current node
        private readonly Rectangle mBounds;

        private readonly List<Texture2D> mObjects;

        private readonly Quadtree[] mChilds;

        public Quadtree(int level, Rectangle size)
        {
            mChilds = new Quadtree[4];
            mLevel = level;
            mBounds = size;
            mObjects = new List<Texture2D>();
        }

        /// <summary>
        /// Deletes recursively all nodes of the QuadTree
        /// </summary>
        public void Delete()
        {
            foreach (var child in mChilds)
            {
                child.Delete();
            }
            mObjects.Clear();
        }

        /// <summary>
        /// Calculates in which of the 4 nodes a certain sprite fits
        /// </summary>
        private int CalculatePosition(Texture2D texture)
        {
            var textureRectangle = texture.Bounds;
            var height = textureRectangle.Height;
            var width = textureRectangle.Width;
            var posX = textureRectangle.X;
            var posY = textureRectangle.Y;

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
        /// Splits the current node into 4 childs
        /// </summary>
        private void Split()
        {
            var halfWidth = mBounds.Width / 2;
            var halfHeight = mBounds.Height / 2;
            
            mChilds[0] = new Quadtree(mLevel+1, new Rectangle(mBounds.X, mBounds.Y, halfWidth, halfHeight));
            mChilds[1] = new Quadtree(mLevel+1, new Rectangle(mBounds.X+halfWidth, mBounds.Y, halfWidth, halfHeight));
            mChilds[2] = new Quadtree(mLevel+1, new Rectangle(mBounds.X+halfWidth, mBounds.Y+halfHeight, halfWidth, halfHeight));
            mChilds[3] = new Quadtree(mLevel+1, new Rectangle(mBounds.X, mBounds.Y+halfHeight, halfWidth, halfHeight));
        }

        public void AddSprite(Texture2D texture)
        {
            if (mChilds[0] != null)
            {
                var index = CalculatePosition(texture);
                if (index != -1)
                {
                    mChilds[index].AddSprite(texture);

                    return;
                }
            }
            
            mObjects.Add(texture);
            
            if (mLevel < mMaximumDepth && mObjects.Count > mMaxObjects)
            {
                if (mChilds[0] == null)
                {
                    Split();
                }
                
                // insert all elements from mObjects to the newly added Childs
                foreach (var Object in mObjects)
                {
                    var index = CalculatePosition(Object);
                    if (index != -1)
                    {
                        mChilds[index].AddSprite(Object);
                        mObjects.Remove(Object);
                    }
                }
            }
        }

        public List<Texture2D> NearObjects(Texture2D texture, List<Texture2D> returnList)
        {
            var index = CalculatePosition(texture);
            if (index != -1 && mChilds[0] != null)
            {
                NearObjects(texture, returnList);
            }
            
            returnList.AddRange(mObjects);
            return returnList;
        }
    }
}