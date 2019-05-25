
namespace KernelPanic
{
    internal class Hero : Unit
    {
        public Hero(int param) : base(param)
        {

        }
        //public CooldownComponent Cooldown { get; set; }
        public bool AbilityAvailable()
        {
            return false;
        }

        public void ActivateAbility()
        {

        }
    }
}
