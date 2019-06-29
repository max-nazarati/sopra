using System;

namespace KernelPanic.ArtificalIntelligence
{
    
    internal sealed class ArtificialPlayer
    {
        private AttackPlanner mAttackPlanner;
        private DefencePlanner mDefencePlanner;
        private UpgradePlanner mUpgradePlanner;
        
        public ArtificialPlayer()
        {
            mAttackPlanner = new AttackPlanner();
            mDefencePlanner = new DefencePlanner();
            mUpgradePlanner = new UpgradePlanner();
        }

        public void Update()
        {
            Console.WriteLine("i dont know what to do T_T");
        }
    }
}