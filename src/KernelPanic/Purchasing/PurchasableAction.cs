using System;
using KernelPanic.Players;

namespace KernelPanic.Purchasing
{
    internal class PurchasableAction<TResource> : IPriced where TResource : IPriced
    {
        public delegate void Delegate(Player buyer, TResource resource);

        public event Delegate Purchased;
        private readonly TResource mResource;

        internal PurchasableAction(TResource resource)
        {
            mResource = resource;
        }

        /// <summary>
        /// Creates a <see cref="PurchasableAction{TResource}"/> on which
        /// <see cref="TryPurchase(KernelPanic.Players.Player)"/> is executed.
        ///
        /// <para>
        /// This can be useful in situations where you have to construct a local <see cref="PurchasableAction{TResource}"/>.
        /// </para>
        /// </summary>
        /// <param name="buyer">The <see cref="Player"/> which buys the resource.</param>
        /// <param name="resource">The resource which is bought.</param>
        /// <returns><c>true</c> if the purchase was successful, otherwise <c>false</c>.</returns>
        internal static bool TryPurchase(Player buyer, TResource resource)
        {
            return new PurchasableAction<TResource>(resource).TryPurchase(buyer);
        }

        /// <summary>
        /// Checks if the given player can afford this action with his resources.
        /// </summary>
        /// Resources are either bitcoins or experience points.
        /// <param name="buyer">The potential buyer.</param>
        /// <returns>True if <paramref name="buyer"/> can afford the resource.</returns>
        internal virtual bool Available(Player buyer) =>
            mResource != null && PlayerResourcesLens(buyer).get() >= mResource.Price;

        /// <summary>
        /// Purchases the action if the given Player can afford it.
        /// </summary>
        /// <param name="buyer">The potential buyer.</param>
        /// <returns>True if <paramref name="buyer"/> bought the resource.</returns>
        internal bool TryPurchase(Player buyer)
        {
            var available = Available(buyer);
            if (available)
                Purchase(buyer);
            return available;
        }

        /// <summary>
        /// Purchases the action, that is it reduces the <paramref name="buyer"/>'s available resources.
        /// </summary>
        /// <param name="buyer">The Player whose available resources are</param>
        protected virtual void Purchase(Player buyer)
        {
            var resources = PlayerResourcesLens(buyer);
            resources.set(resources.get() - mResource.Price);
            Purchased?.Invoke(buyer, mResource);
        }

        /// <summary>
        /// Returns functions (a “lens”) to query and modify the given player's available resources. This uses
        /// <code>Resource.Currency</code> to query/modify either the buyer's bitcoins or experience points.
        /// </summary>
        /// <param name="buyer">The player whose resources will be queried/modified.</param>
        /// <returns>Functions to query and modify the resources of the given player.</returns>
        /// <exception cref="InvalidOperationException">throws if <code>Resource.Currency</code> is an unlisted value.</exception>
        private (Func<int> get, Action<int> set) PlayerResourcesLens(Player buyer)
        {
            switch (mResource.Currency)
            {
                case Currency.Bitcoin:
                    return (() => buyer.Bitcoins, i => buyer.Bitcoins = i);
                case Currency.Experience:
                    return (() => buyer.ExperiencePoints, i => buyer.ExperiencePoints = i);
                default:
                    throw new InvalidOperationException();
            }
        }

        public int Price => mResource.Price;

        public Currency Currency => mResource.Currency;
    }
}
