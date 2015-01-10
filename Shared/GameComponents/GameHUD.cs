using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace UeFGame.GameComponents
{
    /// <summary>
    /// Représente un conteneur de différents éléments du HUD.
    /// </summary>
    public class GameHUD
    {
        #region Variables

        #endregion

        #region Properties
        /// <summary>
        /// Obtient ou définit la liste des composants du HUD.
        /// </summary>
        public List<HUDComponent> Components { get; set; }
        public HUDComponents.MessageComponent MessageComponent { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Crée une nouvelle instance du HUD.
        /// </summary>
        public GameHUD()
        {
            Components = new List<HUDComponent>();
            MessageComponent = new HUDComponents.MessageComponent();
            Components.Add(MessageComponent);
        }

        /// <summary>
        /// Dessine les composants du HUD.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="batch"></param>
        public void Draw(GameTime time, SpriteBatch batch)
        {
            foreach (HUDComponent component in Components)
            {
                component.Draw(time, batch);
            }
        }
        #endregion
    }
}
