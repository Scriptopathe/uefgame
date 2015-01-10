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
namespace UeFGame
{
    public static class RessourceCache<T>
    {
        const int maxCache = 20; 
        static Dictionary<string, T> cache = new Dictionary<string, T>(256);
        /// <summary>
        /// Returns a cached asset with the given type.
        /// </summary>
        /// <param name="assetname">name of the asset to load</param>
        /// <returns></returns>
        public static T Cached(string assetname)
        {
            if (!cache.ContainsKey(assetname))
            {
                if (cache.Count >= maxCache)
                    cache.Remove(cache.First().Key);
#if DEBUG
                if (Globals.ExecuteInEditor)
                {
                    throw new Exception("impossible de loader ça");
                }
                else
                {
                    cache[assetname] = Globals.Content.Load<T>(assetname);
                }
#else
                cache[assetname] = Globals.Content.Load<T>(assetname);
#endif
            }
            return cache[assetname];
        }
    }
    /// <summary>
    /// Special ressource cache for textures.
    /// </summary>
    public static class TextureRessourceCache
    {
        const int maxCache = 20;
        static Dictionary<string, Texture2D> cache = new Dictionary<string, Texture2D>(256);
        /// <summary>
        /// Returns a cached asset with the given type.
        /// </summary>
        /// <param name="assetname">name of the asset to load</param>
        /// <returns></returns>
        public static Texture2D Cached(string assetname)
        {
            if (!cache.ContainsKey(assetname))
            {
                if (cache.Count >= maxCache)
                    cache.Remove(cache.First().Key);
#if DEBUG
                if (Globals.ExecuteInEditor)
                {
                    Editor.Bitmap bmp = Editor.Bitmap.NotCachedBitmap(
                        Globals.EditorContentRootDirectory + "\\" + assetname + ".png",
                        Globals.EditorGraphicsDevice);
                    cache[assetname] = bmp.Texture;
                }
                else
                {
                    cache[assetname] = Globals.Content.Load<Texture2D>(assetname);
                }
#else
                cache[assetname] = Globals.Content.Load<T>(assetname);
#endif
            }
            return cache[assetname];
        }
    }
}
