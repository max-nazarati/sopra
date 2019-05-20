using System.Collections.Generic;

namespace Collision
{
    public sealed class CollisionManager
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
            foreach (var movedObject in mObjectList)
            {
                foreach (var collidingObject in mObjectList)
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
