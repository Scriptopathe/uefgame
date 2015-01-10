using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml.Serialization;
using FarseerPhysics;
using FarseerPhysics.Common;
using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics;

namespace UeFGame.GameObjects
{
    public enum ShapeType
    {
        Rectangle,
        Circle,
        Capsule
    }
    /// <summary>
    /// Data used to initialize the physical object.
    /// </summary>
    public class PhysicalObjectModule : Module
    {
        public PhysicalObjectModule()
            : base()
        {
            ShapeSizeSim = new Vector2(1, 1);
        }
        #region Variables

        #region Shape Initializing Data
        ShapeType m_shapeType = ShapeType.Rectangle;
        /// <summary>
        /// The shape type of the fixture attached to the body.
        /// </summary>
        [Editor.PropertyEdition("Body")]
        public ShapeType ShapeType
        {
            get { return m_shapeType; }
            set { m_shapeType = value; }
        }
        #endregion

        #region Body Initializing Data
        float m_bodyDensity = 1.0f;
        float m_bodyFriction = 0.01f;
        float m_bodyRestitution = 0.01f;
        float m_bodyLinearDamping = 0.0f;
        float m_rotation = 0.0f;
        Vector2 m_fixtureOffset = Vector2.Zero;
        BodyCategories m_bodyCategory;
        BodyType m_bodyType = BodyType.Static;
        /// <summary>
        /// Size of the rectangle or circle shape (if ShapeType equals Rectangle) in 
        /// simulation units.
        /// </summary>
        [Editor.PropertyEdition("Body")]
        public Vector2 ShapeSizeSim { get; set; }
        /// <summary>
        /// Offset of the rotation center.
        /// </summary>
        [Editor.PropertyEdition("Body")]
        public Vector2 CenterOffset { get; set; }

        /// <summary>
        /// The body type of the object.
        /// </summary>
        [Editor.PropertyEdition("Body")]
        public BodyType BodyType
        {
            get { return m_bodyType; }
            set { m_bodyType = value; }
        }
        /// <summary>
        /// Density of the object's body.
        /// </summary>
        [Editor.PropertyEdition("Body")]
        public float BodyDensity
        {
            get { return m_bodyDensity; }
            set { m_bodyDensity = value; }
        }
        /// <summary>
        /// Friction of the object's body.
        /// </summary>
        [Editor.PropertyEdition("Body")]
        public float BodyFriction
        {
            get { return m_bodyFriction; }
            set { m_bodyFriction = value; }
        }
        /// <summary>
        /// Linear damping of the object's body.
        /// </summary>
        [Editor.PropertyEdition("Body")]
        public float BodyLinearDamping
        {
            get { return m_bodyLinearDamping; }
            set { m_bodyLinearDamping = value; }
        }
        /// <summary>
        /// Restitution of the object's body.
        /// </summary>
        [Editor.PropertyEdition("Body")]
        public float BodyRestitution
        {
            get { return m_bodyRestitution; }
            set { m_bodyRestitution = value; }
        }
        /// <summary>
        /// Collision categories of the object
        /// </summary>
        [Editor.PropertyEdition("Body")]
        public BodyCategories BodyCategory
        {
            get { return m_bodyCategory; }
            set { m_bodyCategory = value; }
        }
        /// <summary>
        /// Initial rotation of the object.
        /// </summary>
        [Editor.PropertyEdition("Body")]
        public float Rotation
        {
            get { return m_rotation; }
            set { m_rotation = value; }
        }
        /// <summary>
        /// cf Body.FixedRotation
        /// </summary>
        [Editor.PropertyEdition("Body")]
        public bool IsFixedRotation
        {
            get;
            set;
        }
        #endregion

        #region Overrides

        public override object DeserializeString(string str)
        {
            return Module.DeserializeString<PhysicalObjectModule>(str);
        }

        public override string SerializeString()
        {
            return base.SerializeString(typeof(PhysicalObjectModule));
        }
        public override Module DeepCopy()
        {
            PhysicalObjectModule module = new PhysicalObjectModule();
            module.BodyCategory = BodyCategory;
            module.BodyDensity = BodyDensity;
            module.BodyFriction = BodyFriction;
            module.BodyLinearDamping = BodyLinearDamping;
            module.BodyRestitution = BodyRestitution;
            module.BodyType = BodyType;
            module.CenterOffset = CenterOffset;
            module.IsFixedRotation = IsFixedRotation;
            module.Rotation = Rotation;
            module.ShapeSizeSim = ShapeSizeSim;
            module.ShapeType = ShapeType;
            return module;
        }
        #endregion
        #endregion
    }
}
