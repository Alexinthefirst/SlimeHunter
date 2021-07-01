// ADAPTED FROM: http://rbwhitaker.wikidot.com/monogame-texture-atlases-2

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Roguelike
{
    /// <summary>
    /// Contains the definition for an animatedsprite
    /// </summary>
    public class AnimatedSprite
    {
        public Texture2D Texture { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public int currentFrame;
        public int totalFrames;
        public float delay = 0.1f;

        /// <summary>
        /// Constructor for an animatedsprite
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        public AnimatedSprite(Texture2D texture, int rows, int columns)
        {
            Texture = texture;
            Rows = rows;
            Columns = columns;
            currentFrame = 0;
            totalFrames = Rows * Columns;
        }

        /// <summary>
        /// Called every update
        /// </summary>
        /// <param name="gameTime">GameTime</param>
        public void Update(GameTime gameTime)
        {
            if (delay < 0)
            {
                currentFrame++;
                if (currentFrame == totalFrames)
                {
                    currentFrame = 0;
                }
                delay = 0.1f;
            } else
            {
                delay -= (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000;
            }
        }

        /// <summary>
        /// Called every Draw cycle
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch</param>
        /// <param name="location">Location to draw </param>
        public void Draw(SpriteBatch spriteBatch, Vector2 location)
        {
            int width = Texture.Width / Columns;
            int height = Texture.Height / Rows;
            int row = (int)((float)currentFrame / (float)Columns);
            int column = currentFrame % Columns;

            Rectangle sourceRectangle = new Rectangle(width * column, height * row, width, height);
            Rectangle destinationRectangle = new Rectangle((int)location.X, (int)location.Y, width, height);

            spriteBatch.Draw(Texture, destinationRectangle, sourceRectangle, Color.White);

        }
    }
}
