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
    /// The Game object class.
    /// This is a special class as it may be used in the editor as well as in the Game.
    /// </summary>
    public abstract class GameObject : MarshalByRefObject
    {
        #region Properties
        /// <summary>
        /// Gets the real position in simulation unit.
        /// This should return the same thing as the starting position when it has not been moved.
        /// </summary>
        public abstract Vector2 SimPosition
        {
            get;
            set;
        }
        /// <summary>
        /// Gets the AABB of the object in simulation units.
        /// </summary>
        public abstract AABB BoundingBox { get; }
        /// <summary>
        /// Returns true if the object is disposed.
        /// </summary>
        public abstract bool IsDisposed { get; protected set; }
        /// <summary>
        /// Returns true if the object is activated
        /// </summary>
        public abstract bool IsActivated { get; protected set; }
        /// <summary>
        /// Gets the object initializing data.
        /// </summary>
        public abstract GameObjectInitializingData InitializingData { get; protected set; }
        #endregion
        /* ---------------------------------------------------------------
         * Basics
         * -------------------------------------------------------------*/
        #region Basics
        /// <summary>
        /// Disposes the GameObject and frees all the allocates ressources.
        /// </summary>
        public abstract void Dispose();
        /// <summary>
        /// Updates the Game Object.
        /// </summary>
        /// <param name="gameTime">the gametime object given by the Xna framework</param>
        public abstract void Update(GameTime gameTime);
        /// <summary>
        /// Draws the object on the map
        /// </summary>
        /// <param name="batch">the spritebatch where to draw the object</param>
        /// <param name="scroll">the scrolling value in pixels</param>
        public abstract void Draw(SpriteBatch batch, Vector2 scroll);
        /// <summary>
        /// Deactivate this instance of GameObject.
        /// This also puts it in the pool.
        /// </summary>
        public abstract void Deactivate();
        /// <summary>
        /// Initializes the event from a GameObjectInitializingData object.
        /// </summary>
        /// <param name="data">the object used to initialize the event</param>
        public abstract void InitializeFromData(GameObjectInitializingData data, World world);
        #endregion
        /* ---------------------------------------------------------------
         * Editor-related
         * -------------------------------------------------------------*/
        #region Editor-related
#if DEBUG
        /// <summary>
        /// Initialize the object in the editor from the GameObjectInitializingData.
        /// </summary>
        /// <param name="data"></param>
        public abstract void InitializeFromDataInEditor(GameObjectInitializingData data);
        /// <summary>
        /// Updates the object's state when it's initializing data changed.
        /// </summary>
        public abstract void UpdateFromInitializingDataInEditor();
        /// <summary>
        /// Dispose the object (editor only)
        /// </summary>
        public abstract void DisposeInEditor();
        /// <summary>
        /// Draws the object in the editor.
        /// </summary>
        /// <param name="batch">batch where to draw the object</param>
        /// <param name="scroll">scrolling of the object</param>
        /// <param name="ops">RenderOptions</param>
        public abstract void DrawInEditor(SpriteBatch batch, Vector2 scroll, Editor.GameObjectRenderOptions ops);
        /// <summary>
        /// Gets the type of the initializing data
        /// </summary>
        public abstract Type GetInitializingDataType();
        /// <summary>
        /// Returns a list containing the names of the registrable events.
        /// Used to perform code editing of the event callbacks.
        /// </summary>
        /// <returns></returns>
        public abstract List<string> GetRegistrableEvents();
#endif
        #endregion
        /* ---------------------------------------------------------------
         * Utils
         * -------------------------------------------------------------*/
        #region Utils
        /// <summary>
        /// Returns a texture relative coordinate from a body relative coordinate.
        /// Ex :     m_size.X   -> m_texture.Width
        ///    :     m_size.X/2 -> m_texture.Width/2
        /// </summary>  
        /// <param name="bodyRelative">body relative value to convert</param>
        /// <param name="horiz">true if the given data is width related</param>
        /// <returns></returns>
        protected float texRelative(float bodyRelative, bool horiz, Vector2 size, Texture2D tex)
        {
            if (horiz)
                return bodyRelative * tex.Width / size.X;
            else
                return bodyRelative * tex.Height / size.Y;
        }
        protected float sim(double v) { return ConvertUnits.ToSimUnits(v); }
        protected Vector2 sim(Vector2 v) { return ConvertUnits.ToSimUnits(v); }
        protected float disp(float v) { return ConvertUnits.ToDisplayUnits(v); }
        protected Vector2 disp(Vector2 v) { return ConvertUnits.ToDisplayUnits(v); }
        #endregion
    }
}
