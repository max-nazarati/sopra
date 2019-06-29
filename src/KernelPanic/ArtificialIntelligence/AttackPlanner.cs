using KernelPanic.Entities;
using KernelPanic.Interface;
using KernelPanic.Purchasing;

namespace KernelPanic.ArtificialIntelligence
{
    internal sealed class AttackPlanner : Planner
    {
        /*
        TextButton, Unit, PurchasableAction<Unit>>(player,
        new PurchasableAction<Unit>(new Firefox(sprites)),
        new TextButton(sprites))
        {
            Button = { Title = "Firefox" }}
            */
        /*
         private PurchaseButton<TextButton, Unit, PurchasableAction<Unit>> mPurchaseDemoButton1;
         
        mPurchaseDemoButton1 = new PurchaseButton<TextButton, Unit, PurchasableAction<Unit>>(player,
        new PurchasableAction<Unit>(new Firefox(sprites)),
        new TextButton(sprites))
        {
            Button = { Title = "Firefox" }
        };
    */
        
        private readonly PurchaseButton<TextButton, Unit, PurchasableAction<Unit>> mBuyBug;
        private readonly Player mPlayer;

        public AttackPlanner(Player player, SpriteManager sprites)
        {
            mPlayer = player;
        
            mBuyBug = new PurchaseButton<TextButton, Unit, PurchasableAction<Unit>>(player, new PurchasableAction<Unit>(new Bug(sprites)),
                new TextButton(sprites))
            {
                Button = { Title = "Bug" }
            };
        }

        private void BuyBug()
        {
            mBuyBug.Action.TryPurchase(mPlayer);
        }

        public override void Update()
        {
            base.Update();
            BuyBug();
        }
    }
}