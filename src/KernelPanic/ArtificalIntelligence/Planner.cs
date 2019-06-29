using System;

namespace KernelPanic.ArtificalIntelligence
{
    internal abstract class Planner
    {
        public virtual void Update()
        {
            Console.WriteLine(this + " is updating.");
        }
    }
}