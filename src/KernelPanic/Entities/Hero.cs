
namespace KernelPanic
{
    internal class Hero : Unit
    {
        public Hero(int price) : base(price)
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
