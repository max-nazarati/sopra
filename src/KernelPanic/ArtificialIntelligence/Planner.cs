using System;
using KernelPanic.Entities;
using KernelPanic.Players;
using KernelPanic.Purchasing;

namespace KernelPanic.ArtificialIntelligence
{
    internal abstract class Planner
    {
        protected readonly Player Player;

        protected Planner(Player player, SpriteManager sprites)
        {
            Player = player;
        }
        
        protected void BuyEntity(PurchasableAction<Entity> entity, int amount=1)
        {
            for (var i = 0; i < amount; i++)
            {
                entity.TryPurchase(Player);
            }
        }
        
        protected static void EntityBought(Player buyer, Entity unit)
        {
            buyer.AttackingLane.UnitSpawner.Register(unit.Clone<Entity>());
        }
        
        public virtual void Update()
        {
            // Console.WriteLine(this + " is updating.");
        }
    }
}