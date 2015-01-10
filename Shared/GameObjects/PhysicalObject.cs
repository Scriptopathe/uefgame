using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision;
using FarseerPhysics.Common;
using System.Xml.Serialization;
using UeFGame.GameObjects;
using UeFGame.GameComponents;
using UeFGame.Editor;
namespace UeFGame.GameObjects
{

    /// <summary>
    /// Base class for the PhysicalObjects.
    /// </summary>
    [Editor.EditableGameObject(
        new string[] {"base", "physical_object"},
        new Type[] { typeof(BaseModule), typeof(PhysicalObjectModule)})]
    public class PhysicalObject : GameObject
    {
        /* ------------------------------------------------------------------------------------
         * Constants
         * ----------------------------------------------------------------------------------*/
        #region Constants
        const int ELLIPSE_EDGES_COUNT = 20;
        #endregion
        /* ------------------------------------------------------------------------------------
         * Variables
         * ----------------------------------------------------------------------------------*/
        #region Variables
        /// <summary>
        /// The physical object's body.
        /// </summary>
        protected Body m_body;

        #endregion
        /* ------------------------------------------------------------------------------------
         * Properties
         * ----------------------------------------------------------------------------------*/
        #region Properties
        /// <summary>
        /// Access to the physical object module.
        /// </summary>
        public PhysicalObjectModule MPhysicalObject
        {
            get;
            protected set;
        }
        /// <summary>
        /// Category of the body.
        /// </summary>
        public BodyCategories BodyCategory
        {
            get;
            set;
        }
        /// <summary>
        /// X-coordinate of the body center in simulation units.
        /// </summary>
        public override float SimX
        {
            get { return m_body.Position.X; }
            set { m_body.Position = new Vector2(value, m_body.Position.Y); }
        }
        /// <summary>
        /// Y coordinate of the body center in simulation units
        /// </summary>
        public override float SimY
        {
            get { return m_body.Position.Y; }
            set { m_body.Position = new Vector2(m_body.Position.X, value); }
        }
        /// <summary>
        /// Gets the ShapeSize of the current Body of this event (cf in PhysicalObjectInit) in simulation units.
        /// </summary>
        public virtual Vector2 ShapeSizeSim
        {
            get { return MPhysicalObject.ShapeSizeSim; }
        }
        /// <summary>
        /// Gets the CenterOffset of the current Body of this event (cf in PhysicalObjectInit) in simulation units.
        /// </summary>
        public virtual Vector2 CenterOffset
        {
            get { return MPhysicalObject.CenterOffset; }
        }
        /// <summary>
        /// Gets the ShapeType of the current Body of this event.
        /// </summary>
        public virtual ShapeType ShapeType
        {
            get { return MPhysicalObject.ShapeType; }
        }
        /// <summary>
        /// Physical Body of the Object.
        /// </summary>
        public Body Body
        {
            get { return m_body; }
            set { m_body = value; }
        }
        /// <summary>
        /// AABB instance used in BoundingBox property.
        /// </summary>
        AABB bb_aabb = new AABB();
        /// <summary>
        /// Gets the BoundingBox of the GameObject.
        /// This is mostly used in the editor.
        /// </summary>
        public override AABB BoundingBox { 
            get {
                if (Globals.ExecuteInEditor)
                {
                    Body.FixtureList[0].GetAABB(out bb_aabb, 0);
                    Vector2 offset = -Body.Position + MBase.SimPosition;
                    return new AABB(bb_aabb.LowerBound + offset, bb_aabb.UpperBound + offset);
                }
                else
                {
                    Body.FixtureList[0].GetAABB(out bb_aabb, 0);
                    return bb_aabb;
                }
            } 
        }
        /// <summary>
        /// Indicates whether or not the object's body must be reloaded when
        /// it gets reinitialised.
        /// </summary>
        public virtual bool NeedBodyReload
        {
            get;
            set;
        }
        #endregion
        /* ------------------------------------------------------------------------------------
         * Basic methods
         * ----------------------------------------------------------------------------------*/
        #region Basic Methods
        /* ------------------------------------------------------------------------------------
         * Init methods
         * ----------------------------------------------------------------------------------*/
        #region Init
        /// <summary>
        /// Initializes this object using the provided InitializingData.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            MPhysicalObject = (PhysicalObjectModule)Modules["physical_object"];

