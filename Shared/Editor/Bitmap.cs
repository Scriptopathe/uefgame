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
using System.IO;
namespace UeFGame.Editor
{
    /// <summary>
    /// Class handling bitmaps
    /// </summary>
    public class Bitmap : IDisposable
    {
        /// <summary>
        /// Texture of the bitmap.
        /// </summary>
        private Texture2D _TEXTURE_;
        private bool m_isDisposed = false;
        public Texture2D Texture { get { return _TEXTURE_; } }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="filename">Filename : either full or starting from the root.</param>
        protected Bitmap(string filename, GraphicsDevice device)
        {
            FileStream stream = null;
            if (File.Exists(filename))
            {
                try
                {
                    stream = System.IO.File.Open(filename, FileMode.Open);
                    _TEXTURE_ = Texture2D.FromStream(device, stream);
                    stream.Close();
                }
                catch (Exception e)
                {
                    m_isDisposed = true;
                }
                finally
                {
                    if (stream != null)
                        stream.Close();
                }
            }
            else
            {
                m_isDisposed = true;
            }
        }
        /// <summary>
        /// Creates a new bitmap associated with the given device.
        /// Returns null if it cannot be loaded.
        /// </summary>
        /// <param name="filename">The filename of the texture. Full path or starting from the root.</param>
        /// <param name="device">The device which will be associated with the texture.</param>
        public static Bitmap NotCachedBitmap(string filename, GraphicsDevice device)
        {
            Bitmap bmp = new Bitmap(filename, device);
            if (bmp.m_isDisposed)
                return null;
            return bmp;
        }
        /// <summary>
        /// Disposes the allocated ressources.
        /// </summary>
        public void Dispose()
        {
            if (m_isDisposed)
                return;
            m_isDisposed = true;
            Texture.Dispose();
        }
    }
}
