using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision;
using FarseerPhysics.Common;
using System.Xml.Serialization;
namespace UeFGame.GameObjects
{

    /// <summary>
    /// Base class for all Game Events :
    /// x implements drawing functions
    /// - effects (poison etc...)
    /// - shoot collision management
    /// x sneak position
    /// </summary>
    /*[Editor.EditableGameObject(
        new string[] {"base", "physical_object", "game_event" },
        new Type[] { typeof(BaseModule), typeof(PhysicalObjectModule), typeof(GameEventModule)})]*/
    [Editor.EditableGameObject(
        new string[] { "base", "physical_object", "game_event" },
        new Type[] { typeof(BaseModule), typeof(PhysicalObjectModule), typeof(GameEventModule)})]
    public class GameEvent : PhysicalObject
    {
        #region Events
        /// <summary>
        /// Called when the events collides with another body.
        /// </summary>
        public event FarseerPhysics.Dynamics.OnCollisionEventHandler OnBodyCollision;
        public event ScriptingUpdateEventHandler OnUpdate;
        #endregion
        /* ---------------------------------------------------------------------------------------
         * Variables
         * -------------------------------------------------------------------------------------*/
        #region Variable
        /// <summary>
        /// Collection of bodies usable by the player.
        /// </summary>
        private BodyCollection m_bodies = new BodyCollection();
        /// <summary>
        /// Current Body Id.
        /// </summary>
        private BodyId m_currentBodyId = BodyId.Normal;
        /// <summary>
        /// Creates the EventSprite.
        /// </summary>
        protected GameEventSpriteBase m_sprite;
        /// <summary>
        /// Queue de commandes de cet event.
        /// </summary>
        protected Scripting.CommandQueue m_commandQueue;
        /// <summary>
        /// Retourne vrai si la Queue de commandes de l'event est active.
        /// </summary>
        protected bool m_queueEnabled = true;
        /// <summary>
        /// List of RegisteredCollidingEvents.
        /// </summary>
        protected List<GameObject> m_registeredCollidingEvents;
        #endregion
        /* ---------------------------------------------------------------------------------------
         * Properties
         * -------------------------------------------------------------------------------------*/
        #region Properties
        #region Init
        /// <summary>
        /// Casted data used to initialize the physical object.
        /// </summary>
        public GameEventModule MGameEvent
        {
            get;
            protected set;
        }
        #endregion

        /// <summary>
        /// Gets the CommandQueue of this GameEvent.
        /// </summary>
        public Scripting.CommandQueue Queue
        {
            get { return m_commandQueue; }
        }

        /// <summary>
        /// Gets or sets the current Body Id.
        /// </summary>
        protected BodyId CurrentBodyId
        {
            get { return m_currentBodyId; }
            set
            {
                m_currentBodyId = value;
                OnBodyIdChanged();
            }
        }
        /// <summary>
        /// Gets the ShapeSize of the current Body of this event (cf in PhysicalObjectInit) in simulation units.
        /// </summary>
        public override Vector2 ShapeSizeSim
        {
            get { return m_bodies.GetShapeSizeSim(m_currentBodyId); }
        }
        /// <summary>
        /// Gets the CenterOffset of the current Body of this event (cf in PhysicalObjectInit) in simulation units.
        /// </summary>
        public override Vector2 CenterOffset
        {
            get { return m_bodies.GetCenterOffset(m_currentBodyId); }
        }
        /// <summary>
        /// Gets the ShapeType of the current Body of this event.
        /// </summary>
        public override ShapeType ShapeType
        {
            get { return m_bodies.GetShapeType(m_currentBodyId); }
        }
        /// <summary>
        /// True if the event has sneak body.
        /// </summary>
        bool HasSneakBody
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets a value indicating whether or not the event register collisions.
        /// </summary>
        public bool RegisterCollisions
        {
            get;
            set;
        }
        #endregion
        /* ---------------------------------------------------------------------------------------
         * Methods
         * -------------------------------------------------------------------------------------*/
        #region Methods
        /// <summary>
        /// Creates a new instance of GameEvent.
        /// </summary>
        public GameEvent()
            : base()
        {
            m_commandQueue = new Scripting.CommandQueue();
            m_registeredCollidingEvents = new List<GameObject>();
        }

