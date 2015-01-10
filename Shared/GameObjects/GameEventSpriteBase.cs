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
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision;
using FarseerPhysics.Common;

namespace UeFGame.GameObjects
{
    /// <summary>
    /// Abstract class for drawing Sprites.
    /// </summary>
    public abstract class GameEventSpriteBase
    {
        /* ----------------------------------------------------------------
         * Variables
         * --------------------------------------------------------------*/
        #region Variables
        /// <summary>
        /// Current texture used for the rendering.
        /// </summary>
        protected Texture2D m_currentTexture;
        #endregion
        /* ----------------------------------------------------------------
         * Properties
         * ---------------------------------------------------------------*/
        #region Properties
        /// <summary>
        /// Gets or sets the texture associated to this object.
        /// </summary>
        public Texture2D Texture
        {
            get { return m_currentTexture; }
            set { m_currentTexture = value; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Draws the sprite of the given GameEvent.
        /// Rules will be applied in fonction of the state of the UniqueBodyGameObject.
        /// </summary>
        public abstract void Draw(SpriteBatch batch, GameEvent evt, Vector2 scrollPx, float layerDepth);
        /// <summary>
        /// Draws the sprite of the given GameEvent.
        /// Rules will be applied in fonction of the state of the UniqueBodyGameObject.
        /// </summary>
        public abstract void DrawInEditor(SpriteBatch batch, GameEvent evt, Vector2 scrollPx, float layerDepth, Editor.GameObjectRenderOptions ops);
        #endregion

        #region Utils
        protected float sim(double v) { return ConvertUnits.ToSimUnits(v); }
        protected Vector2 sim(Vector2 v) { return ConvertUnits.ToSimUnits(v); }
        protected float disp(float v) { return ConvertUnits.ToDisplayUnits(v); }
        protected Vector2 disp(Vector2 v) { return ConvertUnits.ToDisplayUnits(v); }
        #endregion
    }
}
