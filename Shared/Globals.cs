using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
namespace UeFGame
{
    /// <summary>
    /// Class containing the globals.
    /// </summary>
    public static class Globals
    {
        public static ContentManager Content;
        /// <summary>
        /// TODO INITIALIZE IT IN THE MAP
        /// </summary>
        public static List<GameObjects.GameObject> GameObjectPool;
        /// <summary>
        /// The screen width of the game in pixels.
        /// </summary>
        public static int ScreenWidth;
        /// <summary>
        /// The screen height of the game in pixels.
        /// </summary>
        public static int ScreenHeight;
        /// <summary>
        /// The word in pixels.
        /// </summary>
        public static World World;
        /// <summary>
        /// Random
        /// </summary>
        public static Random Rand = new Random();
        /// <summary>
        /// The Game Map.
        /// </summary>
        public static GameComponents.GameMap GameMap;
        /// <summary>
        /// Game configuration.
        /// </summary>
        public static Config Cfg;
        #region Editor
        /// <summary>
        /// Tells some object to adapt their behavior if we are in the
        /// editor.
        /// It will changes things such as loading of textures etc...
        /// </summary>
        public static bool ExecuteInEditor = false;
        /// <summary>
        /// If in editor mode, indicates the path to the content root directory,
        /// NOT followed by a backslash.
        /// </summary>
        public static string EditorContentRootDirectory;
        /// <summary>
        /// Graphics device of the editor.
        /// Mainly used to load textures within it.
        /// </summary>
        public static GraphicsDevice EditorGraphicsDevice;
        #endregion
    }
}
