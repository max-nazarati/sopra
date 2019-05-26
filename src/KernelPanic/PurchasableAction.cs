namespace KernelPanic
{
    internal class PurchasableAction
    {
        protected sealed class Resource : IPriced
        {
            public Resource(Currency currency, int price)
            {
                mCurrency = currency;
                mPrice = price;
            }

            private readonly Currency mCurrency;
            public Currency Currency => mCurrency;

            private readonly int mPrice;
            public int Price => mPrice;

        }

        private Resource mResource; // = new Resource(Currency.Bitcoin, 0);

        protected PurchasableAction(Currency currency, int price)
        {
            mResource = new Resource(currency, price);
        }

        /*
        public void Available(Player player)
        {

        }

        public void Delegate<T>(Player player, Resource resource)
        {

        }

        public void Purchase(Player player)
        {
            if (TryPurchase(player))
            {
                // buy
            }
            
        }
        
        private bool TryPurchase(Player player)
        {
            return player.Bitcoins >= mResource.Price;
        }

        // event;
        */
    }
}
