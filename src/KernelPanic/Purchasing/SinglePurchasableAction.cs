namespace KernelPanic.Purchasing
{
    internal sealed class SinglePurchasableAction<TResource> : PurchasableAction<TResource>
        where TResource : class, IPriced
    {
        private bool IsPurchased { get; set; }

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
