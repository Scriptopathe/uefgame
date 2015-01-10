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
    /// Class handling the cursor on the map.
    /// </summary>
    public class Cursor : Sprite
    {
        #region Properties
        protected int m_width;
        protected int m_height;
        /// <summary>
        /// Width of the cursor (in tiles)
        /// </summary>
        public int Width
        {
            get { return m_width/Project.GameOptions.TileSize; }
            set { m_width = value * Project.GameOptions.TileSize; }
        }
        /// <summary>
        /// Height of the cursor (in tiles)
        /// </summary>
        public int Height
        {
            get { return m_height / Project.GameOptions.TileSize; }
            set { m_height = value * Project.GameOptions.TileSize; }
        }
        /// <summary>
        /// Gets or sets the position in tiles of the cursor.
        /// </summary>
        public override float X
        {
            get { return (m_position.X / Project.GameOptions.TileSize); }
            set { m_position.X = value * Project.GameOptions.TileSize; }
        }
        /// <summary>
        /// Gets or sets the position in tiles of the cursor.
        /// </summary>
        public override float Y
        {
            get { return (m_position.Y / Project.GameOptions.TileSize); }
            set { m_position.Y = value * Project.GameOptions.TileSize; }
        }
        #endregion
        /// <summary>
        /// Constructor of the cursor.
        /// </summary>
        public Cursor(GraphicsManager gfxManager)
            : base(gfxManager)
        {
            this.Bitmap = Bitmap.Cached(Common.AppRessources.RessourceDir()+"\\cursor.png");
            m_floatZ = 1.0f;
        }
        /// <summary>
        /// Draws this sprite.
        /// </summary>
        public override void Draw(SpriteBatch batch)
        {
            if(UeFGlobals.MapView.GraphicsManager.RenderOptions.IsCursorVisible != true)
                return;
            // If there is no bitmap
            if (this.Bitmap == null)
                return;
            // Setting up position
            // Here we asume that the controler is a "Rpg" one.
            var controler =UeFGlobals.Controler;
            this.Width = controler.CursorRect.Width;
            this.Height = controler.CursorRect.Height;
            this.X = controler.CursorRect.X;
            this.Y = controler.CursorRect.Y;

            float zoom = UeFGlobals.MapView.GraphicsManager.RenderOptions.Zoom;
            m_scale.X = zoom; m_scale.Y = zoom;
            if (Bitmap != null)
            {
                // Set the position of the buffPoint to the display coordinate of the sprite.
                s_buffPoint.X = m_gfxManager.Offset.X + m_position.X * zoom;
                s_buffPoint.Y = m_gfxManager.Offset.Y + m_position.Y * zoom;

                // Rectangle containing the sprite
                Rectangle spriteRect = BuffRect((int)s_buffPoint.X, (int)s_buffPoint.Y, (int)(m_width / m_scale.X), (int)(m_height / m_scale.Y));
                Rectangle src = new Rectangle(0, 0, 0, 0);
                float tex_w = (float)Bitmap.Texture.Width;
                float tex_h = (float)Bitmap.Texture.Height;
                float curs_w = m_width * m_scale.X;
                float curs_h = m_height * m_scale.Y;
                Vector2 drawPt = new Vector2();
                Vector2 scale = new Vector2();
                // If the sprite is on the screen, then draw it !
                if (m_gfxManager.ClipRect.Intersects(spriteRect))
                {
                    src.Y = 0;
                    drawPt.Y = s_buffPoint.Y;
                    for (int i = 0; i < 2; i++)
                    {
                        // A modifier : src, s_buffPoint, m_scale
                        // left corner :
                        src.X = 0; 
                        src.Width = (int)(tex_w / 4);
                        src.Height = (int)(tex_h / 4);
                        scale.X = 1.0f;
                        scale.Y = 1.0f;
                        drawPt.X = s_buffPoint.X; 
                        batch.Draw(Bitmap.Texture, drawPt, src, Tone, Angle, m_origin, scale, SpriteEffects.None, m_floatZ);
                        // Middle
                        src.X += src.Width;
                        drawPt.X += src.Width;
                        src.Width = (int)(tex_w / 2);
                        // Total width of the top border (less the edges, i.e 2*w/4) / by the width of the texture top (w/2). 
                        scale.X = (curs_w - (tex_w / 2.0f)) / (tex_w / 2.0f);
                        scale.Y = 1f;
                        batch.Draw(Bitmap.Texture, drawPt, src, Tone, Angle, m_origin, scale, SpriteEffects.None, m_floatZ);
                        // right corner
                        src.X += src.Width;
                        drawPt.X += src.Width * scale.X;
                        src.Width /= 2;
                        scale.X = 1f; scale.Y = 1f;
                        batch.Draw(Bitmap.Texture, drawPt, src, Tone, Angle, m_origin, scale, SpriteEffects.None, m_floatZ);

                        // Prepare y for second loop
                        src.Y = (int)(tex_w - tex_w / 4);
                        drawPt.Y += curs_h - tex_h / 4;
                    }
                    // Left middle
                    src.Y = (int)(tex_w / 4);
                    src.X = 0;
                    src.Width = (int)(tex_w / 4);
                    src.Height = (int)(tex_h / 2);
                    scale.X = 1.0f;
                    scale.Y = ((float)curs_h - (tex_h / 2)) / (tex_h / 2);
                    drawPt.X = s_buffPoint.X;
                    drawPt.Y = s_buffPoint.Y + (tex_h / 4);
                    batch.Draw(Bitmap.Texture, drawPt, src, Tone, Angle, m_origin, scale, SpriteEffects.None, m_floatZ);
                    src.X += (int)(3 * tex_w / 4);
                    drawPt.X += curs_w - tex_w / 4;
                    batch.Draw(Bitmap.Texture, drawPt, src, Tone, Angle, m_origin, scale, SpriteEffects.None, m_floatZ);
                }
            }
        }
    }
}
