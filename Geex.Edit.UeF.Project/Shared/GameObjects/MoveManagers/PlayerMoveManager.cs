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
namespace UeFGame.GameObjects.MoveManagers
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
        BasicEvent m_gameEvent;
        /// <summary>
        /// Acceleration in pixels / frame / second
        /// </summary>
        public float Acceleration = 10.0f;
        /// <summary>
        /// Brake force in Newtons/Kg-1
        /// </summary>
        public float BrakeForce = 1.5f;
        /// <summary>
        /// Sets the max speed in pixels / frame
        /// </summary>
        public float MaxSpeed
        {
            set
            {
                // input : pixel/frame ; output : m/s
                m_maxSpeedSim = ConvertUnits.ToSimUnits(value) * (60.0f);
            }
        }
        /// <summary>
        /// The object will be considered as stopped with less than this velocity
        /// </summary>
        protected float m_stoppedVelocitySim = 1.0f;
        /// <summary>
        /// Start impulse force.
        /// </summary>
        protected float m_startSpeedSim = 1.0f;
        /// <summary>
        /// Max speed in simulation units.
        /// </summary>
        protected float m_maxSpeedSim = 1.0f;
        /// <summary>
        /// True if moving process started
        /// </summary>
        protected bool m_movingStarted = false;
        #endregion

        #region Initialization
        /// <summary>
        /// Constructor.
        /// </summary>
        public PlayerMoveManager(BasicEvent evt)
        {
            m_gameEvent = evt;
            MaxSpeed = 5;
        }
        #endregion

        #region IMoveManager implementation
        /// <summary>
        /// Updates the move manager.
        /// For the player it means no input has come !
        /// </summary>
        public virtual void Update()
        {
            // Not stopped and has not called MoveLeft or MoveRight
            if (!isStopped() && !m_movingStarted)
            {
                int coef = -Math.Sign(m_gameEvent.Body.LinearVelocity.X);
                m_gameEvent.Body.ApplyForce(new Microsoft.Xna.Framework.Vector2(coef * BrakeForce, 0.0f));
            }
            m_movingStarted = false;
        }
        /// <summary>
        /// Moves the object to the left
        /// </summary>
        public virtual void MoveLeft()
        {
            if (isStopped())
                m_gameEvent.Body.ApplyLinearImpulse(new Vector2(-GetStartImpulse(), 0.0f));
            if(m_gameEvent.Body.LinearVelocity.X >= -m_maxSpeedSim)
                m_gameEvent.Body.ApplyForce(new Microsoft.Xna.Framework.Vector2(-GetMoveForce(), 0));
            m_movingStarted = true;
        }
        /// <summary>
        /// Moves the object to the right.
        /// </summary>
        public virtual void MoveRight()
        {
            if (isStopped())
                m_gameEvent.Body.ApplyLinearImpulse(new Vector2(GetStartImpulse(), 0.0f));
            if(m_gameEvent.Body.LinearVelocity.X <= m_maxSpeedSim)
                m_gameEvent.Body.ApplyForce(new Microsoft.Xna.Framework.Vector2(GetMoveForce(), 0));
            m_movingStarted = true;
        }
        /// <summary>
        /// Resets the move manager.
        /// </summary>
        public virtual void Reset()
        {
            m_movingStarted = true;
            m_gameEvent.Body.LinearVelocity = new Vector2(0.0f, 0.0f);
        }
        #endregion

        /// <summary>
        /// Returns true if the object can be considered as stopped (too slow)
        /// </summary>
        /// <returns></returns>
        bool isStopped()
        {
            return Math.Abs(m_gameEvent.Body.LinearVelocity.X) < m_stoppedVelocitySim;
        }
        /// <summary>
        /// Gets the force used for acceleration.
        /// </summary>
        float GetMoveForce()
        {
            // Force in Newtons
            float force = m_gameEvent.Body.Mass;
            force *= Acceleration;
            return force;
        }
        /// <summary>
        /// Gets the impulse which starts the moving.
        /// </summary>
        float GetStartImpulse()
        {
            float impulse = m_gameEvent.Body.Mass;
            impulse *= m_startSpeedSim;
            return impulse;
        }


    }
}