            // Destroys the body if we are reinitializing the object in the editor.
            if (Globals.ExecuteInEditor && m_body != null)
                m_body.Dispose();

            if (NeedBodyReload | m_body == null || Globals.ExecuteInEditor)
            {
                m_body = InitBody(MPhysicalObject);
                // Now create a fixture
                InitializeShape(MPhysicalObject, m_body);
            }

            m_body.UserData = this;
            m_body.Position = Modules.Base.SimPosition;
            m_body.Enabled = true;
        }
        /// <summary>
        /// Initializes this object's body :
        /// </summary>
        protected virtual Body InitBody(PhysicalObjectModule init)
        {
            Body body;
            body = new Body(Globals.World);
            body.BodyType = init.BodyType;
            body.Restitution = init.BodyRestitution;
            body.Friction = init.BodyFriction;
            body.LinearDamping = init.BodyLinearDamping;
            body.Rotation = init.Rotation;
            body.FixedRotation = init.IsFixedRotation;
            body.UserData = this;
            body.Position = MBase.SimPosition;
            BodyCategory = init.BodyCategory;
            return body;
        }

        /// <summary>
        /// Initialize the shape of the given body, using a PhysicalObjectInit object.
        /// </summary>
        protected void InitializeShape(PhysicalObjectModule init, Body body)
        {
            InitializeShape(init.ShapeType, init.ShapeSizeSim, init.BodyDensity, init.CenterOffset, body);
        }
        /// <summary>
        /// Initialize the shape of the given body, using some information about the shape.
        /// </summary>
        protected virtual void InitializeShape(ShapeType shapeType, Vector2 shapeSizeSim, float bodyDensity, Vector2 centerOffset, Body body)
        {
            switch (shapeType)
            {
                case ShapeType.Rectangle:
                    FixtureFactory.AttachRectangle(shapeSizeSim.X, shapeSizeSim.Y,
                        bodyDensity, centerOffset, body);
                    break;
                case ShapeType.Circle:
                    FixtureFactory.AttachEllipse(shapeSizeSim.X / 2, shapeSizeSim.Y / 2, ELLIPSE_EDGES_COUNT,
                        bodyDensity, body);
                    break;
                case ShapeType.Capsule:
                    var vertices =  FarseerPhysics.Common.Decomposition.BayazitDecomposer.ConvexPartition(
                        PolygonTools.CreateCapsule(shapeSizeSim.Y, shapeSizeSim.Y / 8, 10));
                    for (int i = 0; i < vertices.Count; i++)
                    {
                        vertices[i].Translate(centerOffset);
                    }
                    FixtureFactory.AttachCompoundPolygon(vertices, bodyDensity, body);
                    break;
                /*case ShapeType.Polygon:
                    var vertices = FarseerPhysics.Common.Decomposition.BayazitDecomposer.ConvexPartition(data.ShapePolygonVertices);
                    FixtureFactory.AttachCompoundPolygon(vertices, 1.0f, m_body);
                    break;*/
            }
            body.OnCollision += new OnCollisionEventHandler(OnCollision);
            body.OnSeparation += new OnSeparationEventHandler(OnSeparation);
        }


        #endregion

        /// <summary>
        /// This method is called once per frame (if IsActive is set to true)
        /// and is the place where the Game Object's logic should be updated.
        /// </summary>
        /// <param name="time"></param>
        public override void Update(GameTime time)
        {
            
        }

        /// <summary>
        /// This method is called once per frame (if IsVisible is set to true)
        /// and is the place to draw the Game object.
        /// </summary>
        /// <param name="time">The current gametime</param>
        /// <param name="batch">The batch used to draw the graphics</param>
        /// <param name="scroll">Current scrolling of the map in pixels</param>
        public override void Draw(GameTime time, SpriteBatch batch, Vector2 scroll)
        {
            
        }

