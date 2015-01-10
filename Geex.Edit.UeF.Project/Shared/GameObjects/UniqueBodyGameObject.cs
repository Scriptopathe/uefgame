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
    /// Class that describes a body with a unique Body.
    /// It assumes there will only be one fixture in the body.
    /// However, no fixture is created.
    /// </summary>
    [Editor.EditableGameObject(typeof(UniqueBodyGameObjectInitializingData))]
    public class UniqueBodyGameObject : GameObject
    {
        /* ---------------------------------------------------------------
         * Variables
         * -------------------------------------------------------------*/
        #region Variables
        /// <summary>
        /// The Body of the Unique Body game object.
        /// </summary>
        protected Body m_body;
        /// <summary>
        /// The world where the body is located.
        /// </summary>
        protected World m_world;
        /// <summary>
        /// Size of the displayed texture in pixels.
        /// </summary>
        protected Vector2 m_sizePx;
        /// <summary>
        /// Offset of the fixture relative to the body.
        /// A fixture offset of 0 makes :
        ///     - rotation from the center
        ///     - Position return the center's position.
        /// A fixture offset of m_sizePx/2 makes :
        ///     - rotation from the upper left corner
        ///     - position return the upper left corner's position.
        /// </summary>
        protected Vector2 m_fixtureOffset;
        /// <summary>
        /// The sprite used for the rendering of the Object.
        /// </summary>
        protected UniqueBodyGameObjectSprite m_sprite;
        /// <summary>
        /// Layer depth of this object.
        /// </summary>
        protected float m_layerDepth;
        /// <summary>
        /// Angle offset calculated from the offset distance.
        /// </summary>
        protected double _angleOffset;
        /// <summary>
        /// Ray from the center of rotation to the center of the square.
        /// </summary>
        protected float _ray;
        #endregion
        /* ---------------------------------------------------------------
         * Properties
         * -------------------------------------------------------------*/
        #region Properties
        /// <summary>
        /// Gets the real position in simulation unit.
        /// This should return the same thing as the starting position when it has not been moved.
        /// </summary>
        public override Vector2 SimPosition
        {
            get { return m_body.Position; }
            set { m_body.Position = value; }
        }
        /// <summary>
        /// Gets the AABB of the object.
        /// </summary>
        public override AABB BoundingBox
        { 
            get {
                AABB aabb;
                m_body.FixtureList.First().GetAABB(out aabb, 0);
                return aabb;
            }
        }
        /// <summary>
        /// Returns true if the object is disposed.
        /// </summary>
        public override bool IsDisposed { get; protected set; }
        /// <summary>
        /// Returns true if the object is activated
        /// </summary>
        public override bool IsActivated { get; protected set; }
        /// <summary>
        /// Gets the object initializing data.
        /// </summary>
        public override GameObjectInitializingData InitializingData { get; protected set; }
        /// <summary>
        /// Returns the center of the shape in Simulation units.
        /// It uses Cos and Sin calculation, so don't abuse it !
        /// </summary>
        public virtual Vector2 ShapeCenterSimPosition
        {
            get
            {
                return new Vector2(m_body.Position.X - _ray * (float)Math.Cos(m_body.Rotation + _angleOffset),
                    m_body.Position.Y - _ray * (float)Math.Sin(m_body.Rotation + _angleOffset));
            }
        }
        /// <summary>
        /// Gets the body of this object.
        /// </summary>
        public Body Body { get { return m_body; } }
        /// <summary>
        /// Returns the upper right simulation position.
        /// </summary>
        public Vector2 UpperRightSimPosition
        {
            get { return Body.Position - sim(m_sizePx / 2.0f) + m_fixtureOffset; }
            set { Body.Position = sim(m_sizePx / 2.0f) - m_fixtureOffset + value; }
        }
        #endregion
        /* ---------------------------------------------------------------
         * Basics
         * -------------------------------------------------------------*/
        #region Basics
        #region Constructor
        /// <summary>
        /// Initializes the unique body game object.
        /// This is called when the object is created in the pool.
        /// </summary>
        /// <param name="m_world">The World. If null, the object will be created as if it was
        /// only used by the editor (no body creation)</param>
        public UniqueBodyGameObject(World world)
        {
            CTor(world);
        }
        /// <summary>
        /// Private constructor called by the overloads of the public constructor.
        /// </summary>
        /// <param name="world"></param>
        private void CTor(World world)
        {
            IsDisposed = false;
            IsActivated = false;

            // Creates the body, with no fixture.
            // If world == null, then we are in the editor.
            if (world != null)
            {
                m_body = BodyFactory.CreateBody(world);
                // Removes it from the world, as we don't want it to interact with the world while
                // it is not activated.
                m_body.Enabled = false;
            }

            // Instanciating some objects
            m_sizePx = new Vector2();
            m_fixtureOffset = new Vector2();
            m_sprite = new UniqueBodyGameObjectSprite();
        }
        #endregion
        /// <summary>
        /// Disposes the GameObject and frees all the allocated ressources.
        /// </summary>
        public override void Dispose()
        {
            IsDisposed = true;
            m_body.Dispose();
        }
        /// <summary>
        /// Updates the Game Object.
        /// </summary>
        /// <param name="gameTime">the gametime object given by the Xna framework</param>
        public override void Update(GameTime gameTime)
        {
            if (!IsActivated || IsDisposed)
                throw new InvalidOperationException("Do not call update on a deactivated or disposed object");
        }
        /// <summary>
        /// Draws the object on the map
        /// </summary>
        /// <param name="batch">the spritebatch where to draw the object</param>
        /// <param name="scrollPx">the scrolling value</param>
        public override void Draw(SpriteBatch batch, Vector2 scrollPx)
        {
            if (!IsActivated || IsDisposed)
                throw new InvalidOperationException("Do not call draw on a deactivated or disposed object");
            
            // Draws the texture
            if (!isOutOfScreen(scrollPx))
            {
                m_sprite.Draw(batch, m_body, scrollPx, m_fixtureOffset, m_sizePx);
            }
        }
        /// <summary>
        /// Returns true if the object is out of screen
        /// </summary>
        /// <returns></returns>
        protected bool isOutOfScreen(Vector2 scroll)
        {
            // Finds the rectangle where is displayed the 
            AABB aabb;
            this.m_body.FixtureList.First().GetAABB(out aabb, 0);

            // Size of the rectangle
            // The rectangle size is multiplied by 2 for more tolerance.
            Vector2 size = aabb.UpperBound - aabb.LowerBound;
            Vector2 topCorner = UpperRightSimPosition - size;
            size *= 2;
            
            Rectangle displayRectangle = new Rectangle((int)disp(topCorner.X) - (int)scroll.X, (int)disp(topCorner.Y) - (int)scroll.Y,
                (int)disp(size.X), (int)disp(size.Y));

            // Test
            if (displayRectangle.Right < 0 || displayRectangle.Bottom < 0 || displayRectangle.Left > Globals.ScreenWidth ||
                displayRectangle.Top > Globals.ScreenHeight)
                return true;

            return false;
        }
        /// <summary>
        /// Deactivate this instance of GameObject.
        /// Usually called from the pool.
        /// </summary>
        public override void Deactivate()
        {
            if (!IsActivated || IsDisposed)
                throw new InvalidOperationException("Object already deactivated or disposed");
            // Marks the object as deactivated
            IsActivated = false;
            // Removes the body from the world
            m_body.Enabled = false;
            // Removes the fixtures of this body
            m_body.FixtureList.Clear();
        }
        /// <summary>
        /// Initializes the event from a GameObjectInitializingData object.
        /// It attaches a fixture to the Body.
        /// </summary>
        /// <param name="data">the object used to initialize the event</param>
        public override void InitializeFromData(GameObjectInitializingData data1, World world)
        {
            if (IsDisposed)
                throw new InvalidOperationException("Disposed object");
            IsActivated = true;
            
            // The new world !!
            m_world = world;
            m_body.Enabled = true;

            // Here perform initialization from gameobject
            UniqueBodyGameObjectInitializingData data = (UniqueBodyGameObjectInitializingData)data1;
            InitializeCommonData(data);
            m_body.Rotation = data.Rotation;
            m_body.BodyType = data.BodyType;
            // Now initialize body and Fixture
            m_body.Position = new Vector2(data.SimStartX, data.SimStartY);
            switch (data.ShapeType)
            {
                case ShapeTypes.Rectangle:
                    FixtureFactory.AttachRectangle(sim(m_sizePx.X), sim(m_sizePx.Y), data.BodyDensity, m_fixtureOffset, m_body);
                    break;
                case ShapeTypes.Circle:
                    FixtureFactory.AttachEllipse(sim(m_sizePx.X / 2), sim(m_sizePx.Y / 2), 20, data.BodyDensity, m_body);
                    break;
                case ShapeTypes.Polygon:
                    FixtureFactory.AttachPolygon(data.ShapePolygonVertices, 1.0f, m_body);
                    break;
            }
            // Initialize values that must be initialized after shape creation
            m_body.Restitution = data.BodyRestitution;
            m_body.Friction = data.BodyFriction;
            m_body.LinearDamping = data.BodyLinearDamping;
            m_body.CollisionCategories = data.CollisionCategories;
            // Precomputes some values
            _ray = _calculateRay();
            _angleOffset = _calculateAngleOffset();
        }

        #endregion
        /* ---------------------------------------------------------------
         * Common Game / Editor
         * -------------------------------------------------------------*/
        #region Common Game / Editor
        /// <summary>
        /// Initialize some data using the provided initializing data
        /// (body is not initialized in editor !)
        /// </summary>
        /// <param name="data1"></param>
        /// <param name="world"></param>
        protected virtual void InitializeCommonData(UniqueBodyGameObjectInitializingData data)
        {
            m_sprite.Texture = TextureRessourceCache.Cached(data.TextureName);
            m_sprite.Tone = data.Tone;
            m_layerDepth = data.Z;
            m_sizePx = data.SizePx;
            m_fixtureOffset = data.FixtureOffset;
            InitializingData = data;
        }

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
        public override void InitializeFromDataInEditor(GameObjectInitializingData data)
        {
            IsActivated = true;
            InitializeCommonData((UniqueBodyGameObjectInitializingData)data);
        }
        /// <summary>
        /// Updates the object's state when its initializing data changed.
        /// </summary>
        public override void UpdateFromInitializingDataInEditor()
        {

        }
        /// <summary>
        /// Dispose the object (editor only)
        /// </summary>
        public override void DisposeInEditor()
        {
            IsDisposed = true;
        }
        /// <summary>
        /// Draws the object in the editor.
        /// </summary>
        /// <param name="batch">batch where to draw the object</param>
        /// <param name="scroll">scrolling of the object</param>
        /// <param name="ops">RenderOptions</param>
        public override void DrawInEditor(SpriteBatch batch, Vector2 scroll, Editor.GameObjectRenderOptions ops)
        {
            // Draws the texture
            UniqueBodyGameObjectInitializingData data = (UniqueBodyGameObjectInitializingData)InitializingData;
            m_sprite.Tone = data.Tone;
            m_sprite.Texture = TextureRessourceCache.Cached(data.TextureName);
            m_sprite.Draw(batch, new Vector2(data.SimStartX, data.SimStartY),
                data.Rotation, scroll, data.FixtureOffset, data.SizePx);
        
            // TODO : add some debug view (in the subclasses ?):
            switch (data.ShapeType)
            {
                case ShapeTypes.Rectangle:
                    Rectangle r = new Rectangle();
                    r.X = (int)(disp(InitializingData.SimStartX) - data.SizePx.X / 2);
                    r.Y = (int)(disp(InitializingData.SimStartY) - data.SizePx.Y / 2);
                    r.Width = (int)m_sizePx.X;
                    r.Height = (int)m_sizePx.Y;
                    DrawingRoutines.DrawRectangle(r);
                    break;
                case ShapeTypes.Circle:
                    Rectangle r1 = new Rectangle();
                    r1.X = (int)(disp(data.SimStartX) - data.SizePx.X / 2);
                    r1.Y = (int)(disp(data.SimStartY) - data.SizePx.Y / 2);
                    r1.Width = (int)data.SizePx.X;
                    r1.Height = (int)data.SizePx.Y;
                    DrawingRoutines.DrawCircle(r1);
                    break;
                case ShapeTypes.Polygon:
                    // TODO Aucune idée de comment je fais ça.
                    break;
            }
        }
        /// <summary>
        /// Gets the type of the initializing data
        /// </summary>
        public override Type GetInitializingDataType()
        {
            // System.Reflection.Assembly.GetExecutingAssembly().GetType("Uef.GameObjects").GetProperty("Name");
            return typeof(UniqueBodyGameObjectInitializingData);
        }
        /// <summary>
        /// Returns a list containing the names of the registrable events.
        /// Used to perform code editing of the event callbacks.
        /// </summary>
        /// <returns></returns>
        public override List<string> GetRegistrableEvents()
        {
            throw new Exception();
        }
#endif
        #endregion
        /* ---------------------------------------------------------------
         * Utils
         * -------------------------------------------------------------*/
        #region Utils
        /// <summary>
        /// Calculates the angle offset between the Center of rotation and the center of the square
        /// </summary>
        /// <returns></returns>
        protected double _calculateAngleOffset()
        {
            return -(Math.PI / 2.0 + Math.Atan(m_fixtureOffset.X / m_fixtureOffset.Y));
        }
        /// <summary>
        /// Calculates the ray from the center of rotation to the center of the square.
        /// </summary>
        /// <returns></returns>
        protected float _calculateRay()
        {

            return (float)Math.Sqrt(Math.Pow(m_fixtureOffset.X, 2) + Math.Pow(m_fixtureOffset.Y, 2));
        }
        #endregion
    }
}
