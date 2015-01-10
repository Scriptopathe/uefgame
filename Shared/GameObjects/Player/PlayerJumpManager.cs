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
using FarseerPhysics.Dynamics;
namespace UeFGame.GameObjects
{
    class PlayerJumpManager : IJumpManager
    {
        #region Variables
        /// <summary>
        /// Linear velocity which will be applied to the object in px/frame.
        /// </summary>
        const int JumpFactorY = 1300;
        /// <summary>
        /// Linear velocity which will be applied to the object in px/frame.
        /// </summary>
        const int JumpFactorX = 5;
        const int MaxLinearVelocity = 1000; // maximum linear velocity in px/frame.
        /// <summary>
        /// Event attached to this manager.
        /// </summary>
        GameEvent m_gameEvent;
        /// <summary>
        /// Call back called when a raycast is effectued.
        /// </summary>
        RayCastCallback m_onJumpCallback;
        RayCastCallback m_onMuralJumpCallback;
        /// <summary>
        /// Vector containing :
        ///     - X : the half-width of the object
        ///     - Y : the offset from the world center of the body
        ///     to the bottom of the body. (half-height of the body)
        /// </summary>
        Vector2 m_detectionOffset;
        /// <summary>
        /// Indicates if the jump has started from the raycast.
        /// (used to end the loops)
        /// </summary>
        int m_rayCastJumpSuccesses = 0;
        /// <summary>
        /// Determines wheither or not a jump has been requested for this frame.
        /// </summary>
        bool m_jumpRequested = false;
        #endregion

        #region Properties
        /// <summary>
        /// Returns true if the player is currently sticking on the ground.
        /// </summary>
        public bool IsStickingOnGround
        {
            get;
            protected set;
        }
        /// <summary>
        /// Returns true if a jump has been requested.
        /// </summary>
        public bool IsJumpRequested
        {
            get { return m_jumpRequested; }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes the jump manager.
        /// </summary>
        /// <param name="body">Vector containing :
        ///     - X : the half-width of the object
        ///     - Y : the offset from the world center of the body
        ///     to the bottom of the body.
        ///     In pixels</param>
        /// <param name="offset"></param>
        public PlayerJumpManager(GameEvent evt, Vector2 offset)
        {
            m_gameEvent = evt;
            m_detectionOffset = sim(offset);
            m_onJumpCallback = new RayCastCallback(StickingOnGroundPredicate);
        }
        #endregion

        #region IJumpManager Initialization
        /// <summary>
        /// Updates the manager.
        /// </summary>
        public virtual void Update()
        {
            // Determines wheither or not the player is sticking on the ground.
            for (float x = -m_detectionOffset.X; x < m_detectionOffset.X && !(m_rayCastJumpSuccesses > 1); x+=sim(1))
            {
                Globals.World.RayCast(m_onJumpCallback,
                    new Vector2(m_gameEvent.Body.Position.X + x, m_gameEvent.Body.Position.Y + m_detectionOffset.Y),
                    new Vector2(m_gameEvent.Body.Position.X + x, m_gameEvent.Body.Position.Y + m_detectionOffset.Y + sim(4)));
            }

            // Jumps if requested.
            if (m_jumpRequested && IsStickingOnGround)
            {
                m_gameEvent.Body.LinearVelocity = new Vector2(m_gameEvent.Body.LinearVelocity.X, -sim(JumpFactorY));
            }
            m_jumpRequested = false;
            IsStickingOnGround = false;
        }

        /// <summary>
        /// Requests for a jump
        /// </summary>
        /// <returns>True if the jump is sucessfull.</returns>
        public virtual void RequestJump()
        {
            m_jumpRequested = true;
        }
        /// <summary>
        /// Resets the jump manager.
        /// </summary>
        public virtual void Reset()
        {

        }
        /// <summary>
        // Vector containing :
        ///     - X : the half-width of the object
        ///     - Y : the offset from the world center of the body
        ///     to the bottom of the body.
        /// In display units.
        /// </summary>
        /// <param name="offset"></param>
        public virtual void SetOffsetPx(Vector2 offset)
        {
            m_detectionOffset = ConvertUnits.ToSimUnits(offset);
        }
        #endregion

        /// <summary>
        /// Callback of the raycast : when the up key is pressed, and when there is
        /// something under the player's feet, starts a jump.
        /// </summary>
        float StickingOnGroundPredicate(Fixture fixture, Vector2 contactPoint, Vector2 normal, float f2)
        {
            Vector2 point = new Vector2(m_gameEvent.Body.WorldCenter.X, m_gameEvent.Body.WorldCenter.Y);
            if (normal.X <= 0.8 && normal.X >= -0.8)
            {
                IsStickingOnGround = true;
            }
            return 0;
        }

        /* ---------------------------------------------------------------
         * Utils
         * -------------------------------------------------------------*/
        #region Utils
        protected float sim(double v) { return ConvertUnits.ToSimUnits(v); }
        protected Vector2 sim(Vector2 v) { return ConvertUnits.ToSimUnits(v); }
        protected float disp(float v) { return ConvertUnits.ToDisplayUnits(v); }
        protected Vector2 disp(Vector2 v) { return ConvertUnits.ToDisplayUnits(v); }
        #endregion
    }
}