        /// <summary>
        /// Disposes the GameObject.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            this.Body.Dispose();
        }
        #endregion
        /* ------------------------------------------------------------------------------------
         * Callbacks
         * ----------------------------------------------------------------------------------*/
        #region Callbacks
        /// <summary>
        /// Called when a collision happens.
        /// </summary>
        /// <param name="fixtureA">This body's fixture</param>
        /// <param name="fixtureB">The other body's fixture</param>
        /// <param name="contact">A contact object.</param>
        /// <returns>True to acknowledge the collision, false to ignore.</returns>
        protected virtual bool OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            if (fixtureB.Body.UserData is BodyCategories)
            {
                // C'est un mur
                return true;
            }
            if (fixtureB.Body.UserData is PhysicalObject)
            {
                PhysicalObject obj = (PhysicalObject)(fixtureB.Body.UserData);
                if (obj.BodyCategory == BodyCategories.NoTouch)
                    return false;
            }
            return true;
        }
        /// <summary>
        /// Called when a body separation happens.
        /// </summary>
        protected virtual void OnSeparation(Fixture fixtureA, Fixture fixtureB)
        {
            
        }
        #endregion
        /* ------------------------------------------------------------------------------------
         * Editor
         * ----------------------------------------------------------------------------------*/
        #region Editor
        /* ------------------------------------------------------------------------------------
         * Colors - Editor Drawing
         * ----------------------------------------------------------------------------------*/
        #region Colors - Editor Drawing
        /// <summary>
        /// Constant indicating the border size where the user is able to 
        /// resize or rotate the object.
        /// </summary>
        const int ResizeRotateBorderSize = 10;
        Color _selectedPointColor = Color.Red;
        Color _pointColor = Color.White;
        Color _polygonSegColor = Color.White;
        Color _boundingBoxColor = Color.Green;
        Color _rotateLineColor = Color.DarkRed;
        Color _resizeLineColor = Color.DarkBlue;
        #endregion
        /* ------------------------------------------------------------------------------------
         * Editor vars
         * ----------------------------------------------------------------------------------*/
        #region Editor vars
        bool _rotationStarted = false;
        #endregion
        /* ---------------------------------------------------------------
         * Draw In Editor
         * -------------------------------------------------------------*/
        #region Draw In Editor
        /// <summary>
        /// Draws the shape of the GameObject in editor.
        /// </summary>
        /// <param name="shapeData"></param>
        protected void DrawShapeInEditor(SpriteBatch batch, Vector2 scroll,
            Editor.GameObjectRenderOptions ops, float layerDepth)
        {
            int i;
            switch (ShapeType)
            {
                case ShapeType.Rectangle:
                    Vector2[] points = GetRectangleShapePoints();
                    points[0] *= ops.Zoom;
                    for (i = 1; i < points.Count(); i++)
                    {
                        points[i].X *= ops.Zoom;
                        points[i].Y *= ops.Zoom;
                        DrawingRoutines.DrawLine(batch, disp(points[i - 1]) - scroll, disp(points[i]) - scroll, layerDepth, _polygonSegColor);
                    }
                    DrawingRoutines.DrawLine(batch, disp(points[points.Count() - 1]) - scroll, disp(points[0]) - scroll, layerDepth, _polygonSegColor);
                    break;
                case ShapeType.Circle:
                    // todo
                    break;
            }
        }
        /// <summary>
        /// Draws the bounding box of the GameObject in the editor.
        /// </summary>
        protected void DrawBoundingBoxInEditor(SpriteBatch batch, Vector2 scroll,
            Editor.GameObjectRenderOptions ops, float layerDepth)
        {
            // For later use
            AABB boundingbox = BoundingBox;
            // Draws the outline for the "commands" like resizing etc...
            if (ops.IsSelected)
            {
                // Draws the bounding box
                AABB aabb = boundingbox;
                Rectangle rectDraw = new Rectangle(
                    (int)disp(aabb.LowerBound.X * ops.Zoom) - (int)scroll.X,
                    (int)disp(aabb.LowerBound.Y * ops.Zoom) - (int)scroll.Y,
                    (int)(disp(aabb.UpperBound.X - aabb.LowerBound.X) * ops.Zoom),
                    (int)(disp(aabb.UpperBound.Y - aabb.LowerBound.Y) * ops.Zoom));
                DrawingRoutines.DrawRectangle(batch, rectDraw, layerDepth + 0.001f, _boundingBoxColor);

                // Draws the line indicating we can rotate the object :
                DrawingRoutines.DrawLine(batch,
                    new Vector2(rectDraw.Left, rectDraw.Top + ResizeRotateBorderSize),
                    new Vector2(rectDraw.Right, rectDraw.Top + ResizeRotateBorderSize),
                    layerDepth, _rotateLineColor);
                // Draws the line indicating we can resize the object.
                DrawingRoutines.DrawLine(batch,
                    new Vector2(rectDraw.Right - ResizeRotateBorderSize, rectDraw.Bottom - ResizeRotateBorderSize),
                    new Vector2(rectDraw.Right, rectDraw.Bottom - ResizeRotateBorderSize),
                    layerDepth, _resizeLineColor);
                DrawingRoutines.DrawLine(batch,
                    new Vector2(rectDraw.Right - ResizeRotateBorderSize, rectDraw.Bottom - ResizeRotateBorderSize),
                    new Vector2(rectDraw.Right - ResizeRotateBorderSize, rectDraw.Bottom),
                    layerDepth, _resizeLineColor);
            }

            // We will draw the line between the center of rotation and the center of the boundingbox
            if (_rotationStarted)
            {
                Vector2 center = disp(GetCenterOfRotation()) * ops.Zoom - scroll;
                Vector2 bbCenter = disp(boundingbox.Center) * ops.Zoom - scroll;
                DrawingRoutines.DrawLine(batch, center, bbCenter, layerDepth, Color.White);
            }
        }
        /// <summary>
        /// Draws the object in the editor.
        /// </summary>
        /// <param name="batch">batch where to draw the object</param>
        /// <param name="scroll">scrolling of the object in pixels</param>
        /// <param name="ops">RenderOptions</param>
        public override void DrawInEditor(SpriteBatch batch, Vector2 scroll, Editor.GameObjectRenderOptions ops)
        {   
            // Choose layer depth of the debug views (lines etc...)
            float layerDepth = ops.DrawOnTop ? 0.9f : MBase.Z + 0.0000001f;

            // Bounding box
            DrawBoundingBoxInEditor(batch, scroll, ops, layerDepth);

            // Here we draw the Shape
            DrawShapeInEditor(batch, scroll, ops, layerDepth);
        }
        #endregion

        bool isResizing = false;
        bool isRotation = false;
        /// <summary>
        /// Action performed when the object is clicked in editor.
        /// </summary>
        /// <param name="simPos">Mouse position in simulation units. Already "zoomed"</param>
        /// <param name="redoAction">Delegate which redoes the action performed by this call.</param>
        /// <param name="undoAction">Delegate which undoes the action performed by this call.</param>
        /// <returns>True if the object needs to receive OnMouseMoveEditor and OnMouseUpInEditor calls for a drag operation.</returns>
        public override ActionType OnMouseDownInEditor(Vector2 simPos, out EditorActionDelegate undoAction, out EditorActionDelegate redoAction)
        {
            /*
            // Handles the actions of the base class.
            bool retValue = base.OnMouseDownInEditor(simPos, out undoAction, out redoAction);
            if(retValue)
                return true;
            
            
             * */

            // Draws the bounding boxs
            AABB aabb = BoundingBox;
            Rectangle rectDraw = new Rectangle(
                (int)disp(aabb.LowerBound.X),
                (int)disp(aabb.LowerBound.Y),
                (int)disp(aabb.UpperBound.X - aabb.LowerBound.X),
                (int)disp(aabb.UpperBound.Y - aabb.LowerBound.Y));

            // Point cliqué
            Point dispPt = new Point((int)disp(simPos.X), (int)disp(simPos.Y));
            Microsoft.Xna.Framework.Rectangle rotateArea = new Rectangle(rectDraw.Left, rectDraw.Top, rectDraw.Width, ResizeRotateBorderSize);
            Microsoft.Xna.Framework.Rectangle resizeArea = new Rectangle(rectDraw.Right - ResizeRotateBorderSize, rectDraw.Bottom - ResizeRotateBorderSize,
                                                                        ResizeRotateBorderSize, rectDraw.Bottom - ResizeRotateBorderSize);

            // Starts the rotation of the object.
            if (rotateArea.Contains(dispPt))
            {
                float oldValue = MPhysicalObject.Rotation;
                float currentValue = MPhysicalObject.Rotation;
                isRotation = true;
                undoAction = new EditorActionDelegate(() => { MPhysicalObject.Rotation = oldValue; });
                redoAction = new EditorActionDelegate(() => { MPhysicalObject.Rotation = currentValue; });
                return ActionType.Drag;
            }
            // Starts the resizing of the object.
            else if (resizeArea.Contains(dispPt))
            {
                Vector2 oldValue = MPhysicalObject.ShapeSizeSim;
                undoAction = new EditorActionDelegate(() => { MPhysicalObject.ShapeSizeSim = oldValue; });
                redoAction = new EditorActionDelegate(() => { MPhysicalObject.ShapeSizeSim = oldValue; });
                isResizing = true;
                return ActionType.Drag;
            }
            undoAction = new EditorActionDelegate(() => { return; });
            redoAction = new EditorActionDelegate(() => { return; });
            return ActionType.None;
        }
        /// <summary>
        /// Action performed when a point is dragging in editor.
        /// </summary>
        /// <param name="simPos">Current mouse position in Sim units</param>
        /// <param name="delta">Delta of the mouse position at last frames</param>
        public override void OnMouseMoveEditor(Vector2 simPos, Vector2 delta)
        {
            if (isRotation)
            {
                // Applies rotation on the object.
                MPhysicalObject.Rotation += delta.Y*5;
            }
            else if (isResizing)
            {
                // Translates the screen-space coordinnates of the mouse to object-space (with rotation) coordinates.
                Vector2 factorX = new Vector2((float)Math.Cos(MPhysicalObject.Rotation), (float)Math.Sin(MPhysicalObject.Rotation));
                Vector2 factorY = new Vector2((float)Math.Sin(MPhysicalObject.Rotation), (float)Math.Cos(MPhysicalObject.Rotation));
                MPhysicalObject.ShapeSizeSim += factorX * delta.X + factorY * delta.Y;
            }
            
        }
        /// <summary>
        /// Action performed when the object's gradding is finished and the action performed must be recorded.
        /// </summary>
        /// <param name="simPos">Mouse position in simulation units. Already "zoomed"</param>
        /// <param name="redoAction">Delegate which redoes the action performed by this call.</param>
        public override void OnMouseUpInEditor(Vector2 simPos, out EditorActionDelegate redoAction)
        {
            if (isResizing)
            {
                float value = MPhysicalObject.Rotation;
                redoAction = new EditorActionDelegate(() =>
                {
                    MPhysicalObject.Rotation = value;
                });
                isRotation = false;
            }
            else if (isRotation)
            {
                Vector2 value = MPhysicalObject.ShapeSizeSim;
                redoAction = new EditorActionDelegate(() =>
                {
                    MPhysicalObject.ShapeSizeSim = value;
                });
                isResizing = false;
            }
            else
            {
                redoAction = new EditorActionDelegate(() => { return; });
            }
            isResizing = false;
            isRotation = false;
        }
        /// <summary>
        /// Action performed when the object is selected an a key is pressed.
        /// Returns true if the key is captured, false otherwise.
        /// </summary>
        /// <param name="simPos">Position of the mouse in sim units (zoomed)</param>
        /// <param name="zoom">Zoom of the editor</param>
        /// <param name="modifiedProperty">The property modified during this method.</param>
        /// <param name="keys">Keys pressed.</param>
        /// <param name="newValue">The new value of the modified property.</param>
        /// <param name="oldValue">The old value of the modified property.</param>
        /// <returns>True if the stroke was captured.</returns>
        public override bool OnKeyDownInEditor(Vector2 simPos, out EditorActionDelegate undoAction, out EditorActionDelegate redoAction, System.Windows.Forms.Keys keys)
        {
            undoAction = new EditorActionDelegate(() => { return; });
            redoAction = new EditorActionDelegate(() => { return; });
            return false;
        }
        #endregion
        /* ------------------------------------------------------------------------------------
         * Utils
         * ----------------------------------------------------------------------------------*/
        #region Utils
        /// <summary>
        /// Gets the points (including rotation) which define the rectangle of the object.
        /// (sim units)
        /// Only in editor mode.
        /// </summary>
        /// <returns></returns>
        Vector2[] GetRectangleShapePoints()
        {
            var init = MPhysicalObject;
            if (Globals.ExecuteInEditor && MPhysicalObject.ShapeType == ShapeType.Rectangle)
            {
                // Rectangle containing the shape in sim units.
                float x = MBase.SimStartX - ShapeSizeSim.X / 2.0f + CenterOffset.X;
                float y = MBase.SimStartY - ShapeSizeSim.Y / 2.0f + CenterOffset.Y;
                float w = ShapeSizeSim.X;
                float h = ShapeSizeSim.Y;
                // Rotate the rectangle from the center of rotation.
                Vector2[] arr = new Vector2[4];
                if (init.Rotation == 0.0f)
                {
                    arr[0] = new Vector2(x, y);
                    arr[1] = new Vector2(x + w, y);
                    arr[2] = new Vector2(x + w, y + h);
                    arr[3] = new Vector2(x, y + h);
                }
                else
                {
                    // Center of rotation
                    float cx = x + w / 2 - CenterOffset.X;
                    float cy = y + h / 2 - CenterOffset.Y;
                    Vector2 center = new Vector2(cx, cy);
                    arr[0] = GeomHelper.RotatePoint(center, new Vector2(x, y), init.Rotation);
                    arr[1] = GeomHelper.RotatePoint(center, new Vector2(x + w, y), init.Rotation);
                    arr[2] = GeomHelper.RotatePoint(center, new Vector2(x + w, y + h), init.Rotation);
                    arr[3] = GeomHelper.RotatePoint(center, new Vector2(x, y + h), init.Rotation);
                }
                return arr;
            }
            else
            {
                throw new NotSupportedException("This method is only supported in Editor mode and with the Rectangle shapetype");
            }
        }
        /// <summary>
        /// Gets the center of rotation of the object in sim units.
        /// Editor only.
        /// </summary>
        /// <returns></returns>
        Vector2 GetCenterOfRotation()
        {
            if (Globals.ExecuteInEditor)
            {
                var init = MPhysicalObject;
                // Rectangle containing the shape in sim units.
                float x = MBase.SimStartX - MPhysicalObject.ShapeSizeSim.X / 2.0f + init.CenterOffset.X;
                float y = MBase.SimStartY - MPhysicalObject.ShapeSizeSim.Y / 2.0f + init.CenterOffset.Y;
                float w = sim(MPhysicalObject.ShapeSizeSim.X);
                float h = sim(MPhysicalObject.ShapeSizeSim.Y);
                // Center of rotation
                float cx = x + w / 2 - init.CenterOffset.X;
                float cy = y + h / 2 - init.CenterOffset.Y;
                return new Vector2(cx, cy);
            }
            else
            {
                throw new NotSupportedException("The 'GetCenterOfRotation' method is only available in editor");
            }
        }
        #endregion
    }
}
