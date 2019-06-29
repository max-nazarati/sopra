using System;

namespace KernelPanic.ArtificalIntelligence
{
    
    internal sealed class ArtificialPlayer
    {
        private AttackPlanner mAttackPlanner;
        private DefencePlanner mDefencePlanner;
        private UpgradePlanner mUpgradePlanner;

        private Planner[] mPlanners;
        public ArtificialPlayer()
        {
            mAttackPlanner = new AttackPlanner();
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