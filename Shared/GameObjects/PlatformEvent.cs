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
namespace UeFGame.GameObjects
{

    /// <summary>
    /// Class that handles platform behavior and trajectory editing.
    /// 
    /// It enables the user to draw a trajectory in the editor.
    /// </summary>
    [Editor.EditableGameObject(
        new string[] { "base", "physical_object", "game_event", "platform"},
        new Type[] { typeof(BaseModule), typeof(PhysicalObjectModule), typeof(GameEventModule), typeof(PlatformEventModule)})]
    public class PlatformEvent : GameEvent
    {
        /* ---------------------------------------------------------------------------------------
         * Variables
         * -------------------------------------------------------------------------------------*/
        #region Variables
        /// <summary>
        /// Id of the trajectory unit currently operating.
        /// </summary>
        int m_currentTrajectoryUnitId;
        #endregion
        /* ---------------------------------------------------------------------------------------
         * Properties
         * -------------------------------------------------------------------------------------*/
        #region Properties
        /// <summary>
        /// Gets or sets the platform event module.
        /// </summary>
        public PlatformEventModule MPlatform
        {
            get;
            set;
        }
        #endregion
        /* ---------------------------------------------------------------------------------------
         * Methods
         * -------------------------------------------------------------------------------------*/
        #region Methods
        /// <summary>
        /// Creates a new instance of GameEvent.
        /// </summary>
        public PlatformEvent()
            : base()
        {
        }

        #region Init
        /// <summary>
        /// Initializes this object using the provided InitializingData.
        /// This call initializes in that order :
        ///     - The Bodies (Physical Object)
        ///     - The Sprite (InitializeSprite)
        ///     - The Event Queue (InitializeQueue)
        ///     - The Scripts (InitializeScript)
        /// These methods can be overriden in subclasses of event.
        /// This one provides general features.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            InitializePlatform();
        }
        /// <summary>
        /// Initialise le module de plateformes.
        /// </summary>
        void InitializePlatform()
        {
            MPlatform = (PlatformEventModule)Modules["platform"];
            m_body.BodyType = BodyType.Kinematic;

            var unit = MPlatform.TrajectoryUnits.First();
            if (unit.Coordinate == PlatformEventModule.TrajectoryUnit.CoordinateMode.Relative)
                m_nextPosition = MPlatform.TrajectoryUnits.First().EndPoint + m_body.Position;
            else
                m_nextPosition = MPlatform.TrajectoryUnits.First().EndPoint;
        }
        /// <summary>
        /// Initialize the Event's command queue.
        /// </summary>
        void InitializeQueue()
        {
            m_commandQueue.Reset();
        }
        /// <summary>
        /// Initialize the Event's script.
        /// </summary>
        void InitializeScript()
        {
            if (!Globals.ExecuteInEditor && Globals.GameMap.Assemblies.ContainsKey(MapId))
            {
                Type t = Globals.GameMap.Assemblies[MapId].GetType("UeFGame.GameObjects.GeneratedCode.Map_" + MapId.ToString());
                string methodName = "Evt_" + MBase.BehaviorID.ToString() + "_Initialize";
                var m = t.GetMethods();
                var method = t.GetMethod(methodName);
                if(method != null)
                    method.Invoke(this, new object[] { this });
            }
        }
        #endregion

        #region Update
        /// <summary>
        /// Updates the GameEvent.
        /// </summary>
        /// <param name="time"></param>
        public override void Update(GameTime time)
        {
            base.Update(time);
            UpdateTrajectory(time);
        }

