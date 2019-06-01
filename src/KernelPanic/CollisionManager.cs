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
            // check for collisions between objects
            foreach (Entity movedObject in mObjectList)
            {
                foreach (Entity collidingObject in mObjectList)
                {
                    if (movedObject == collidingObject) continue;
                    if (movedObject.IsColliding(collidingObject))
                    {
                        movedObject.CollisionDetected();
                    }
                }
            }
        }
    }
}
