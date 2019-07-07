using System.Diagnostics.CodeAnalysis;
using KernelPanic.Entities;
using KernelPanic.Entities.Projectiles;

namespace KernelPanic
{
    internal static class CollisionManager
    {
        internal static void Handle(IGameObject object1, IGameObject object2, PositionProvider positionProvider)
        {
            if (!TryHandle(object1, object2, positionProvider))
                TryHandle(object2, object1, positionProvider);
        }

        [SuppressMessage("ReSharper", "InvertIf")]
        private static bool TryHandle(IGameObject object1, IGameObject object2, PositionProvider positionProvider)
        {
            if (object1 is Projectile projectile && object2 is Unit unit)
            {
                projectile.Hit(unit, positionProvider);
                return true;
            }

            return false;
        }
    }
}
