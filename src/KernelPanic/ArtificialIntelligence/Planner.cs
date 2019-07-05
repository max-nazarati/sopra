using System;
using KernelPanic.Entities;
using KernelPanic.Players;
using KernelPanic.Purchasing;

namespace KernelPanic.ArtificialIntelligence
{
    internal abstract class Planner
    {
        protected readonly Player mPlayer;

        protected Planner(Player player)
        {
            mPlayer = player;
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