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
namespace UeFGame.GameObjects.JumpManagers
{
    class PlayerJumpManager : IJumpManager
    {
        #region Variables
        /// <summary>
        /// Linear velocity which will be applied to the object in px/frame.
        /// </summary>
        const int JumpFactorY = 700;
        /// <summary>
        /// Linear velocity which will be applied to the object in px/frame.
        /// </summary>
        const int JumpFactorX = 5;
        const int MaxLinearVelocity = 1000; // maximum linear velocity in px/frame.
        /// <summary>
        /// Event attached to this manager.
        /// </summary>
        BasicEvent m_gameEvent;
        /// <summary>
        /// Call back called when a raycast is effectued.
        /// </summary>
        RayCastCallback m_onJumpCallback;
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
        public PlayerJumpManager(BasicEvent evt, Vector2 offset)
        {
            m_gameEvent = evt;
            m_detectionOffset = sim(offset);
            m_onJumpCallback = new RayCastCallback(OnJump);
        }
        #endregion

        #region IJumpManager Initialization
        /// <summary>
        /// Updates the manager.
        /// </summary>
        public virtual void Update()
        {

        }
        /// <summary>
        /// Requests for a jump
        /// </summary>
        public virtual void RequestJump()
        {
            // Increased each time "OnJump" is called.
            // It is usefull to avoid 
            //      - Jumping if just one point is raycasted.
            //      - Jumping if more than two points are raycasted.
            m_rayCastJumpSuccesses = 0;
            for (float x = -m_detectionOffset.X; x < m_detectionOffset.X && !(m_rayCastJumpSuccesses > 1); x+=sim(4))
            {
                Globals.World.RayCast(m_onJumpCallback,
                    new Vector2(m_gameEvent.Body.WorldCenter.X + x, m_gameEvent.Body.WorldCenter.Y + m_detectionOffset.Y),
                    new Vector2(m_gameEvent.Body.WorldCenter.X + x, m_gameEvent.Body.WorldCenter.Y + m_detectionOffset.Y + sim(4)));
            }
            if (!(m_rayCastJumpSuccesses > 1))
            {
                m_rayCastJumpSuccesses = 1;
                Globals.World.RayCast(m_onJumpCallback,
                    new Vector2(m_gameEvent.Body.WorldCenter.X - m_detectionOffset.X, m_gameEvent.Body.WorldCenter.Y + m_detectionOffset.Y),
                    new Vector2(m_gameEvent.Body.WorldCenter.X - m_detectionOffset.X - sim(4), m_gameEvent.Body.WorldCenter.Y + m_detectionOffset.Y));
            }
            if (!(m_rayCastJumpSuccesses > 1))
            {
                m_rayCastJumpSuccesses = 1;
                Globals.World.RayCast(m_onJumpCallback,
                    new Vector2(m_gameEvent.Body.WorldCenter.X + m_detectionOffset.X, m_gameEvent.Body.WorldCenter.Y + m_detectionOffset.Y),
                    new Vector2(m_gameEvent.Body.WorldCenter.X + m_detectionOffset.X + sim(4), m_gameEvent.Body.WorldCenter.Y + m_detectionOffset.Y));
            }
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
        public virtual void SetOffset(Vector2 offset)
        {
            m_detectionOffset = ConvertUnits.ToSimUnits(offset);
        }
        #endregion

        /// <summary>
        /// Callback of the raycast : when the up key is pressed, and when there is
        /// something under the player's feet, starts a jump.
        /// </summary>
        float OnJump(Fixture fixture, Vector2 v1, Vector2 v2, float f2)
        {
            m_rayCastJumpSuccesses += 1;
            // If we have not the good amount of success
            // (i.e. 1 : one raycast was successfull)
            if (m_rayCastJumpSuccesses != 2)
                return 0;

            Vector2 point = new Vector2(m_gameEvent.Body.WorldCenter.X, m_gameEvent.Body.WorldCenter.Y);
            if ((int)v2.X == 0)
            {
                if (m_gameEvent.Body.LinearVelocity.Y <= sim(MaxLinearVelocity))
                    m_gameEvent.Body.LinearVelocity = -sim(Vector2.UnitY) * JumpFactorY;
            }
            else
            {
                if ((fixture.Body.FixtureList[0].CollisionCategories & Category.Ground) == Category.Ground)
                {

                    v2 = new Vector2((Math.Sign(v2.X)) * JumpFactorX, sim(-1) * JumpFactorY);
                    if (m_gameEvent.Body.LinearVelocity.Y <= sim(MaxLinearVelocity) &&
                        m_gameEvent.Body.LinearVelocity.X <= sim(MaxLinearVelocity))
                        m_gameEvent.Body.LinearVelocity = v2;
                }
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