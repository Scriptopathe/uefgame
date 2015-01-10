using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UeFGame.GameObjects.Particles;
namespace UeFGame.GameObjects.Player
{
    /// <summary>
    /// The player !
    /// </summary>
    public class Player : GameEvent
    {
        /* --------------------------------------------------------------------------------------------------
         * Variables
         * ------------------------------------------------------------------------------------------------*/
        #region Variables
        /// <summary>
        /// Sprite du joueur.
        /// </summary>
        PlayerSprite m_playerSprite;
        /// <summary>
        /// Gestionnaire d'états d'animations du joueur.
        /// </summary>
        AnimationStateManager m_animationStateManager;
        /// <summary>
        /// Gestionnaire de combos.
        /// </summary>
        Combos.ChainCombo m_comboManager = new Combos.ChainCombo();
        /// <summary>
        /// Gestionnaire de mouvements.
        /// </summary>
        PlayerMoveManager m_moveManager;
        /// <summary>
        /// Gestionnaire de saut.
        /// </summary>
        PlayerJumpManager m_jumpManager;
        #endregion
        /* --------------------------------------------------------------------------------------------------
         * Properties
         * ------------------------------------------------------------------------------------------------*/
        #region Properties
        /// <summary>
        /// Direction du joueur.
        /// </summary>
        public PlayerDirection Direction { get; protected set; }
        /// <summary>
        /// Etat d'animation du joueur.
        /// </summary>
        public PlayerAnimationState AnimationState
        {
            get { return m_animationStateManager.CurrentState; }
        }

        /* --------------------------------------------------------------------------------------------------
         * States
         * Les états sont définis avant la détermination de l'état d'animation et permette l'exécution
         * des prédicates de ce dernier.
         * ------------------------------------------------------------------------------------------------*/
        #region States
        /// <summary>
        /// Obtient ou définit une valeur déterminant si le joueur est en état IDLE.
        /// </summary>
        public bool IsIdle
        {
            get;
            set;
        }
        /// <summary>
        /// Obtient ou définit une valeur déterminant si le joueur est en état RUNNING.
        /// </summary>
        public bool IsRunning
        {
            get;
            set;
        }
        /// <summary>
        /// Obtient ou définit une valeur déterminant si le joueur est collé au sol.
        /// </summary>
        public bool IsStickingOnGround
        {
            get;
            set;
        }
        /// <summary>
        /// Obtient ou définit une valeur déterminant si le joueur est en train de sauter.
        /// </summary>
        public bool IsStartingJump
        {
            get;
            set;
        }
        /// <summary>
        /// Obtient ou définit une valeur déterminant si le joueur est entrain de tomber.
        /// </summary>
        public bool IsFalling
        {
            get;
            set;
        }
        #endregion
        #endregion
        /* --------------------------------------------------------------------------------------------------
         * Methods
         * ------------------------------------------------------------------------------------------------*/
        #region Methods
        /* --------------------------------------------------------------------------------------------------
         * Sprite
         * ------------------------------------------------------------------------------------------------*/
        #region Sprite
        /// <summary>
        /// Initialise le sprite du joueur.
        /// </summary>
        protected override void InitializeSprite()
        {
            m_playerSprite = new PlayerSprite(TextureRessourceCache.Cached("RunTimeAssets\\Graphics\\Geo\\geo-course"));
        }
        /// <summary>
        /// Dessine le joueur en utilisant PlayerSprite.
        /// </summary>
        public override void Draw(Microsoft.Xna.Framework.GameTime time, Microsoft.Xna.Framework.Graphics.SpriteBatch batch, Microsoft.Xna.Framework.Vector2 scroll)
        {
            m_playerSprite.Draw(batch, this, scroll, MBase.Z);
        }
        #endregion
        /* --------------------------------------------------------------------------------------------------
         * Input
         * ------------------------------------------------------------------------------------------------*/
        #region Input
        /// <summary>
        /// Mets à jour l'état du joueur en fonction des entrées clavier / manette.
        /// </summary>
        void UpdateInput()
        {
            if (Input.IsTrigger(Microsoft.Xna.Framework.Input.Keys.Up))
            {
                /*if(m_body.LinearVelocity.Y >= -5.0f)
                    m_body.ApplyLinearImpulse(new Microsoft.Xna.Framework.Vector2(0, -0.07f));*/
                m_jumpManager.RequestJump();

            }

            IsIdle = true;
            IsRunning = false;
            if (Input.IsPressed(Microsoft.Xna.Framework.Input.Keys.Right))
            {
                //m_body.LinearVelocity = new Microsoft.Xna.Framework.Vector2(5, m_body.LinearVelocity.Y);;
                m_moveManager.MoveRight();
                Direction = PlayerDirection.Right;
                IsIdle = false;
                IsRunning = true;
            }

            if (Input.IsPressed(Microsoft.Xna.Framework.Input.Keys.Left))
            {
                //m_body.LinearVelocity = new Microsoft.Xna.Framework.Vector2(-5f, m_body.LinearVelocity.Y);//
                m_moveManager.MoveLeft();
                Direction = PlayerDirection.Left;
                IsIdle = false;
                IsRunning = true;
            }
        }