        Vector2 m_nextPosition;
        /// <summary>
        /// Updates the platform's trajectory.
        /// </summary>
        /// <param name="time"></param>
        void UpdateTrajectory(GameTime time)
        {
            PlatformEventModule.TrajectoryUnit trajUnit = MPlatform.TrajectoryUnits[m_currentTrajectoryUnitId];
            // Directional vector to the end point.
            Vector2 dst = m_nextPosition - m_body.Position;
            dst.Normalize();
            // Updates the body's speed.
            m_body.LinearVelocity = (dst*trajUnit.Speed);
            // Updates the current trajectory unit id if we are in the good place.
            if((m_body.Position - m_nextPosition).LengthSquared() <= sim(trajUnit.Speed))
            {
                m_currentTrajectoryUnitId = (m_currentTrajectoryUnitId+1) % MPlatform.TrajectoryUnits.Count;
                trajUnit = MPlatform.TrajectoryUnits[m_currentTrajectoryUnitId];
                if (m_currentTrajectoryUnitId == 0)
                {
                    if (trajUnit.Coordinate == PlatformEventModule.TrajectoryUnit.CoordinateMode.Absolute)
                        m_nextPosition = trajUnit.EndPoint;
                    else
                        m_nextPosition = trajUnit.EndPoint + MBase.SimPosition;
                }
                else if (trajUnit.Coordinate == PlatformEventModule.TrajectoryUnit.CoordinateMode.Absolute)
                    m_nextPosition = trajUnit.EndPoint;
                else
                    m_nextPosition += trajUnit.EndPoint;
            }
        }
        #endregion
        #endregion

        /* ---------------------------------------------------------------------------------------
         * Editor
         * -------------------------------------------------------------------------------------*/
        #region Editor
        /// <summary>
        /// Draws the object in the editor.
        /// </summary>
        /// <param name="batch">batch where to draw the object</param>
        /// <param name="scroll">scrolling of the object in pixels</param>
        /// <param name="ops">RenderOptions</param>
        public override void DrawInEditor(SpriteBatch batch, Vector2 scroll, Editor.GameObjectRenderOptions ops)
        {
            // Draws the base elements.
            base.DrawInEditor(batch, scroll, ops);

            // Draws the trajectory.
            DrawTrajectory(batch, scroll, ops);
        }

        /// <summary>
        /// Draws the trajectory of the platform.
        /// </summary>
        void DrawTrajectory(SpriteBatch batch, Vector2 scroll, Editor.GameObjectRenderOptions ops)
        {
            // If the object is selected, draws the trajectory.
            if (ops.IsSelected)
            {
                int i = 0;
                Vector2 currentSimPosition = MBase.SimPosition;
                Vector2 oldSimPosition = MBase.SimPosition;
                for (i = 0; i < MPlatform.TrajectoryUnits.Count; i++)
                {
                    PlatformEventModule.TrajectoryUnit unit = MPlatform.TrajectoryUnits[i];
                    
                    // Compute the destination position depending on the coordinate mode.
                    if (unit.Coordinate == PlatformEventModule.TrajectoryUnit.CoordinateMode.Absolute)
                        currentSimPosition = unit.EndPoint;
                    else
                        currentSimPosition += unit.EndPoint;

                    // Draws the current position.
                    Point dispPosition = new Point((int)disp(currentSimPosition.X * ops.Zoom) - (int)scroll.X,
                        (int)disp(currentSimPosition.Y * ops.Zoom) - (int)scroll.Y);
                    
                    DrawingRoutines.DrawPoint(batch, new Vector2(dispPosition.X, dispPosition.Y), 1.0f, Color.Red, Math.Max(4, (int)(4 * ops.Zoom)));

                    if(i != 0)
                    {
                        Point oldDispPosition = new Point((int)disp(oldSimPosition.X * ops.Zoom) - (int)scroll.X,
                                                            (int)disp(oldSimPosition.Y * ops.Zoom) - (int)scroll.Y);
                        DrawingRoutines.DrawLine(batch, new Vector2(oldDispPosition.X, oldDispPosition.Y),
                                                        new Vector2(dispPosition.X, dispPosition.Y), 1.0f, Color.White);
                    }

                    oldSimPosition = currentSimPosition;
                }
            }
        }

