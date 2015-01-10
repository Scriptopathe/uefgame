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
    /// Base class for the basic events
    /// </summary>
    [Editor.EditableGameObject(typeof(UniqueBodyGameObjectInitializingData))]
    public class BasicEvent : UniqueBodyGameObject
    {
        /* ---------------------------------------------------------------
         * Variables
         * -------------------------------------------------------------*/
        #region Variables
        protected MoveManagers.IMoveManager m_moveManager;
        protected JumpManagers.IJumpManager m_jumpManager;
        /// <summary>
        /// Collection of bodies used by the event.
        /// </summary>
        BodyCollection m_bodies;
        #endregion
        /* ---------------------------------------------------------------
         * Properties
         * -------------------------------------------------------------*/
        #region Properties
        /// <summary>
        /// Get the MoveManager of this event.
        /// </summary>
        public virtual MoveManagers.IMoveManager MoveManager
        {
            get { return m_moveManager; }
        }
        /// <summary>
        /// Gets the JumpManager of this event.
        /// </summary>
        public virtual JumpManagers.IJumpManager JumpManager
        {
            get { return m_jumpManager; }
        }
        /// <summary>
        /// Sets the bodyId
        /// </summary>
        public BodyId BodyId
        {
            set
            {
                TBody body = m_bodies[value];
                if (m_body != body)
                {
                    body.Enabled = true;
                    body.Position = m_body.Position;
                    body.LinearVelocity = m_body.LinearVelocity;
                    body.LinearDamping = m_body.LinearDamping;
                    body.AngularDamping = m_body.AngularDamping;
                    body.AngularVelocity = m_body.AngularVelocity;
                    body.Awake = true;
                    m_body.Enabled = false;
                    m_body = body;
                    FixtureOffset = m_bodies.GetFixtureOffset(value);
                    SizePx = m_bodies.GetSizePx(value);
                }

            }
        }
        /// <summary>
        /// Gets or sets the fixture offset.
        /// Setting the fixture offset REQUIRES doing it through this property.
        /// </summary>
        Vector2 FixtureOffset
        {
            get { return m_fixtureOffset; }
            set
            {
                // if we change the fixture offset, we must recalculate these :
                _calculateAngleOffset();
                _calculateRay();
            }
        }
        /// <summary>
        /// Gets or sets the size px value.
        /// WARNING : this method should only be called inside the BodyId property.
        /// </summary>
        Vector2 SizePx
        {
            get { return m_sizePx; }
            set
            {
                // Changement de la position
                Vector2 deltaSize = (m_sizePx - value) / 2;
                Body.Position = new Vector2(Body.Position.X + sim(deltaSize.X), Body.Position.Y + sim(deltaSize.Y));
                m_sizePx = value;
                m_jumpManager.SetOffset(new Vector2(m_sizePx.X / 2, m_sizePx.Y / 2));
            }
        }
        #endregion
        /* ---------------------------------------------------------------
         * Methods overrides
         * -------------------------------------------------------------*/
        #region Methods overrides
        public BasicEvent(World world)
            : base(world)
        {
            InitializeMoveManager();
            InitializeJumpManager();
            m_bodies = new BodyCollection();
        }
        /// <summary>
        /// Initializes the event.
        /// </summary>
        /// <param name="data1"></param>
        /// <param name="world"></param>
        public override void InitializeFromData(GameObjectInitializingData data1, World world)
        {
            base.InitializeFromData(data1, world);
            // Adds the reset
            SetupBodyCollection();

            m_moveManager.Reset();
            m_jumpManager.Reset();
            m_jumpManager.SetOffset(new Vector2(m_sizePx.X / 2, m_sizePx.Y / 2));
        }
        /// <summary>
        /// Updates the event.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            m_moveManager.Update();
            m_jumpManager.Update();
            if(this.GetType() != typeof(Player))
                if (Globals.Rand.Next(20) == 1)
                    if (Globals.Rand.Next(2) == 0)
                        BodyId = GameObjects.BodyId.Down;
                    else
                        BodyId = GameObjects.BodyId.Normal;
                else
                    switch(Globals.Rand.Next(5))
                    {
                        case 0:
                            m_jumpManager.RequestJump();
                            break;
                        case 1:
                            m_moveManager.MoveLeft();
                            break;
                        case 2:
                            m_moveManager.MoveRight();
                            break;
                    }

                    // BodyId = GameObjects.BodyId.Normal;
        }
        #endregion
        /* ---------------------------------------------------------------
         * Init
         * -------------------------------------------------------------*/
        #region Init
        /// <summary>
        /// Sets up the body collection, including :
        ///     - Normal body
        ///     - Squat body
        /// </summary>
        protected void SetupBodyCollection(ShapeTypes shapeType, float density, Vertices shapePolygonVertices)
        {
            // Body collection
            m_bodies = new BodyCollection();

            // Squat body
            TBody body = BodyFactory.CreateBody(m_world);
            body.Enabled = false;
            Vector2 fixOffset = Vector2.Zero;
            Vector2 sizePx = new Vector2(m_sizePx.X, m_sizePx.Y / 2);
            // Create the shape.
            switch (shapeType)
            {
                case ShapeTypes.Rectangle:
                    FixtureFactory.AttachRectangle(sim(sizePx.X), sim(sizePx.Y), density, m_fixtureOffset, body);
                    break;
                case ShapeTypes.Circle:
                    FixtureFactory.AttachEllipse(sim(sizePx.X / 2), sim(sizePx.Y / 4), 20, density, body);
                    break;
                case ShapeTypes.Polygon:
                    FixtureFactory.AttachPolygon(shapePolygonVertices, 1.0f, body);
                    break;
            }
            CopyBodyCharacteristics(m_body, body);

            // Add the bodies
            m_bodies.Add(body, BodyId.Down, fixOffset, sizePx);
            m_bodies.Add(m_body, BodyId.Normal, m_fixtureOffset, m_sizePx);
        }
        /// <summary>
        /// Sets up the body collection, including :
        ///     - Normal body
        ///     - Squat body
        /// </summary>
        protected void SetupBodyCollection()
        {
            UniqueBodyGameObjectInitializingData data = (UniqueBodyGameObjectInitializingData)InitializingData;

            // Body collection
            m_bodies = new BodyCollection();

            // Squat body
            TBody body = BodyFactory.CreateBody(m_world);
            body.Enabled = false;
            Vector2 fixOffset = Vector2.Zero;
            Vector2 sizePx = new Vector2(m_sizePx.X, m_sizePx.Y / 2);
            // Create the shape.
            switch (data.ShapeType)
            {
                case ShapeTypes.Rectangle:
                    FixtureFactory.AttachRectangle(sim(sizePx.X), sim(sizePx.Y / 2), data.BodyDensity, m_fixtureOffset, body);
                    break;
                case ShapeTypes.Circle:
                    FixtureFactory.AttachEllipse(sim(sizePx.X / 2), sim(sizePx.Y / 2), 20, data.BodyDensity, body);
                    break;
                case ShapeTypes.Polygon:
                    //FixtureFactory.AttachPolygon(data.ShapePolygonVertices, 1.0f, body);
                    break;
            }
            CopyBodyCharacteristics(m_body, body);

            // Add the bodies
            m_bodies.Add(body, BodyId.Down, fixOffset, sizePx);
            m_bodies.Add(m_body, BodyId.Normal, m_fixtureOffset, m_sizePx);
        }
        /// <summary>
        /// Copies the main characteristics of a body, for its initialization.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        void CopyBodyCharacteristics(Body from, Body to)
        {
            to.BodyType = from.BodyType;
            to.Restitution = from.Restitution;
            to.Friction = from.Friction;
            to.Mass = from.Mass;
            to.LinearDamping = from.LinearDamping;
            to.FixedRotation = from.FixedRotation;
            to.Rotation = from.Rotation;
        }

        #endregion
        /* ---------------------------------------------------------------
         * Virtual methods
         * -------------------------------------------------------------*/
        #region Virtual methods
        /// <summary>
        /// Initialize the move manager.
        /// Override it to put the subclass specific manager and settings.
        /// </summary>
        protected virtual void InitializeMoveManager()
        {
            m_moveManager = new MoveManagers.PlayerMoveManager(this);
        }
        /// <summary>
        /// Initializes the jump manager.
        /// Override it to put the subclass specific manager and settings.
        /// </summary>
        protected virtual void InitializeJumpManager()
        {
            Vector2 offset = new Vector2(m_sizePx.X / 2, m_sizePx.Y / 2);
            m_jumpManager = new JumpManagers.PlayerJumpManager(this, offset);
        }
        #endregion
    }
}
