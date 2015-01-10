using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using SystemBitmap = System.Drawing.Bitmap;
namespace Geex.Edit.Common.Tools.Graphics
{
    /// <summary>
    /// Cache for windows form bitmaps.
    /// </summary>
    public class SystemBitmapCache
    {
        const int BITMAP_CACHE_LIMIT = 50;
        /// <summary>
        /// Contains the cached bitmaps.
        /// To get one : CachedBitmaps[path][zoom][tileId][transparent]
        /// </summary>
        static Dictionary<string, SystemBitmap> CachedBitmaps = new Dictionary<string, SystemBitmap>();

        /// <summary>
        /// Returns the SystemBitmap with the given fullpath
        /// Returns null if it does not exist.
        /// </summary>
        /// <returns>SystemBitmap</returns>
        public static SystemBitmap CachedBitmap(string fullpath)
        {
            if (fullpath == "ok.png")
                fullpath = "ok.png";
            //return new SystemBitmap(32, 32);
            if (CachedBitmaps.ContainsKey(fullpath))
            {
                return CachedBitmaps[fullpath];
            }
            if (!System.IO.File.Exists(fullpath))
                return null;
            FileStream stream = null;
            try
            {
                stream = System.IO.File.Open(fullpath, FileMode.Open);
                CachedBitmaps[fullpath] = new SystemBitmap(stream);
            }
            finally
            {
                if(stream != null)
                    stream.Close();
            }
            // Deletes the oldest cached SystemBitmap
            if (CachedBitmaps.Count > BITMAP_CACHE_LIMIT)
            {
                FreeCachedBitmap(CachedBitmaps.Keys.First());
            }
            return CachedBitmaps[fullpath];
        }
        /// <summary>
        /// Returns the SystemBitmap with the given fullpath
        /// If no SystemBitmap found, returns a empty SystemBitmap of the given size.
        /// </summary>
        /// <returns>SystemBitmap</returns>
        public static SystemBitmap CachedBitmap(string fullpath, int width, int height)
        {
            try
            {
                SystemBitmap bmp = CachedBitmap(fullpath);
                if(bmp == null)
                    return new SystemBitmap(width, height);
                return bmp;
            }
            catch
            {
                return new SystemBitmap(width, height);
            }
        }
        /// <summary>
        /// Free the cached SystemBitmap with the given path
        /// </summary>
        /// <param name="fullpath"></param>
        public static void FreeCachedBitmap(string bitmapName)
        {
            CachedBitmaps[bitmapName].Dispose();
            CachedBitmaps.Remove(bitmapName);
        }
    }
}
