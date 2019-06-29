using System;

namespace KernelPanic.ArtificialIntelligence
{
    internal abstract class Planner
    {
        public virtual void Update()
        {
            Console.WriteLine(this + " is updating.");
        }
    }
}