        #region Init
        
        /// <summary>
        /// Initializes this object using the provided InitializingData.
        /// This call initializes in that order :
        ///     - The Bodies (Physical Object)
        ///     - The Sprite (InitializeSprite)
        ///     - The Event Queue (InitializeQueue)
        ///     - The Scripts (InitializeScript)
        /// These methods can be overriden in subclasses of event.
        /// This one provides general features.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            MGameEvent = (GameEventModule)Modules["game_event"];
            // Collisions
            RegisterCollisions = MGameEvent.RegisterCollisions;
            m_registeredCollidingEvents.Clear();

            // Bodies management
            m_bodies.Add(m_body, BodyId.Normal, MPhysicalObject.CenterOffset, MPhysicalObject.ShapeSizeSim, MPhysicalObject.ShapeType);
            if(HasSneakBody)
                InitSneakBody();

            InitializeSprite();

            InitializeQueue();

            InitializeScript();
        }
        
        /// <summary>
        /// Initialize the Event's command queue.
        /// </summary>
        void InitializeQueue()
        {
            m_commandQueue.Reset();
        }
        /// <summary>
        /// Initialize the Event's script.
        /// </summary>
        void InitializeScript()
        {
            if (!Globals.ExecuteInEditor && Globals.GameMap.Assemblies.ContainsKey(MapId))
            {
                Type t = Globals.GameMap.Assemblies[MapId].GetType("UeFGame.GameObjects.GeneratedCode.Map_" + MapId.ToString());
                string methodName = "Evt_" + MBase.BehaviorID.ToString() + "_Initialize";
                var m = t.GetMethods();
                var method = t.GetMethod(methodName);
                if(method != null)
                    method.Invoke(this, new object[] { this });
            }
        }
        /// <summary>
        /// Initializes the "sneak" body.
        /// </summary>
        private void InitSneakBody()
        {
            // Create the sneak body
            Body sneakBody = InitBody(MPhysicalObject);
            InitializeShape(MPhysicalObject.ShapeType, new Vector2(MPhysicalObject.ShapeSizeSim.X, MPhysicalObject.ShapeSizeSim.Y / 2),
                MPhysicalObject.BodyDensity, MPhysicalObject.CenterOffset, sneakBody);
            m_bodies.Add(sneakBody, BodyId.Sneak, MPhysicalObject.CenterOffset, MPhysicalObject.ShapeSizeSim, MPhysicalObject.ShapeType);
            sneakBody.Enabled = false;
            // Now initializes the Sprite.
        }
        /// <summary>
        /// Method called in GameEvent.Initialize in order to create a the sprite instance and
        /// perform setup on it.
        /// </summary>
        protected virtual void InitializeSprite()
        {
            EventSprite spr = new EventSprite();
            m_sprite = spr;
            spr.Texture = TextureRessourceCache.Cached(MGameEvent.TextureName);
            spr.Tone = MGameEvent.Tone;
        }
        #endregion

        /// <summary>
        /// Updates the GameEvent.
        /// </summary>
        /// <param name="time"></param>
        public override void Update(GameTime time)
        {
            if (OnUpdate != null)
                OnUpdate(this, time);

            UpdateQueue();
        }

        /// <summary>
        /// Updates the Event's command queue.
        /// </summary>
        protected virtual void UpdateQueue()
        {
            m_commandQueue.Update(this);
        }

        /// <summary>
        /// This method is called when the Body Id changes. It performs the effective change operation.
        /// </summary>
        protected void OnBodyIdChanged()
        {
            Body oldBody = m_body;
            Body newBody = m_bodies[m_currentBodyId];

            if (oldBody != newBody)
            {
                // Gives the newBody the state and movement of the 
                // old one.
                newBody.Enabled = true;
                newBody.Position = oldBody.Position;
                newBody.LinearVelocity = oldBody.LinearVelocity;
                newBody.LinearDamping = oldBody.LinearDamping;
                newBody.AngularDamping = oldBody.AngularDamping;
                newBody.AngularVelocity = oldBody.AngularVelocity;
                newBody.Rotation = oldBody.Rotation;
                newBody.Awake = true;
                oldBody.Enabled = false;
            }

            m_body = newBody;
        }