        #endregion
        /* --------------------------------------------------------------------------------------------------
         * Animation
         * ------------------------------------------------------------------------------------------------*/
        #region Animation
        /// <summary>
        /// Update the animation States.
        /// </summary>
        void UpdateAnimationStates()
        {
            IsFalling = Body.LinearVelocity.Y > 0.05f;
            IsStartingJump = m_jumpManager.IsJumpRequested && m_jumpManager.IsStickingOnGround;
            m_animationStateManager.Update(this);
        }
        #endregion
        #endregion



        #region Test
        Random rand = new Random();
        int i = 0;
        ImageParticleInit m_partInit;

        public Player() : base()
        {
            m_partInit = new ImageParticleInit();
            m_partInit.TextureName = "RunTimeAssets\\Graphics\\Particles\\test";
            m_partInit.Lifetime = 2000;
            m_partInit.FadeOutTime = 1500;
            m_partInit.AngleVelocity = 1f;
            RegisterCollisions = true;
            BodyCategory = BodyCategories.Friend;
            m_moveManager = new PlayerMoveManager(this);
            m_jumpManager = new PlayerJumpManager(this, new Microsoft.Xna.Framework.Vector2(16, 16));
            m_animationStateManager = new AnimationStateManager();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime time)
        {
            i++;
            m_body.FixedRotation = true;
            m_body.BodyType = FarseerPhysics.Dynamics.BodyType.Dynamic;
            //m_body.IgnoreGravity = true;

            UpdateInput();

            if (Input.IsPressed(Microsoft.Xna.Framework.Input.Keys.Down))
            {
                m_body.ApplyTorque(2f);
            }

            if (Input.IsTrigger(Microsoft.Xna.Framework.Input.Keys.Enter))
            {
                Shoots.GatlingShootInit init = new Shoots.GatlingShootInit();
                init.BodyCategory = BodyCategories.Friend;
                init.SimStart = m_body.Position + new Microsoft.Xna.Framework.Vector2(1f, 0.12f);
                Shoots.GatlingShoot.Pool.GenericGetFromPool(init);
            }

            // Mise à jour des gestionnaires.
            m_comboManager.Update(time);
            m_moveManager.Update(time);
            m_jumpManager.Update();
            UpdateAnimationStates();
            

            
            m_partInit.PxX = (int)disp(m_body.Position.X);
            m_partInit.PxY = (int)disp(m_body.Position.Y);
            m_partInit.Angle = m_body.Rotation;
            ImageParticle.Pool.GenericGetFromPool(m_partInit);

            base.Update(time);
        }
        #endregion
    }
}
