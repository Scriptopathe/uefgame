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
namespace Geex.Edit.Common.Tools.Graphics
{
    /// <summary>
    /// Class used to load textures, and cache them.
    /// </summary>
    public class Bitmap : IDisposable
    {
        /// <summary>
        /// Texture of the bitmap.
        /// </summary>
        private Texture2D _TEXTURE_;
        /// <summary>
        /// Indicates if the bitmap is disposed.
        /// </summary>
        private bool m_isDisposed = false;
        /// <summary>
        /// Gets the texture of the bitmap.
        /// </summary>
        public Texture2D Texture { get { return _TEXTURE_; } }
        /// <summary>
        /// Creates a new bitmap from the given filename, bound to the
        /// given GraphicsDevice.
        /// If the bitmap is not found, the bitmap will flaged as disposed.
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
                catch
                {
                    m_isDisposed = true;
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
            if(m_isDisposed)
                return;
            m_isDisposed = true;
            Texture.Dispose();
        }

        #region CACHEING
        const int GRAPHICS_BITMAP_CACHE_LIMIT = 50;
        /// <summary>
        /// Contains the cached bitmaps.
        /// To get one : CachedBitmaps[path][zoom][tileId][transparent]
        /// </summary>
        static Dictionary<string, Graphics.Bitmap> CachedBitmaps = new Dictionary<string, Graphics.Bitmap>();
        /// <summary>
        /// Returns the Bitmap with the given fullpath.
        /// Returns null if it does not exist or cannot be loaded.
        /// </summary>
        /// <returns>Graphics.Bitmap</returns>
        public static Graphics.Bitmap Cached(string fullpath)
        {
            if (CachedBitmaps.ContainsKey(fullpath))
            {
                return CachedBitmaps[fullpath];
            }
            Bitmap bmp = new Graphics.Bitmap(fullpath, Globals.MapView.GraphicsDevice);
            

            if (bmp.m_isDisposed)
            {
                return null;
            }
            else
            {
                CachedBitmaps[fullpath] = bmp;
            }

            // Deletes the oldest cached Bitmap
            if (CachedBitmaps.Count > GRAPHICS_BITMAP_CACHE_LIMIT)
            {
                FreeCached(CachedBitmaps.Keys.First());
            }
            return CachedBitmaps[fullpath];
        }
        /// <summary>
        /// Free the cached bitmap with the given path
        /// </summary>
        /// <param name="fullpath"></param>
        public static void FreeCached(string bitmapName)
        {
            CachedBitmaps[bitmapName].Dispose();
            CachedBitmaps.Remove(bitmapName);
        }
        /// <summary>
        /// Frees all the cached bitmaps, and dispose them.
        /// </summary>
        public static void FreeAndDisposeAllCachedBitmaps()
        {
            while(CachedBitmaps.Count != 0)
            {
                KeyValuePair<string, Graphics.Bitmap> kvp = CachedBitmaps.First();
                FreeCached(kvp.Key);
                kvp.Value.Dispose();
            }
        }
        #endregion
    }
}