        /// <summary>
        /// Draw the game event.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="batch"></param>
        /// <param name="scroll"></param>
        public override void Draw(GameTime time, SpriteBatch batch, Vector2 scroll)
        {
            if(m_sprite != null)
                m_sprite.Draw(batch, this, scroll, DrawingDepths.DefaultEventDepth);
        }
        #endregion
        /* ---------------------------------------------------------------------------------------
         * Event API
         * -------------------------------------------------------------------------------------*/
        #region API
        /// <summary>
        /// Returns true if this event is colliding with the Given GameObject.
        /// /!\ This method can be slow.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool IsCollidingWith(GameObject obj)
        {
            if (!RegisterCollisions)
                throw new InvalidOperationException("IsCollidingWith ne peut être appelé que si RegisterCollisions est activé sur l'event");

            return m_registeredCollidingEvents.Contains(obj);
        }
        /// <summary>
        /// Callback for farseer's on separation event.
        /// </summary>
        /// <param name="fixtureA"></param>
        /// <param name="fixtureB"></param>
        protected override void OnSeparation(Fixture fixtureA, Fixture fixtureB)
        {
            base.OnSeparation(fixtureA, fixtureB);
            if (fixtureB.Body.UserData is PhysicalObject)
            {
                if (RegisterCollisions)
                    m_registeredCollidingEvents.Remove((GameObject)fixtureB.Body.UserData);
            }
        }
        // TODO :
        // MoveLeft
        // MoveRight
        // TryJump
        // Sneak
        /// <summary>
        /// Callback for farseer's on collision event.
        /// </summary>
        /// <param name="fixtureA"></param>
        /// <param name="fixtureB"></param>
        /// <param name="contact"></param>
        /// <returns></returns>
        protected override bool OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            if (fixtureB.Body.UserData is BodyCategories)
            {
                // C'est un mur
                return true;
            }

            if (fixtureB.Body.UserData is PhysicalObject)
            {
                if (RegisterCollisions)
                    m_registeredCollidingEvents.Add((GameObject)fixtureB.Body.UserData);

                PhysicalObject obj = (PhysicalObject)(fixtureB.Body.UserData);
                if (obj.BodyCategory == BodyCategories.NoTouch)
                    return false;
            }

            if (OnBodyCollision != null)
                return OnBodyCollision(fixtureA, fixtureB, contact);

            return true;
        }

        #endregion
        /* ---------------------------------------------------------------------------------------
         * Editor
         * -------------------------------------------------------------------------------------*/
        #region Editor
        string _oldTextureName;
        /// <summary>
        /// Draws the object in the editor.
        /// </summary>
        /// <param name="batch">batch where to draw the object</param>
        /// <param name="scroll">scrolling of the object in pixels</param>
        /// <param name="ops">RenderOptions</param>
        public override void DrawInEditor(SpriteBatch batch, Vector2 scroll, Editor.GameObjectRenderOptions ops)
        {
            // Choose layer depth of the debug views (lines etc...)
            float layerDepth = ops.DrawOnTop ? 0.9f : 0.6f;

            // Bounding box
            DrawBoundingBoxInEditor(batch, scroll, ops, layerDepth);

            // Here we draw the Shape
            DrawShapeInEditor(batch, scroll, ops, layerDepth);

            // Here we draw the event.
            if (m_sprite == null)
            {
                InitializeSprite();
                _oldTextureName = MGameEvent.TextureName;
            }

            if (_oldTextureName != MGameEvent.TextureName)
            {
                _oldTextureName = MGameEvent.TextureName;
                m_sprite.Texture = TextureRessourceCache.Cached(MGameEvent.TextureName);
            }
            m_sprite.DrawInEditor(batch, this, scroll, layerDepth, ops);
        }
        #endregion
    }
}
