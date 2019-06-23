using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using KernelPanic.Entities;
using KernelPanic.Sprites;
using KernelPanic.Interface;
using KernelPanic.Purchasing;

namespace KernelPanic
{
    class BuyingMenuOverlay
    {
        internal IEnumerable<IDrawable> Drawables{ get; private set; }
        internal IEnumerable<IUpdatable> Updatables { get; private set; }

        #region Troupes
        
        protected PurchaseButton<ImageButton, Unit> CreateTrojanPurchaseButton(SpriteManager spriteManager, Player player)
        {
            var sprite = spriteManager.CreateTrojan();
            var btn = new PurchaseButton<ImageButton, Unit>(player,
                new PurchasableAction<Unit>(new Trojan(spriteManager)), new ImageButton(spriteManager, 
                sprite.getSingleFrame(spriteManager), 70, 70));
            {
                
            };
            var trojanSprite = btn.Button.Sprite;
            trojanSprite.Position = new Vector2(spriteManager.ScreenSize.X - 2*trojanSprite.Width, 90 + trojanSprite.Height);
            return btn;
        }
        
        protected PurchaseButton<ImageButton, Unit> CreateBugPurchaseButton(SpriteManager spriteManager, Player player)
        {
            var sprite = spriteManager.CreateBug();
            var bugButton = new PurchaseButton<ImageButton, Unit>(player,
                new PurchasableAction<Unit>(new Bug(spriteManager)),
                new ImageButton(spriteManager, sprite.getSingleFrame(spriteManager), 70, 70));
            var tmpSprite = bugButton.Button.Sprite;
            tmpSprite.Position = new Vector2(spriteManager.ScreenSize.X - 2*tmpSprite.Width, 90 + 2*tmpSprite.Height);
            return bugButton;
        }

        #endregion
        
        #region Heroes
        
        protected PurchaseButton<ImageButton, Unit> CreateFirefoxPurchaseButton(SpriteManager spriteManager, Player player)
        {
            var sprite = spriteManager.CreateFirefox();
            var firefoxButton = new PurchaseButton<ImageButton, Unit>(player,
                new PurchasableAction<Unit>(new Firefox(spriteManager)),
                new ImageButton(spriteManager, sprite.getSingleFrame(spriteManager), 70, 70));
            var tmpSprite = firefoxButton.Button.Sprite;
            tmpSprite.Position = new Vector2(spriteManager.ScreenSize.X - 2*tmpSprite.Width, 90);
            return firefoxButton;
        }
        
        #endregion
    }
}
