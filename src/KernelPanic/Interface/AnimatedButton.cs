using KernelPanic.Entities.Units;
 using KernelPanic.Input;
 using KernelPanic.Sprites;
 using Microsoft.Xna.Framework;
 using Microsoft.Xna.Framework.Graphics;
 
 namespace KernelPanic.Interface
 {
     internal class AnimatedButton : TextButton
     {
         private readonly ImageSprite mButtonOverlay;
         private readonly Hero mHero;
         internal AnimatedButton(SpriteManager sprites, Hero hero, int width = 250, int height = 70) : base(sprites, width, height)
         {
             mHero = hero;
             mButtonOverlay = sprites.CreateColoredRectangle(1, 1, new Color[]{new Color(0.8f, 0.8f, 0.8f, 0.5f)});
         }
         
         public override void Update(InputManager inputManager, GameTime gameTime)
         {
             base.Update(inputManager, gameTime);
             var width = (int) (50 * mHero.RemainingCooldownTime);
             mButtonOverlay.DestinationRectangle = new Rectangle((int)Position.X, (int)Position.Y, width, 70);
         }
         
         public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
         {
             base.Draw(spriteBatch, gameTime);
             mButtonOverlay.Draw(spriteBatch, gameTime);
         }
     }
 }