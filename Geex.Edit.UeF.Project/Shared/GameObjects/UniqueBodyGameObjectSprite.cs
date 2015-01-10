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
    /// Sprite used for the UniqueBodyGameObjects
    /// </summary>
    public class UniqueBodyGameObjectSprite
    {
        /* ----------------------------------------------------------------
         * Variables
         * --------------------------------------------------------------*/
        #region Variables
        /// <summary>
        /// Current texture used for the rendering.
        /// </summary>
        Texture2D m_currentTexture;
        /// <summary>
        /// Source rectangle of the current texture.
        /// It will be modified in draw in fonction of the GameObject's state
        /// and the frame that should be displayed.
        /// </summary>
        Rectangle m_currentTextureSrcRect = new Rectangle();
        /// <summary>
        /// Destination rect instance, it will only be modified and used in the "Draw" method.
        /// </summary>
        Rectangle m_destRect = new Rectangle();
        /// <summary>
        /// Color of the sprite which will be displayed.
        /// </summary>
        protected Color m_tone = Color.White;
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
        /// <summary>
        /// Gets or sets the tone associated to this object.
        /// </summary>
        public Color Tone
        {
            get { return m_tone; }
            set { m_tone = value; }
        }
        #endregion
        #region Methods


        /// <summary>
        /// Draws the sprite of the UniqueBodyGameObject.
        /// Rules will be applied in fonction of the state of the UniqueBodyGameObject.
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="realPosition">In simulation units</param>
        /// <param name="rotation"></param>
        /// <param name="scrollPx"></param>
        /// <param name="fixtureOffset"></param>
        /// <param name="sizePx"></param>
        public void Draw(SpriteBatch batch, Vector2 realPosition, float rotation, Vector2 scrollPx, Vector2 fixtureOffset, Vector2 sizePx)
        {
            // Draws the texture
            m_destRect.X = (int)disp(realPosition.X) - (int)scrollPx.X;
            m_destRect.Y = (int)disp(realPosition.Y) - (int)scrollPx.Y;
            m_destRect.Width = (int)sizePx.X;
            m_destRect.Height = (int)sizePx.Y;
            batch.Draw(m_currentTexture,
                m_destRect,
                null,                               // src rect
                m_tone,                             // color
                rotation,                           // rotation
                new Vector2(m_currentTexture.Width / 2 - disp(texRelative(fixtureOffset.X, sizePx, true)),
                    this.m_currentTexture.Height / 2 - disp(texRelative(fixtureOffset.Y, sizePx, false))), // origin of rotation
                SpriteEffects.None,                 // effects
                0.0f);
        }
        /// <summary>
        /// Draws the sprite of the UniqueBodyGameObject.
        /// Rules will be applied in fonction of the state of the UniqueBodyGameObject.
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="body"></param>
        /// <param name="scrollPx"></param>
        /// <param name="fixtureOffset"></param>
        /// <param name="sizePx"></param>
        public void Draw(SpriteBatch batch, Body body, Vector2 scrollPx, Vector2 fixtureOffset, Vector2 sizePx)
        {
            Draw(batch, body.Position, body.Rotation, scrollPx, fixtureOffset, sizePx);
        }
        #endregion

        #region Utils
        /// <summary>
        /// Returns a texture relative coordinate from a body relative coordinate.
        /// Ex :     m_size.X   -> m_texture.Width
        ///    :     m_size.X/2 -> m_texture.Width/2
        /// </summary>  
        /// <param name="bodyRelative">body relative value to convert</param>
        /// <param name="horiz">true if the given data is width related</param>
        /// <returns></returns>
        protected float texRelative(float bodyRelative, Vector2 sizePx, bool horiz)
        {
            if (horiz)
                return bodyRelative * m_currentTextureSrcRect.Width / sizePx.X;
            else
                return bodyRelative * m_currentTextureSrcRect.Height / sizePx.Y;
        }
        protected float sim(double v) { return ConvertUnits.ToSimUnits(v); }
        protected Vector2 sim(Vector2 v) { return ConvertUnits.ToSimUnits(v); }
        protected float disp(float v) { return ConvertUnits.ToDisplayUnits(v); }
        protected Vector2 disp(Vector2 v) { return ConvertUnits.ToDisplayUnits(v); }
        #endregion
    }
}
