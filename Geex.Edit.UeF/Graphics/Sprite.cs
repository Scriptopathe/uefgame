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
using Bitmap = Geex.Edit.Common.Tools.Graphics.Bitmap;
namespace Geex.Edit.UeF.Graphics
{
    /// <summary>
    /// Represents a Sprite bound to a GraphicsManager object.
    /// </summary>
    public class Sprite : IDisposable
    {
        /* -------------------------------------------------------------
         * STATIC VARIABLES
         * -----------------------------------------------------------*/
        #region STATIC VARIABLES
        /// <summary>Pre-allocated instance of point</summary>
        protected static Vector2 s_buffPoint = new Vector2();
        /// <summary> Instance of rectangle that is used as a temporary rect in order not to allocate one</summary>
        protected static Rectangle s_buffRect = new Rectangle();
        protected static Rectangle BuffRect(int x, int y, int w, int h)
        {
            s_buffRect.X = x;
            s_buffRect.Y = y;
            s_buffRect.Width = w;
            s_buffRect.Height = h;
            return s_buffRect;
        }
        #endregion
        /* -------------------------------------------------------------
         * Variables
         * -----------------------------------------------------------*/
        #region Variables
        protected Vector2 m_position = new Vector2(0, 0);
        protected Vector2 m_origin = new Vector2(0, 0);
        protected Vector2 m_scale = new Vector2(1, 1);
        protected float m_floatZ;
        protected bool m_disposed;
        protected GraphicsManager m_gfxManager;
        #endregion
        /* -------------------------------------------------------------
         * Properties
         * -----------------------------------------------------------*/
        #region Properties
        /// <summary>
        /// Gets or sets the x-position of the sprite
        /// </summary>
        public virtual float X { get { return m_position.X; } set { m_position.X = value; } }
        /// <summary>
        /// Gets or sets the y-position of the sprite
        /// </summary>
        public virtual float Y { get { return m_position.Y; } set { m_position.Y = value; } }
        public virtual float Ox { get { return m_origin.X; } set { m_origin.X = value; } }
        public virtual float Oy { get { return m_origin.Y; } set { m_origin.Y = value; } }
        public virtual float ZoomX { get { return m_scale.X; } set { m_scale.X = value; } }
        public virtual float ZoomY { get { return m_scale.Y; } set { m_scale.Y = value; } }
        /// <summary>
        /// Gets or Set the Z-index of the sprite. The maximum value is 1000
        /// </summary>
        public int Z { get { return (int)(m_floatZ * 1000); } set { m_floatZ = (float)value / 1000.0f; } }
        /// <summary>
        /// Angle in radians
        /// </summary>
        public float Angle { get; set; }
        public Rectangle SrcRect { get; set; }
        public Bitmap Bitmap { get; set; }
        public Color Tone { get; set; }
        public bool IsDisposed { get { return m_disposed; }}
        #endregion
        /* -------------------------------------------------------------
         * Methods
         * -----------------------------------------------------------*/
        #region Methods
        /// <summary>
        /// Constructor.
        /// The Sprite will be bound to the given GraphicsManager object.
        /// </summary>
        public Sprite(GraphicsManager gfxManager)
        {
            m_gfxManager = gfxManager;
            gfxManager.RegisterSprite(this);
            Angle = 0.0f;
            SrcRect = Graphics.GraphicsManager.NullRectangle;
            Tone = Color.White;
        }
        /// <summary>
        /// Draws this sprite.
        /// </summary>
        public virtual void Draw(SpriteBatch batch)
        {
            if(Bitmap != null)
            {
                if (SrcRect == Graphics.GraphicsManager.NullRectangle)
                    SrcRect = BuffRect(0, 0, Bitmap.Texture.Width, Bitmap.Texture.Height);

                // Set the position of the buffPoint to the display coordinate of the sprite.
                s_buffPoint.X = m_gfxManager.Offset.X + m_position.X;
                s_buffPoint.Y = m_gfxManager.Offset.Y + m_position.Y;

                // Rectangle containing the sprite
                Rectangle spriteRect = BuffRect((int)s_buffPoint.X, (int)s_buffPoint.Y, (int)(SrcRect.Width * m_scale.X), (int)(SrcRect.Height * m_scale.Y));
                
                // If the sprite is on the screen, then draw it !
                if (m_gfxManager.ClipRect.Intersects(spriteRect))
                {
                    batch.Draw(Bitmap.Texture, s_buffPoint, SrcRect, Tone, Angle, m_origin, m_scale, SpriteEffects.None, m_floatZ);
                }
            }
        }
        /// <summary>
        /// Disposes the sprite. Doesn't dispose the bitmap !!
        /// </summary>
        public void Dispose()
        {
            m_gfxManager.UnregisterSprite(this);
            m_disposed = true;
        }
        #endregion
    }
}
