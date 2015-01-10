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
using UeFGame.GameComponents;
namespace UeFGame.Scenes
{
    public class SceneGame : IScene
    {
        /* --------------------------------------------------------------------------------
        * Variables
        * -------------------------------------------------------------------------------*/
        #region Variables
        /// <summary>
        /// The map of this scene
        /// </summary>
        GameMap m_map;
        #endregion

        /* --------------------------------------------------------------------------------
        * Basics / Scene Implementation
        * -------------------------------------------------------------------------------*/
        #region Basics / Scene Implementation
        /// <summary>
        /// Initializes the Scene.
        /// </summary>
        public SceneGame()
        {
            UeFGame.GameDatabase.Load();
            m_map = new GameMap();
        }
        /// <summary>
        /// Updates the scene.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            m_map.Update(gameTime);
        }
        /// <summary>
        /// Draws the scene.
        /// The sprite batch Begin() and End() functions must be called in this method.
        /// </summary>
        /// <param name="batch"></param>
        public void Draw(GameTime time, SpriteBatch batch)
        {
            batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            m_map.Draw(time, batch);
            batch.End();
        }
        /// <summary>
        /// Disposes the scene
        /// </summary>
        public void Dispose()
        {
            m_map.Dispose();
        }
        #endregion
    }
}
