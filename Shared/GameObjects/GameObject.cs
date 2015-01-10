using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Collision;
using System.Xml.Serialization;
using UeFGame.GameObjects;
using UeFGame.GameComponents;
using UeFGame.Editor;

namespace UeFGame.GameObjects
{


    /// <summary>
    /// Base class for all the Game objects.
    /// </summary>
    [XmlInclude(typeof(GameObject))]
    public abstract class GameObject : MarshalByRefObject, IDisposable
    {
        #region Events
        public delegate void ScriptingUpdateEventHandler(GameObject obj, GameTime time);

        #endregion
        /* ---------------------------------------------------------------
         * Properties
         * -------------------------------------------------------------*/
        #region Properties
        /// <summary>
        /// Id of the map containing the object.
        /// </summary>
        public int MapId;
        /// <summary>
        /// Object providing the information for the object to initialize itself.
        /// </summary>
        public virtual ModuleSet Modules
        {
            get { return InitializingData.ModuleSet; }
            protected set
            {
                InitializingData.ModuleSet = value;
            }
        }
        /// <summary>
        /// Value indicating wheither or not the object is disposed, i.e. when
        /// it has disapeared from the Game and freed all the ressources it was
        /// handling.
        /// </summary>
        public virtual bool IsDisposed { get; set; }
        /// <summary>
        /// Value indicating wheither or not this event should be updated (the 
        /// Update() method called) in the Map update process.
        /// </summary>
        public virtual bool IsActive { get; set; }
        /// <summary>
        /// Value indicating wheither or not this event should be drawn on the
        /// screen.
        /// </summary>
        public virtual bool IsVisible { get; set; }
        /// <summary>
        /// Get the object's position in simulation units.
        /// </summary>
        public virtual float SimX { get; set; }
        /// <summary>
        /// Get the object's position in simulation units.
        /// </summary>
        public virtual float SimY { get; set; }
        /// <summary>
        /// Gets the BoundingBox of the GameObject.
        /// This is mostly used in the editor.
        /// </summary>
        public abstract AABB BoundingBox { get; }
        /// <summary>
        /// Base module.
        /// </summary>
        public BaseModule MBase
        {
            get;
            protected set;
        }

        /// <summary>
        /// Copy of the initializing data of this GameObject.
        /// </summary>
        public GameObjectInit InitializingData
        {
            get;
            set;
        }
        #endregion
        /* ---------------------------------------------------------------
         * Basic methods
         * -------------------------------------------------------------*/
        #region Basic Methods
        /// <summary>
        /// Creates a new non initialized Game Object.
        /// </summary>
        public GameObject()
        {

        }
        /// <summary>
        /// Initializes this object using the provided Modules.
        /// </summary>
        public virtual void Initialize()
        {
#if DEBUG
            if (InitializingData == null)
            {
                throw new Exception("Un ModuleSet doit être assigné à l'objet avant de pouvoir l'initialiser.");
            }
#endif

            MBase = (BaseModule)Modules["base"];
        }
        /// <summary>
        /// This method is called once per frame (if IsActive is set to true)
        /// and is the place where the Game Object's logic should be updated.
        /// </summary>
        /// <param name="time"></param>
        public abstract void Update(GameTime time);
        /// <summary>
        /// This method is called once per frame (if IsVisible is set to true)
        /// and is the place to draw the Game object.
        /// </summary>
        /// <param name="time">The current gametime</param>
        /// <param name="batch">The batch used to draw the graphics</param>
        /// <param name="scroll">Current scrolling of the map in pixels</param>
        public abstract void Draw(GameTime time, SpriteBatch batch, Vector2 scroll);
        /// <summary>
        /// Disposes the GameObject.
        /// </summary>
        public virtual void Dispose()
        {
            IsDisposed = true;
            IsActive = false;
        }
        #endregion
        /* ---------------------------------------------------------------
         * Editor
         * -------------------------------------------------------------*/
        #region Editor
        public delegate void EditorActionDelegate();
        /// <summary>
        /// Draws the object in the editor.
        /// </summary>
        /// <param name="batch">batch where to draw the object</param>
        /// <param name="scroll">scrolling of the object in pixels</param>
        /// <param name="ops">RenderOptions</param>
        public abstract void DrawInEditor(SpriteBatch batch, Vector2 scroll, Editor.GameObjectRenderOptions ops);
        /// <summary>
        /// Action performed when the object is selected an a key is pressed.
        /// Returns true if the key is captured, false otherwise.
        /// </summary>
        /// <param name="simPos">Position of the mouse in sim units (zoomed)</param>
        /// <param name="zoom">Zoom of the editor</param>
        /// <param name="modifiedProperty">The property modified during this method.</param>
        /// <param name="keys">Keys pressed.</param>
        /// <param name="newValue">The new value of the modified property.</param>
        /// <param name="oldValue">The old value of the modified property.</param>
        /// <returns>True if the stroke was captured.</returns>
        public abstract bool OnKeyDownInEditor(Vector2 simPos, out EditorActionDelegate undoAction, out EditorActionDelegate redoAction, System.Windows.Forms.Keys keys);
        public enum ActionType
        {
            None,
            Drag,
            NoDrag
        }
        /// <summary>
        /// Action performed when the object is clicked in editor.
        /// </summary>
        /// <param name="simPos">Mouse position in simulation units. Already "zoomed"</param>
        /// <param name="redoAction">Delegate which redoes the action performed by this call.</param>
        /// <param name="undoAction">Delegate which undoes the action performed by this call.</param>
        /// <returns>True if the object needs to receive OnMouseMoveEditor and OnMouseUpInEditor calls for a drag operation.</returns>
        public abstract ActionType OnMouseDownInEditor(Vector2 simPos, out EditorActionDelegate undoAction, out EditorActionDelegate redoAction);
        /// <summary>
        /// Action performed when a point is dragging in editor.
        /// </summary>
        /// <param name="simPos">Current mouse position in Sim units</param>
        /// <param name="delta">Delta of the mouse position at last frames</param>
        public abstract void OnMouseMoveEditor(Vector2 simPos, Vector2 delta);
        /// <summary>
        /// Action performed when the object's gradding is 
        /// </summary>
        /// <param name="simPos">Mouse position in simulation units. Already "zoomed"</param>
        /// <param name="redoAction">Delegate which redoes the action performed by this call.</param>
        public abstract void OnMouseUpInEditor(Vector2 simPos, out EditorActionDelegate redoAction);
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
