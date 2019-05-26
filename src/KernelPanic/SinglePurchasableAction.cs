namespace KernelPanic
{
    class SinglePurchasableAction : PurchasableAction
    {
        protected SinglePurchasableAction(Currency currency, int price, Resource resource) : base(currency, price)
        {
            // nothing else to do then calling the parent constructor i guess
        }
        private Resource mResource;
        private bool mPurchased = false;

        private bool Purchased
        {
            get => mPurchased;
            set => mPurchased = value;
        }

    }
}
