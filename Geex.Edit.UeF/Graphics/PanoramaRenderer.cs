using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Bitmap = Geex.Edit.Common.Tools.Graphics.Bitmap;
namespace Geex.Edit.UeF.Graphics
{
    public class PanoramaRenderer
    {
        /// <summary>
        /// Returns the client rectangle of the main form
        /// </summary>
        System.Drawing.Rectangle ClientRectangle
        {
            get
            {
                return Common.Globals.MainForm.ClientRectangle;
            }
        }
        /// <summary>
        /// Constructor of the panorama renderer
        /// </summary>
        public PanoramaRenderer()
        {

        }
        /// <summary>
        /// Draws the panoramas
        /// </summary>
        public void DrawPanoramas(SpriteBatch batch, Bitmap[] panoramas, Vector2[] speeds)
        {
            float scaling = 1.0f / (float)UeFGlobals.MapView.GraphicsManager.RenderOptions.Zoom;
            System.Drawing.Rectangle clipRect = ClientRectangle;
            // These are used to position the sprite at the good pixel
            int scrollX = -UeFGlobals.MapView.GraphicsManager.GetScrolling().X;
            int scrollY = -UeFGlobals.MapView.GraphicsManager.GetScrolling().Y;
            
            for (int i = 0; i < panoramas.Length; i++)
            {
                Bitmap pano = panoramas[i];

                // If the file does not exist then the Bitmap is null at this point.
                if (pano == null)
                    continue;
                Vector2 speed = speeds[i];
                Rectangle bmpRect = new Rectangle(0, 0, (int)(pano.Texture.Width * scaling), (int)(pano.Texture.Height * scaling));
                // Start position of the first
                int firstX = (int)((speed.X * scrollX) % bmpRect.Width);
                int firstY = (int)((speed.Y * scrollY) % bmpRect.Height);
                // Now we have to fill the panorama on the left and right
                for(int x = 0; x < (clipRect.Width - firstX) / bmpRect.Width + 1; x++)
                {
                    for (int y = 0; y < (clipRect.Height - firstY) / bmpRect.Height + 1; y++)
                    {
                        batch.Draw(pano.Texture,
                            new Vector2(clipRect.X + firstX + x * bmpRect.Width, clipRect.Y + firstY + y * bmpRect.Height), // Position
                            pano.Texture.Bounds,        // Source Rectangle
                            new Color(255, 255, 255),
                            0.0f,
                            new Vector2(0f, 0f),
                            scaling,
                            SpriteEffects.None,
                            0.0f);
                    }
                }
            }
        }
    }
}
