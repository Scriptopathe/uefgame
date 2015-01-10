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
    /// Abstract class for drawing event Sprite.
    /// </summary>
    public class EventSprite : GameEventSpriteBase
    {
        /* ----------------------------------------------------------------
         * Variables
         * --------------------------------------------------------------*/
        #region Variables
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
        /// Default behavior of the sprite : draws the full texture.
        /// </summary>
        /// <param name="eventState"></param>
        protected virtual void SetupSrcRect(GameEvent gameEvent)
        {
            m_currentTextureSrcRect.X = 0;
            m_currentTextureSrcRect.Y = 0;
            m_currentTextureSrcRect.Width = m_currentTexture.Width;
            m_currentTextureSrcRect.Height = m_currentTexture.Height;
        }
        /// <summary>
        /// Draws the sprite of the UniqueBodyGameObject.
        /// Rules will be applied in fonction of the state of the UniqueBodyGameObject.
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="realPosition">In simulation units</param>
        /// <param name="rotation"></param>
        /// <param name="scrollPx"></param>
        /// <param name="centerOffset"></param>
        /// <param name="shapeSize">in sim units</param>
        public void Draw(SpriteBatch batch,
            GameEvent evt,
            Vector2 realPosition, float rotation,
            Vector2 scrollPx, Vector2 centerOffset,
            Vector2 shapeSize, float layerDepth)
        {
            if (m_currentTexture == null)
                return;

            // Draws the texture
            SetupSrcRect(evt);

            m_destRect.X = (int)disp(realPosition.X) - (int)scrollPx.X;
            m_destRect.Y = (int)disp(realPosition.Y) - (int)scrollPx.Y;
            m_destRect.Width = (int)disp(shapeSize.X);
            m_destRect.Height = (int)disp(shapeSize.Y);

            var origin = new Vector2(m_currentTexture.Width / 2 - disp(texRelative(centerOffset.X, shapeSize, true)),
                    this.m_currentTexture.Height / 2 - disp(texRelative(centerOffset.Y, shapeSize, false)));
            //origin = new Vector2(Globals.Rand.Next(32), Globals.Rand.Next(32));
            batch.Draw(m_currentTexture,
                m_destRect,
                null,                               // src rect
                m_tone,                             // color
                rotation,                           // rotation
                origin,                             // origin of rotation
                SpriteEffects.None,                 // effects
                layerDepth);
        }
        /// <summary>
        /// Draws the sprite of a GameEvent given information about it.
        /// Rules will be applied in fonction of the state of the UniqueBodyGameObject.
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="body"></param>
        /// <param name="scrollPx"></param>
        /// <param name="centerOffset"></param>
        /// <param name="shapeSize"></param>
        void Draw(SpriteBatch batch, GameEvent evt, Body body, Vector2 scrollPx, Vector2 centerOffset, Vector2 shapeSize, float layerDepth)
        {
            Draw(batch, evt, body.Position, body.Rotation, scrollPx, centerOffset, shapeSize, layerDepth);
        }
        /// <summary>
        /// Draws the sprite of the given GameEvent.
        /// Rules will be applied in fonction of the state of the UniqueBodyGameObject.
        /// </summary>
        public override void Draw(SpriteBatch batch, GameEvent evt, Vector2 scrollPx, float layerDepth)
        {
            Draw(batch, evt, evt.Body.Position, evt.Body.Rotation, scrollPx, evt.MPhysicalObject.CenterOffset, evt.MPhysicalObject.ShapeSizeSim, layerDepth);
        }
        /// <summary>
        /// Draws the sprite of the given GameEvent.
        /// Rules will be applied in fonction of the state of the UniqueBodyGameObject.
        /// </summary>
        public override void DrawInEditor(SpriteBatch batch, GameEvent evt, Vector2 scrollPx, float layerDepth, Editor.GameObjectRenderOptions ops)
        {
            Draw(batch, evt, evt.Modules.Base.SimPosition*ops.Zoom, evt.MPhysicalObject.Rotation, scrollPx, evt.MPhysicalObject.CenterOffset, evt.MPhysicalObject.ShapeSizeSim*ops.Zoom, layerDepth);
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
        protected float texRelative(float bodyRelative, Vector2 shapeSize, bool horiz)
        {
            if (horiz)
                return bodyRelative * m_currentTextureSrcRect.Width / disp(shapeSize.X);
            else
                return bodyRelative * m_currentTextureSrcRect.Height / disp(shapeSize.Y);
        }
        #endregion
    }
}
