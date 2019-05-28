using System;

namespace KernelPanic
{
    internal class PurchasableAction<TResource> where TResource : IPriced
    {
        public delegate void Delegate(Player buyer, TResource resource);

        public event Delegate Purchased;
        
        private TResource Resource { get; }

        protected PurchasableAction(TResource resource)
        {
            Resource = resource;
        }
        
        internal virtual bool Available(Player buyer) => ResourceModifier(buyer).get() >= Resource.Price;

        internal bool TryPurchase(Player buyer)
        {
            var available = Available(buyer);
            if (available)
                Purchase(buyer);
            return available;
        }

        internal virtual void Purchase(Player buyer)
        {
            var resources = ResourceModifier(buyer);
            resources.set(resources.get() - Resource.Price);
            Purchased?.Invoke(buyer, Resource);
        }

        private (Func<int> get, Action<int> set) ResourceModifier(Player buyer)
        {
            switch (Resource.Currency)
            {
                case Currency.Bitcoin:
                    return (() => buyer.Bitcoins, i => buyer.Bitcoins = i);
                case Currency.Experience:
                    return (() => buyer.ExperiencePoints, i => buyer.ExperiencePoints = i);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
