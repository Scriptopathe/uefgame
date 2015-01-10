using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;
namespace UeFGame.GameObjects
{
    /// <summary>
    /// Ids of the bodies
    /// </summary>
    public enum BodyId
    {
        Normal,
        Down
    }
    /// <summary>
    /// Collection containing the bodies of the event.
    /// </summary>
    public class BodyCollection
    {
        private Dictionary<BodyId, Tuple<Body, Vector2, Vector2>> m_bodies = new Dictionary<BodyId,Tuple<Body,Vector2, Vector2>>();
        /// <summary>
        /// Creates a new collection of player bodies
        /// </summary>
        public BodyCollection()
        {
            m_bodies = new Dictionary<BodyId,Tuple<Body,Vector2, Vector2>>();
        }
        /// <summary>
        /// Adds a body to the collection.
        /// </summary>
        /// <param name="body"></param>
        /// <param name="id"></param>
        public void Add(Body body, BodyId id, Vector2 fixtureOffset, Vector2 sizePx)
        {
            m_bodies.Add(id, new Tuple<Body,Vector2, Vector2>(body, fixtureOffset, sizePx));
        }
        /// <summary>
        /// Clears the body list
        /// </summary>
        public void Clear()
        {
            m_bodies.Clear();
        }
        /// <summary>
        /// Return the body with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Body this[BodyId id]
        {
            get { return m_bodies[id].Item1; }
        }
        /// <summary>
        /// Gets the fixture offset of a body.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Vector2 GetFixtureOffset(BodyId id)
        {
            return m_bodies[id].Item2;
        }
        /// <summary>
        /// Gets the size in pixels of the body.
        /// </summary>
        public Vector2 GetSizePx(BodyId id)
        {
            return m_bodies[id].Item3;
        }
    }
}
