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
        Sneak
    }
    /// <summary>
    /// Collection containing the bodies of the event.
    /// </summary>
    public class BodyCollection
    {
        private Dictionary<BodyId, Tuple<Body, Vector2, Vector2, ShapeType>> m_bodies;
        /// <summary>
        /// Creates a new collection of player bodies
        /// </summary>
        public BodyCollection()
        {
            m_bodies = new Dictionary<BodyId,Tuple<Body,Vector2, Vector2, ShapeType>>();
        }
        /// <summary>
        /// Adds a body to the collection.
        /// </summary>
        /// <param name="body"></param>
        /// <param name="id"></param>
        public void Add(Body body, BodyId id, Vector2 fixtureOffset, Vector2 shapeSizeSim, ShapeType shapeType)
        {
            // In editor, we update the body when refreshed.
            if (Globals.ExecuteInEditor)
            {
                if (m_bodies.ContainsKey(id))
                {
                    m_bodies[id].Item1.Dispose();
                    m_bodies.Remove(id);
                }
            }

            m_bodies.Add(id, new Tuple<Body,Vector2, Vector2, ShapeType>(body, fixtureOffset, shapeSizeSim, shapeType));
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
        public Vector2 GetCenterOffset(BodyId id)
        {
            return m_bodies[id].Item2;
        }
        /// <summary>
        /// Gets the size in pixels of the body.
        /// </summary>
        public Vector2 GetShapeSizeSim(BodyId id)
        {
            return m_bodies[id].Item3;
        }
        /// <summary>
        /// Gets the shape type of the body given by its id.
        /// </summary>
        public ShapeType GetShapeType(BodyId id)
        {
            return m_bodies[id].Item4;
        }
    }
}
