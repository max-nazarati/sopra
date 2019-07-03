using System.Linq;
using KernelPanic.Input;
using KernelPanic.Entities;
using KernelPanic.Players;
using KernelPanic.Purchasing;

namespace KernelPanic
{
    internal sealed class BuildingBuyer
    {
        private readonly Player mBuyer;
        private bool mSelected;

        internal Building Building { get; set; }
        internal PurchasableAction<Building> BuyAction { private get; set; }

        internal BuildingBuyer(Player player)
        {
            mBuyer = player;
        }

        internal void Update(InputManager input)
        {
            var position = input.TranslatedMousePosition;
            if (!mSelected && Building != null && mBuyer.DefendingLane.Contains(position))
            {
                mSelected = true;
                return;
            }

            if (Building != null && mBuyer.DefendingLane.Contains(position))
            {
                if (!input.MousePressed(InputManager.MouseButton.Left))
                    return;

                if (mBuyer.DefendingLane.EntityGraph.EntitiesAt(position).Any())
                {
                    mSelected = false;
                    Building = null;
                    return;
                }

                if (BuyAction.TryPurchase(mBuyer))
                {
                    mBuyer.DefendingLane.BuildingSpawner.Register(Building.Clone(), position);
                }
            }
            else
            {
                mSelected = false;
                Building = null;
            }
        }
    }
}
