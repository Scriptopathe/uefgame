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
    /// <summary>
    /// Class that manages the movement of the events.
    /// </summary>
    public class PlayerMoveManagerOld : IMoveManager
    {
        #region Variables / Properties
        /// <summary>
        /// Event attached to this manager.
        /// </summary>
        GameEvent m_gameEvent;
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
        protected bool m_moveRight;
        protected bool m_moveLeft;
        #endregion

        #region Initialization
        /// <summary>
        /// Constructor.
        /// </summary>
        public PlayerMoveManagerOld(GameEvent evt)
        {
            m_gameEvent = evt;
            MaxSpeed = 5;
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
            // If the player doesn't request move.
            if (isStopped())
            {
                m_gameEvent.Body.LinearVelocity = new Vector2(0, m_gameEvent.Body.LinearVelocity.Y);
            }

            MoveBottom(0.15f);
            if (m_moveRight)
                MoveRight(0.15f);
            if (m_moveLeft)
                MoveLeft(0.15f);

            // Floors position
            m_gameEvent.Body.Position = new Vector2((int)(m_gameEvent.Body.Position.X * 100) / 100.0f,
                                                    (int)(m_gameEvent.Body.Position.Y * 100) / 100.0f);

            m_movingStarted = false;
            m_moveRight = false;
            m_moveLeft = false;
        }
        
        #region Move Bottom
        float _moveBottomFraction;
        /// <summary>
        /// Lets the player move at the bottom with the given speed (sim units).
        /// </summary>
        public virtual void MoveBottom(float speed)
        {
            var aabb = m_gameEvent.BoundingBox;
            var size = aabb.UpperBound - aabb.LowerBound;
            float x = m_gameEvent.Body.Position.X;
            float y = m_gameEvent.Body.Position.Y + size.Y / 2;
            _moveBottomFraction = 1.0f;

            // We do 3 raycast and take the minimum value.
            for (float sx = -size.X / 2 - 0.01f; sx <= size.X / 2 + 0.01f; sx += 0.01f)
            {
                Globals.World.RayCast(new RayCastCallback(MoveBottomRayCast),
                    new Vector2(x + sx, y),
                    new Vector2(x, y + speed));
            }
            if(_moveBottomFraction*speed > 0.01)
                m_gameEvent.Body.LinearVelocity = new Vector2(m_gameEvent.Body.LinearVelocity.X, _moveBottomFraction * speed * _gameTime.ElapsedGameTime.Milliseconds);
            else
                m_gameEvent.Body.LinearVelocity = new Vector2(m_gameEvent.Body.LinearVelocity.X, 0);
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
            if(f2 < _moveBottomFraction)
                _moveBottomFraction = f2;
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
        float _moveLeftFractionDown;
        float _moveLeftFractionFoot;
        /// <summary>
        /// Lets the player move at the bottom with the given speed (sim units).
        /// </summary>
        public virtual void MoveLeft(float speed)
        {
            m_movingStarted = true;
            var aabb = m_gameEvent.BoundingBox;
            var size = aabb.UpperBound - aabb.LowerBound;
            float x = m_gameEvent.Body.Position.X + size.X / 2;
            float y = m_gameEvent.Body.Position.Y + size.Y / 2;
            _moveLeftFractionDown = 1.0f;
            _moveLeftFractionFoot = 1.0f;
            Globals.World.RayCast(new RayCastCallback(MoveLeftRayCastDown),
                new Vector2(x, y),
                new Vector2(x + speed, y));
            float foot_size = 0.06f;
            y -= foot_size; // foot size
            Globals.World.RayCast(new RayCastCallback(MoveLeftRayCastFoot),
                new Vector2(x, y),
                new Vector2(x + speed, y));
            m_gameEvent.Body.LinearVelocity = new Vector2(0.5f, m_gameEvent.Body.LinearVelocity.Y);
            /*
            if (_moveLeftFractionDown > 0.002)
                if (_moveLeftFractionFoot >= 0.95) // rien à monter
                    m_gameEvent.Body.LinearVelocity = new Vector2(_moveLeftFractionDown * speed * _gameTime.ElapsedGameTime.Milliseconds, m_gameEvent.Body.LinearVelocity.Y);
                else
                {
                    // Il y a quelque chose à monter.
                    float newVelY = -foot_size * _gameTime.ElapsedGameTime.Milliseconds * _moveLeftFractionFoot;
                    if (m_gameEvent.Body.LinearVelocity.Y >= newVelY) // si la vélocité est trop faible pour monter.
                    {
                        m_gameEvent.Body.LinearVelocity = new Vector2(_moveLeftFractionDown * speed * _gameTime.ElapsedGameTime.Milliseconds,
                            newVelY);
                    }
                }
            else
                m_gameEvent.Body.LinearVelocity = new Vector2(0, m_gameEvent.Body.LinearVelocity.Y);*/
        }
        /// <summary>
        /// Callback of the raycast : when the up key is pressed, and when there is
        /// something under the player's feet, starts a jump.
        /// </summary>
        float MoveLeftRayCastFoot(Fixture fixture, Vector2 contactPoint, Vector2 normal, float f2)
        {
            if (fixture.Body.UserData is PhysicalObject)
            {
                PhysicalObject obj = (PhysicalObject)(fixture.Body.UserData);
                if (obj.BodyCategory == BodyCategories.NoTouch)
                    return -1; // continue the raycast
            }
            _moveLeftFractionDown = f2;
            return 0;
        }
        /// <summary>
        /// Callback of the raycast : when the up key is pressed, and when there is
        /// something under the player's feet, starts a jump.
        /// </summary>
        float MoveLeftRayCastDown(Fixture fixture, Vector2 contactPoint, Vector2 normal, float f2)
        {
            if (fixture.Body.UserData is PhysicalObject)
            {
                PhysicalObject obj = (PhysicalObject)(fixture.Body.UserData);
                if (obj.BodyCategory == BodyCategories.NoTouch)
                    return -1; // continue the raycast
            }
            _moveLeftFractionFoot = f2;
            return 0;
        }
        #endregion

        #region Move Right
        float _moveRightFractionDown;
        float _moveRightFractionFoot;
        /// <summary>
        /// Lets the player move at the bottom with the given speed (sim units).
        /// </summary>
        public virtual void MoveRight(float speed)
        {
            m_movingStarted = true;
            var aabb = m_gameEvent.BoundingBox;
            var size = aabb.UpperBound - aabb.LowerBound;
            float x = m_gameEvent.Body.Position.X + size.X / 2;
            float y = m_gameEvent.Body.Position.Y + size.Y / 2;
            _moveRightFractionDown = 1.0f;
            _moveRightFractionFoot = 1.0f;
            Globals.World.RayCast(new RayCastCallback(MoveRightRayCastDown),
                new Vector2(x, y),
                new Vector2(x + speed, y));
            float foot_size = 0.06f;
            y -= foot_size; // foot size
            Globals.World.RayCast(new RayCastCallback(MoveRightRayCastFoot),
                new Vector2(x, y),
                new Vector2(x + speed, y));

            if (_moveRightFractionDown > 0.002)
                if (_moveRightFractionFoot >= 0.95) // rien à monter
                    m_gameEvent.Body.LinearVelocity = new Vector2(_moveRightFractionDown * speed * _gameTime.ElapsedGameTime.Milliseconds, m_gameEvent.Body.LinearVelocity.Y);
                else
                {
                    // Il y a quelque chose à monter.
                    if (m_gameEvent.Body.LinearVelocity.Y >= -0.1) // si la vélocité est trop faible pour monter.
                    {
                        m_gameEvent.Body.LinearVelocity = new Vector2(_moveRightFractionDown * speed * _gameTime.ElapsedGameTime.Milliseconds,
                            -foot_size * _gameTime.ElapsedGameTime.Milliseconds * _moveRightFractionFoot);
                    }
                }
            else
                m_gameEvent.Body.LinearVelocity = new Vector2(0, m_gameEvent.Body.LinearVelocity.Y);
        }
        /// <summary>
        /// Callback of the raycast : when the up key is pressed, and when there is
        /// something under the player's feet, starts a jump.
        /// </summary>
        float MoveRightRayCastFoot(Fixture fixture, Vector2 contactPoint, Vector2 normal, float f2)
        {
            if (fixture.Body.UserData is PhysicalObject)
            {
                PhysicalObject obj = (PhysicalObject)(fixture.Body.UserData);
                if (obj.BodyCategory == BodyCategories.NoTouch)
                    return -1; // continue the raycast
            }
            _moveRightFractionDown = f2;
            return 0;
        }
        /// <summary>
        /// Callback of the raycast : when the up key is pressed, and when there is
        /// something under the player's feet, starts a jump.
        /// </summary>
        float MoveRightRayCastDown(Fixture fixture, Vector2 contactPoint, Vector2 normal, float f2)
        {
            if (fixture.Body.UserData is PhysicalObject)
            {
                PhysicalObject obj = (PhysicalObject)(fixture.Body.UserData);
                if (obj.BodyCategory == BodyCategories.NoTouch)
                    return -1; // continue the raycast
            }
            _moveRightFractionFoot = f2;
            return 0;
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
