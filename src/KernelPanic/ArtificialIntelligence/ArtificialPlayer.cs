using System;

namespace KernelPanic.ArtificialIntelligence
{
    
    internal sealed class ArtificialPlayer
    {
        private AttackPlanner mAttackPlanner;
        private DefencePlanner mDefencePlanner;
        private UpgradePlanner mUpgradePlanner;

        private Planner[] mPlanners;
        
        public ArtificialPlayer(Player player, SpriteManager spriteManager)
        {
            mAttackPlanner = new AttackPlanner(player, spriteManager);
            mDefencePlanner = new DefencePlanner();
            mUpgradePlanner = new UpgradePlanner();
            mPlanners = new Planner[] {mAttackPlanner, mDefencePlanner, mUpgradePlanner};
        }

        public void Update()
        {
            foreach (var planner in mPlanners)
            {
                planner.Update();
            }
            Console.WriteLine(this + " is updating: 'i dont know what to do T_T'");
        }
    }
}