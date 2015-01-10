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
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision;
using FarseerPhysics.Common;
using System.Xml.Serialization;
namespace UeFGame.GameObjects
{
    public enum ShapeTypes
    {
        Rectangle,
        Circle,
        Polygon
    }
    /// <summary>
    /// Initializing Data for a UniqueBodyGameObject
    /// </summary>
    [XmlInclude(typeof(Vector2))]
    [XmlInclude(typeof(Color))]
    [XmlInclude(typeof(Vertices))]
    [XmlInclude(typeof(BodyType))]
    [XmlInclude(typeof(Category))]
    public class UniqueBodyGameObjectInitializingData : GameObjectInitializingData
    {
        
        #region Variables
        public string TextureName;
        public Color Tone = Color.White;
        /// <summary>
        /// Size in pixels of the body's fixture.
        /// If the shape is not "rectangle" or "circle", it will only be used
        /// to get the 100% size of the texture.
        /// </summary>
        public Vector2 SizePx = new Vector2(50);
        /// <summary>
        /// Offset of the fixture on the body.
        /// </summary>
        public Vector2 FixtureOffset = Vector2.Zero;
        /// <summary>
        /// The shape type of the fixture attached to the body.
        /// </summary>
        public ShapeTypes ShapeType = ShapeTypes.Rectangle;
        /// <summary>
        /// The vertices of the shape if the ShapeType is polygon.
        /// </summary>
        public Vertices ShapePolygonVertices;
        /// <summary>
        /// The body type of the object.
        /// </summary>
        public BodyType BodyType = BodyType.Static;
        /// <summary>
        /// Density of the object's body.
        /// </summary>
        public float BodyDensity = 1.0f;
        /// <summary>
        /// Friction of the object's body.
        /// </summary>
        public float BodyFriction = 0.01f;
        /// <summary>
        /// Linear damping of the object's body.
        /// </summary>
        public float BodyLinearDamping = 0.0f;
        /// <summary>
        /// Restitution of the object's body.
        /// </summary>
        public float BodyRestitution = 0.01f;
        /// <summary>
        /// Initialial rotation of the object.
        /// </summary>
        public float Rotation = 0.00f;
        /// <summary>
        /// Collision categories of the object
        /// </summary>
        public Category CollisionCategories = Category.Object;
        /// <summary>
        /// Gets or sets the UpperRight corner start x.
        /// The FixtureOffset and Size must have been set before using this method.
        /// The given value must be in simulation units.
        /// </summary>
        public float UpperRightStartX
        {
            get { return SimStartX - ConvertUnits.ToSimUnits(SizePx.X / 2.0f) + FixtureOffset.X; }
            set { SimStartX = ConvertUnits.ToSimUnits(SizePx.X / 2) - FixtureOffset.X + value; }
        }
        /// <summary>
        /// Gets or sets the UpperRight corner start .
        /// The FixtureOffset and Size must have been set before using this method.
        /// The given value must be in simulation units.
        /// </summary>
        public float UpperRightStartY
        {
            get { return SimStartY - ConvertUnits.ToSimUnits(SizePx.Y / 2.0f) + FixtureOffset.Y; }
            set { SimStartY = ConvertUnits.ToSimUnits(SizePx.Y / 2) - FixtureOffset.Y + value; }
        }
        // Add body properties
        #endregion
        /// <summary>
        /// Creates a new instance of UniqueBodyGameObjectInitializingData.
        /// </summary>
        public UniqueBodyGameObjectInitializingData()
            : base()
        {

        }
    }
}
