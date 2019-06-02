using System;
using System.Collections.Generic;

namespace KernelPanic
{
    internal sealed class CollisionManager
    {
        private readonly List<Entity> mObjectList;

        public CollisionManager()
        {
            mObjectList = new List<Entity>();
        }

        public void CreatedObject(Entity Object)
        {
            mObjectList.Add(Object);
        }

        public void Update()
        {
            // throw new NotImplementedException();
        }
    }
}
