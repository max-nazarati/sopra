using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace KernelPanic
{
    public class Entity
    {
        public Point? Position { get; set; }
        public int Price { get; set; }
        public bool Selected { get; set; }
        Currency Currency { get; }

        public Entity(int param)
        {

        }
        public Entity(TimeSpan timeSpan)
        {

        }

        /*internal void Update(GameTime gameTime, IPositionProvider positionProvider)
        {

        }*/

        /// <summary>
        /// old stuff below
        /// </summary>


        protected Rectangle mContainerRectangle;
        //private GraphicsDeviceManager mGraphics;
        //private Texture2D Texture;

        protected Entity(int x, int y, int width, int height)
        {
            mContainerRectangle = new Rectangle(new Point(x, y), new Point(width, height));
            //Texture = texture;
            //mGraphics = graphics;
        }
        internal virtual void Update()
        {
        }

        /*public virtual void Draw(SpriteBatch spriteBatch)
        {
            /*
            spriteBatch.Begin();
            spriteBatch.Draw(Texture, mContainerRectangle, Color.White);
            spriteBatch.End();
        }*/
        
    }
}
