using KernelPanic.Entities;
using KernelPanic.Entities.Projectiles;

namespace KernelPanic
{
    internal static class CollisionManager
    {
        internal static void Handle(IGameObject object1, IGameObject object2)
        {
            if (TryHandle(object1, object2) || TryHandle(object2, object1))
            {
            }
        }

        private static bool TryHandle(IGameObject object1, IGameObject object2)
        {
            if (object1 is Projectile projectile && object2 is Unit unit)
            {
                projectile.Hit(unit);
                return true;
            }

            return false;
        }
    }
}
