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
using TBody = FarseerPhysics.Dynamics.Body;
namespace UeFGame.GameObjects
{
    /// <summary>
    /// Class that describes the player.
    /// Important notes :
    ///     - The player has more than one texture.
    ///     - The player is not available in the editor.
    /// </summary>
    public class Player : BasicEvent
    {
        /* ---------------------------------------------------------------
         * Variables
         * -------------------------------------------------------------*/
        #region Variables
        /// <summary>
        /// Collection of bodies used by the player.
        /// </summary>
        BodyCollection m_bodies;
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
            get
            {
                AABB aabb;
                m_body.FixtureList.First().GetAABB(out aabb, 0);
                return aabb;
            }
        }
        /// <summary>
        /// Gets the object initializing data.
        /// </summary>
        public override GameObjectInitializingData InitializingData { get; protected set; }
        /// <summary>
        /// Returns the center of the shape in Simulation units.
        /// It uses Cos and Sin calculation, so don't abuse it !
        /// </summary>
        public override Vector2 ShapeCenterSimPosition
        {
            get
            {
                return new Vector2(m_body.Position.X - _ray * (float)Math.Cos(m_body.Rotation + _angleOffset),
                    m_body.Position.Y - _ray * (float)Math.Sin(m_body.Rotation + _angleOffset));
            }
        }

        #endregion

        /* ---------------------------------------------------------------
         * Basics
         * -------------------------------------------------------------*/
        #region Basics
        /// <summary>
        /// Initializes the player.
        /// </summary>
        public Player(World world)
            : base(world)
        {
            // The new world !!
            m_world = world;
            m_body.Enabled = true;
            IsActivated = true;

            // Initializes base variables
            m_sprite.Texture = TextureRessourceCache.Cached("Editor\\geo");
            m_sprite.Tone = Color.White;
            m_layerDepth = 0.0f;
            m_sizePx = new Vector2(m_sprite.Texture.Width, m_sprite.Texture.Height);
            m_fixtureOffset = Vector2.Zero;

            // Creates a fixture for the body.
            FixtureFactory.AttachRectangle(sim(m_sizePx.X), sim(m_sizePx.Y), 1.0f, m_fixtureOffset, m_body);

            // Now setup body variables
            m_body.Position = sim(new Vector2(150, 150));
            m_body.BodyType = BodyType.Dynamic;
            m_body.Restitution = 0.0f;
            m_body.Friction = Constants.EventFriction;
            m_body.Mass = 70.0f;
            m_body.LinearDamping = 0.0f;
            m_body.FixedRotation = true;
            m_jumpManager.SetOffset(new Vector2(m_sizePx.X / 2, m_sizePx.Y / 2));
            // Setup components
            SetupBodyCollection(ShapeTypes.Rectangle, 1.0f, null);

            // Precomputes some values
            _ray = _calculateRay();
            _angleOffset = _calculateAngleOffset();
        }
        /// <summary>
        /// Initializes the player's move manager.
        /// Called by the base (BasicEvent) constructor.
        /// </summary>
        protected override void InitializeMoveManager()
        {
            m_moveManager = new MoveManagers.PlayerMoveManager(this);
        }
        /// <summary>
        /// Disposes the GameObject and frees all the allocated ressources.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose(); // body + IsDisposed
        }
        /// <summary>
        /// Updates the Game Object.
        /// </summary>
        /// <param name="gameTime">the gametime object given by the Xna framework</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            UpdateInput();
        }
        /// <summary>
        /// Updates the input-related stuff
        /// </summary>
        void UpdateInput()
        {
            if (Input.IsPressed(Input.KRight))
            {
                m_moveManager.MoveRight();
            }
            if (Input.IsPressed(Input.KLeft))
            {
                m_moveManager.MoveLeft();
            }
            if (Input.IsPressed(Input.KJump))
            {
                m_jumpManager.RequestJump();
            }
            if (Input.IsPressed(Input.KDown))
            {
                BodyId = BodyId.Down;
            }
            else
            {
                BodyId = BodyId.Normal;
            }
        }
        /// <summary>
        /// Draws the object on the map
        /// </summary>
        /// <param name="batch">the spritebatch where to draw the object</param>
        /// <param name="scroll">the scrolling value</param>
        public override void Draw(SpriteBatch batch, Vector2 scroll)
        {
            if (IsDisposed)
                throw new InvalidOperationException("Do not call draw on a deactivated or disposed object");
            if (!isOutOfScreen(scroll))
            {
                m_sprite.Draw(batch, m_body, scroll, m_fixtureOffset, m_sizePx);
            }
        }
        /// <summary>
        /// Deactivate this instance of GameObject.
        /// </summary>
        public override void Deactivate()
        {
            throw new Exception("Impossible to deactivate Player");
        }
        /// <summary>
        /// Initializes the event from a GameObjectInitializingData object.
        /// </summary>
        /// <param name="data">the object used to initialize the event</param>
        public override void InitializeFromData(GameObjectInitializingData data1, World world)
        {
            throw new Exception("Impossible to initialize player from data.");
        }

        #endregion
        /* ---------------------------------------------------------------
         * Common Game / Editor
         * -------------------------------------------------------------*/
        #region Common Game / Editor
        /// <summary>
        /// Initialize some data using the provided initializing data
        /// </summary>
        /// <param name="data1"></param>
        /// <param name="world"></param>
        protected override void InitializeCommonData(UniqueBodyGameObjectInitializingData data)
        {
            throw new Exception("No common data to be initialized");
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
            throw new Exception("Player not available in editor");
        }
        /// <summary>
        /// Updates the object's state when its initializing data changed.
        /// </summary>
        public override void UpdateFromInitializingDataInEditor()
        {
            throw new Exception("Player not available in editor");
        }
        /// <summary>
        /// Dispose the object (editor only)
        /// </summary>
        public override void DisposeInEditor()
        {
            throw new Exception("Player not available in editor");
        }
        /// <summary>
        /// Draws the object in the editor.
        /// </summary>
        /// <param name="batch">batch where to draw the object</param>
        /// <param name="scroll">scrolling of the object</param>
        /// <param name="ops">RenderOptions</param>
        public override void DrawInEditor(SpriteBatch batch, Vector2 scroll, Editor.GameObjectRenderOptions ops)
        {
            throw new Exception("Player not available in editor");
        }
        /// <summary>
        /// Gets the type of the initializing data
        /// </summary>
        public override Type GetInitializingDataType()
        {
            throw new Exception("Player not available in editor");
        }
        /// <summary>
        /// Returns a list containing the names of the registrable events.
        /// Used to perform code editing of the event callbacks.
        /// </summary>
        /// <returns></returns>
        public override List<string> GetRegistrableEvents()
        {
            throw new Exception("Player not available in editor");
        }
#endif
        #endregion

        /* ---------------------------------------------------------------
         * Utils
         * -------------------------------------------------------------*/
        #region Utils
        /// <summary>
        /// Returns a texture relative coordinate from a body relative coordinate.
        /// </summary>  
        /// <param name="bodyRelative">body relative value to convert</param>
        /// <param name="horiz">true if the given data is width related</param>
        /// <returns></returns>
        protected float texRelative(float bodyRelative, bool horiz)
        {
            if (horiz)
                return bodyRelative;
            else
                return bodyRelative;
        }
        #endregion
    }
}
