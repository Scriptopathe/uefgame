using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace UeFGame.Scenes
{
    public interface IScene
    {
        /// <summary>
        /// Updates the scene.
        /// </summary>
        /// <param name="gameTime"></param>
        void Update(GameTime gameTime);
        /// <summary>
        /// Draws the scene.
        /// The sprite batch Begin() and End() functions must be called in this method.
        /// </summary>
        /// <param name="batch"></param>
        void Draw(GameTime time, SpriteBatch batch);
        /// <summary>
        /// Disposes the scene
        /// </summary>
        void Dispose();
    }
}
