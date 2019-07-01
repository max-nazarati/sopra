using KernelPanic.Players;

namespace KernelPanic.Purchasing
{
    internal sealed class SinglePurchasableAction<TResource> : PurchasableAction<TResource>
        where TResource : IPriced
    {
        internal bool IsPurchased { get; set; }

        internal SinglePurchasableAction(TResource resource, bool isPurchased = false) : base(resource)
        {
            IsPurchased = isPurchased;
        }

        internal override bool Available(Player buyer) => !IsPurchased && base.Available(buyer);

        protected override void Purchase(Player buyer)
        {
            IsPurchased = true;
            base.Purchase(buyer);
        }
    }
}
