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
        /// When used in the editor, assetname must provide the filename extension
        /// of the asset.
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
                    string contentDir = Globals.EditorContentRootDirectory;
                    string fullName = contentDir + "/" + assetname;
                    if (System.IO.File.Exists(fullName))
                    {
                        cache[assetname] = UeFGame.Tools.Serializer.DeserializeNoConstraints<T>(fullName);
                    }
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
    /// Can be used in the editor and in the game.
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
            if (assetname == null)
                return null;
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
                    if (bmp == null)
                    {
                        cache[assetname] = null;
                        return null;
                    }
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
        public static void FreeAndDisposeAll()
        {
            foreach (Texture2D tex in cache.Values)
            {
                if (tex != null && !tex.IsDisposed)
                    tex.Dispose();
            }
            cache.Clear();
        }
    }
}