        #region Platform Editing
        bool _isDraging = false;
        PlatformEventModule.TrajectoryUnit _selectedUnit;
        public override ActionType OnMouseDownInEditor(Vector2 simPos, out EditorActionDelegate undoAction, out EditorActionDelegate redoAction)
        {
            // Handles the actions of the base class.
            ActionType retValue = base.OnMouseDownInEditor(simPos, out undoAction, out redoAction);
            if (retValue != ActionType.None)
                return retValue;

            _selectedUnit = null;
            _isDraging = false;

            // If the object is selected, draws the trajectory.
            Vector2 currentSimPosition = MBase.SimPosition;
            foreach (PlatformEventModule.TrajectoryUnit unit in MPlatform.TrajectoryUnits)
            {
                // Compute the destination position depending on the coordinate mode.
                if (unit.Coordinate == PlatformEventModule.TrajectoryUnit.CoordinateMode.Absolute)
                    currentSimPosition = unit.EndPoint;
                else
                    currentSimPosition += unit.EndPoint;

                // Check if this unit is clicked.
                if (disp(Vector2.Distance(simPos, currentSimPosition)) < 8)
                {
                    _selectedUnit = unit;
                }
            }

            EditorActionDelegate undoTmp = delegate() { return; };
            EditorActionDelegate redoTmp = delegate() { return; };
            ActionType action = ActionType.None;

            // Exits with no undo action.
            if (_selectedUnit == null)
            {
                if ((System.Windows.Forms.Control.ModifierKeys & System.Windows.Forms.Keys.Shift) == System.Windows.Forms.Keys.Shift)
                {
                    System.Windows.Forms.ContextMenuStrip menu = new System.Windows.Forms.ContextMenuStrip();
                    var addUnit = new System.Windows.Forms.ToolStripDropDownButton();
                    addUnit.Text = "Ajouter point";
                    addUnit.Click += delegate(object o, EventArgs e)
                    {
                        var unit = new PlatformEventModule.TrajectoryUnit();

                        // Substract the absolute to the base vector to obtain a relative position.
                        unit.EndPoint = simPos - currentSimPosition;

                        MPlatform.TrajectoryUnits.Add(unit);
                        undoTmp = delegate()
                        {
                            MPlatform.TrajectoryUnits.Remove(unit);
                        };
                        redoTmp = delegate()
                        {
                            MPlatform.TrajectoryUnits.Add(unit);
                        };
                        action = ActionType.NoDrag;
                    };

                    menu.Items.Add(addUnit);
                    menu.Show(System.Windows.Forms.Cursor.Position);
                    // Waits for the menu to terminate (and the user to do the actions) before
                    // returning (and giving out the undo / redo macros).
                    while (menu.Visible)
                    {
                        System.Windows.Forms.Application.DoEvents();
                        System.Threading.Thread.Sleep(1);
                    }
                }
                undoAction = undoTmp;
                redoAction = redoTmp;
                return action;
                
            }
            else
            {
                if ((System.Windows.Forms.Control.ModifierKeys & System.Windows.Forms.Keys.Shift) == System.Windows.Forms.Keys.Shift)
                {
                    #region Menu
                    // -- A menu lets the user edit the trajectory unit.
                    System.Windows.Forms.ContextMenuStrip menu = new System.Windows.Forms.ContextMenuStrip();

                    // Trigger
                    var trigger = new System.Windows.Forms.ToolStripDropDownButton();
                    trigger.Text = "Trigger Mode (" + _selectedUnit.Trigger.ToString() + ")";
                    System.Windows.Forms.ToolStripDropDownButton onPlayer = new System.Windows.Forms.ToolStripDropDownButton();
                    onPlayer.Text = "OnPlayer";
                    var oldTrigger = _selectedUnit.Trigger;
                    onPlayer.Click += delegate(object o, EventArgs e)
                    {

                        _selectedUnit.Trigger = PlatformEventModule.TrajectoryUnit.TriggerMode.PlayerOn;
                        undoTmp = delegate() { _selectedUnit.Trigger = oldTrigger; };
                        redoTmp = delegate() { _selectedUnit.Trigger = PlatformEventModule.TrajectoryUnit.TriggerMode.PlayerOn; };
                        action = ActionType.NoDrag;
                    };
                    trigger.DropDownItems.Add(onPlayer);
                    System.Windows.Forms.ToolStripDropDownButton onAlways = new System.Windows.Forms.ToolStripDropDownButton();
                    onAlways.Text = "Always";
                    onAlways.Click += delegate(object o, EventArgs e)
                    {
                        _selectedUnit.Trigger = PlatformEventModule.TrajectoryUnit.TriggerMode.Always;
                        undoTmp = delegate() { _selectedUnit.Trigger = oldTrigger; };
                        redoTmp = delegate() { _selectedUnit.Trigger = PlatformEventModule.TrajectoryUnit.TriggerMode.Always; };
                        action = ActionType.NoDrag;
                    };
                    trigger.DropDownItems.Add(onAlways);
                    menu.Items.Add(trigger);

                    // Speed
                    System.Windows.Forms.ToolStripTextBox tb = new System.Windows.Forms.ToolStripTextBox();
                    tb.ToolTipText = "Vitesse (sim/s)";
                    tb.Text = _selectedUnit.Speed.ToString();
                    float oldValue = _selectedUnit.Speed;
                    tb.LostFocus += delegate(object o, EventArgs e)
                    {
                        float result = 0;
                        if (Single.TryParse(tb.Text, out result))
                        {
                            _selectedUnit.Speed = result;
                            float resBuff = result;
                            undoTmp = delegate() { _selectedUnit.Speed = oldValue; };
                            redoTmp = delegate() { _selectedUnit.Speed = resBuff; };
                        }
                        action = ActionType.NoDrag;
                    };
                    menu.Items.Add(tb);

                    // Delete
                    System.Windows.Forms.ToolStripDropDownButton deleteBtn = new System.Windows.Forms.ToolStripDropDownButton();
                    deleteBtn.Text = "Supprimer";
                    deleteBtn.Click += delegate(object o, EventArgs e)
                    {
                        var unit = _selectedUnit;
                        int index = MPlatform.TrajectoryUnits.IndexOf(unit);

                        MPlatform.TrajectoryUnits.Remove(_selectedUnit);

                        // Undo / redo
                        undoTmp = delegate() { MPlatform.TrajectoryUnits.Insert(index, unit); };
                        redoTmp = delegate() { MPlatform.TrajectoryUnits.Remove(_selectedUnit); };
                        action = ActionType.NoDrag;
                    };

                    menu.Items.Add(deleteBtn);
                    menu.Show(System.Windows.Forms.Cursor.Position);

                    // Waits for the menu to terminate (and the user to do the actions) before
                    // returning (and giving out the undo / redo macros).
                    while (menu.Visible)
                    {
                        System.Windows.Forms.Application.DoEvents();
                        System.Threading.Thread.Sleep(1);
                    }
                    undoAction = undoTmp;
                    redoAction = redoTmp;
                    return action;
                    #endregion
                }
                else
                {
                    // Départ du déplacement du point de trajectoire à la souris.
                    _isDraging = true;
                    Vector2 position = _selectedUnit.EndPoint;
                    PlatformEventModule.TrajectoryUnit trajRef = _selectedUnit;
                    undoAction = delegate()
                    {
                        trajRef.EndPoint = position;
                    };
                    redoAction = delegate() { return; };

                    return ActionType.Drag;
                }
            }

            return ActionType.None;
        }

        /// <summary>
        /// Updates the event when the mouse moves in the editor and it is selected.
        /// </summary>
        /// <param name="simPos"></param>
        /// <param name="delta"></param>
        public override void OnMouseMoveEditor(Vector2 simPos, Vector2 delta)
        {
            base.OnMouseMoveEditor(simPos, delta);
            // Updates the dragging
            if (_isDraging)
            {
                _selectedUnit.EndPoint += delta;
            }
        }

        /// <summary>
        /// Updates tje event when the mouse is released in the editor and it is selected.
        /// </summary>
        /// <param name="simPos"></param>
        /// <param name="redoAction"></param>
        public override void OnMouseUpInEditor(Vector2 simPos, out GameObject.EditorActionDelegate redoAction)
        {
            base.OnMouseUpInEditor(simPos, out redoAction);

            // Ends the dragging
            if(_isDraging && _selectedUnit != null)
            {
                _isDraging = false;
                Vector2 position = _selectedUnit.EndPoint;
                var unitRef = _selectedUnit;
                redoAction = delegate()
                {
                    unitRef.EndPoint = position;
                };
            }
        }
        #endregion
        #endregion


    }
}
