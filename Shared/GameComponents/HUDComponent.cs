using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace UeFGame.GameComponents
{
    /// <summary>
    /// Représente un composant du HUD.
    /// </summary>
    public abstract class HUDComponent
    {
        /// <summary>
        /// Dessine le composant du HUD.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="batch"></param>
        public abstract void Draw(GameTime time, SpriteBatch batch);
    }
}
