using System;
using KernelPanic.Players;
using KernelPanic.Purchasing;
using KernelPanic.Upgrades;

namespace KernelPanic.ArtificialIntelligence
{
    internal sealed class UpgradePlanner : Planner
    {
        // private readonly Func<Upgrade.Id, SinglePurchasableAction<Upgrade>> mUpgradeLookup;

        public UpgradePlanner(Player player, Func<Upgrade.Id, SinglePurchasableAction<Upgrade>> upgradeLookup) : base(player)
        {
           // mUpgradeLookup = upgradeLookup;
        }
    }
}