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
using FarseerPhysics.Factories;
using FarseerPhysics.Controllers;
namespace UeFGame.GameObjects
{
    public enum Direction
    {
        Up,
        Down,
        Right,
        Left,
    }
    /// <summary>
    /// Class that manages the movement of the events.
    /// </summary>
    public class PlayerMoveManager : IMoveManager
    {
        #region Variables / Properties
        /// <summary>
        /// Event attached to this manager.
        /// </summary>
        GameEvent m_gameEvent;
        /// <summary>
        /// True if moving process started
        /// </summary>
        protected bool m_movingStarted = false;
        protected float m_movementImpulse = 2f;
        protected float m_maxVelocityX = 5;
        protected float m_brakeFactor = 0.05f;
        protected float m_stopTreshold = 0.015f;
        protected bool m_moveLeft = false;
        protected bool m_moveRight = false;

        protected Vector2 m_movementVelocity;
        protected Vector2 m_lastMovementVelocity;
        protected Vector2 m_groundVelocity;
        #endregion

        #region Initialization
        /// <summary>
        /// Constructor.
        /// </summary>
        public PlayerMoveManager(GameEvent evt)
        {
            m_gameEvent = evt;
            InitializeBody();
        }
        /// <summary>
        /// Initialises the Body / Fixture.
        /// </summary>
        void InitializeBody()
        {

        }
        #endregion

        #region IMoveManager implementation
        GameTime _gameTime;
        /// <summary>
        /// Updates the move manager.
        /// For the player it means no input has come !
        /// </summary>
        public virtual void Update(GameTime time)
        {
            _gameTime = time;

            m_lastMovementVelocity = m_movementVelocity;
            m_movementVelocity = m_gameEvent.Body.LinearVelocity;
            m_groundVelocity = Vector2.Zero;

            // Brake
            if (!m_movingStarted)
            {
                m_movementVelocity.X = m_gameEvent.Body.LinearVelocity.X * m_brakeFactor;
                if (m_movementVelocity.X <= m_stopTreshold)
                    m_movementVelocity.X = 0;
            }

            // Move left or right
            if (m_moveLeft)
                MoveLeftVelocity();
            else if (m_moveRight)
                MoveRightVelocity();

            // Velocity correction
            if (m_movementVelocity.X <= -m_maxVelocityX)
            {
                m_movementVelocity.X = -m_maxVelocityX;
            }
            else if (m_movementVelocity.X >= m_maxVelocityX)
            {
                m_movementVelocity.X = m_maxVelocityX;
            }

            // Stick to ground
            StickToGround();

            // Apply calculations
            m_gameEvent.Body.LinearVelocity = m_movementVelocity + m_groundVelocity;

            m_gameEvent.Body.Position = Truncate(m_gameEvent.Body.Position);
            m_movingStarted = false;
            m_moveLeft = false;
            m_moveRight = false;
        }

        /// <summary>
        /// Truncates the given vector to a pixel of precision.
        /// </summary>
        /// <param name="vect"></param>
        /// <returns></returns>
        Vector2 Truncate(Vector2 vect)
        {
            return new Vector2((int)(vect.X * 100) / 100.0f + 0.005f, (int)(vect.Y * 100) / 100.0f + 0.005f);
        }

        #region Stick to ground
        float _moveFraction;

        /// <summary>
        /// Lets the player move at the bottom with the given speed (sim units).
        /// </summary>
        public virtual void StickToGround()
        {
            var aabb = m_gameEvent.BoundingBox;
            var size = aabb.UpperBound - aabb.LowerBound;
            float x = m_gameEvent.Body.Position.X;
            float y = m_gameEvent.Body.Position.Y + size.Y / 2;
            
            // We do 3 raycast and take the minimum value.
            for (float sx = -size.X / 2 - 0.01f; sx <= size.X / 2 + 0.01f; sx += 0.04f)
            {
                Globals.World.RayCast(new RayCastCallback(MoveBottomRayCast),
                    new Vector2(x + sx, y),
                    new Vector2(x + sx, y + 0.10f));
                if (m_groundVelocity != Vector2.Zero)
                    break;
            }

            
        }
        /// <summary>
        /// Callback of the raycast : when the up key is pressed, and when there is
        /// something under the player's feet, starts a jump.
        /// </summary>
        float MoveBottomRayCast(Fixture fixture, Vector2 contactPoint, Vector2 normal, float f2)
        {
            if (fixture.Body.UserData is PhysicalObject)
            {
                PhysicalObject obj = (PhysicalObject)(fixture.Body.UserData);
                if (obj.BodyCategory == BodyCategories.NoTouch)
                    return -1; // continue the raycast
                
            }
            m_groundVelocity = fixture.Body.LinearVelocity;
            return 0;
        }
        #endregion


        #region Move Left
        /// <summary>
        /// Moves the object to the left
        /// </summary>
        public virtual void MoveLeft()
        {
            m_movingStarted = true;
            m_moveLeft = true;
        }
        /// <summary>
        /// Lets the player move at the bottom with the given speed (sim units).
        /// </summary>
        void MoveLeftVelocity()
        {
            m_movementVelocity.X -= m_movementImpulse;
        }
        /// <summary>
        /// Callback of the raycast : when the up key is pressed, and when there is
        /// something under the player's feet, starts a jump.
        /// </summary>
        float MoveLeftRayCast(Fixture fixture, Vector2 contactPoint, Vector2 normal, float f2)
        {
            if (fixture.Body.UserData is PhysicalObject)
            {
                PhysicalObject obj = (PhysicalObject)(fixture.Body.UserData);
                if (obj.BodyCategory == BodyCategories.NoTouch)
                    return -1; // continue the raycast

            }
            _moveFraction = f2;
            return 0;
        }
        #endregion

        #region Move Right
        /// <summary>
        /// Lets the player move at the bottom with the given speed (sim units).
        /// </summary>
        void MoveRightVelocity()
        {
            //m_gameEvent.Body.ApplyLinearImpulse(new Vector2(m_movementImpulse, 0));
            m_movementVelocity.X += m_movementImpulse;
        }
        /// <summary>
        /// Moves the object to the right.
        /// </summary>
        public virtual void MoveRight()
        {
            m_moveRight = true;
            m_movingStarted = true;
        }

        #endregion
        /// <summary>
        /// Resets the move manager.
        /// </summary>
        public virtual void Reset()
        {
            m_movingStarted = false;
            m_gameEvent.Body.LinearVelocity = new Vector2(0.0f, 0.0f);
        }
        #endregion

        /// <summary>
        /// Returns true if the object can be considered as stopped (too slow)
        /// </summary>
        /// <returns></returns>
        bool isStopped()
        {
            return !m_movingStarted;
        }
    }
}
