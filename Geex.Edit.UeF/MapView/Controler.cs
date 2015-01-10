using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Geex.Edit.Common.Project;
using Geex.Edit.UeF.Project;
using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics;
using GameObject = UeFGame.GameObjects.GameObject;
using GameObjectInit = UeFGame.GameObjects.GameObjectInit;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using CU = UeFGame.ConvertUnits;
using Map = UeFGame.GameComponents.MapInitializingData;
using MacroRecorder = Geex.Edit.Common.Tools.MacroRecorder<Geex.Edit.UeF.MapView.MacroUnit>;
using Module = UeFGame.GameObjects.Module;
namespace Geex.Edit.UeF.MapView
{
    public delegate void GameObjectActionEndedEventHandler(MacroUnit unit);
    public delegate void GameObjectSelectedEventHandler(GameObject obj);
    public delegate void DrawingActionEndedEventHandler(UeFTileMacroUnit unit);
    /// <summary>
    /// Controler of the map editor.
    /// Its methods will be called by the MapView itself, when some input / actions are 
    /// performed.
    /// </summary>
    public class Controler : IDisposable
    {
        /* --------------------------------------------------------------
         * Events
         * ------------------------------------------------------------*/
        #region Events
        /// <summary>
        /// Occurs at the end of a drawing action.
        /// Must be sent in order to some components to be aware of what's going
        /// on in the view :
        /// For exemple enable/disable the Undo/Redo buttons.
        /// </summary>
        public event GameObjectActionEndedEventHandler GameObjectActionEnded;
        /// <summary>
        /// Event fired when a Game object is selected.
        /// </summary>
        public event GameObjectSelectedEventHandler GameObjectSelected;
        /// <summary>
        /// Occurs at the end of a drawing action.
        /// </summary>
        public event DrawingActionEndedEventHandler DrawingActionEnded;
        #endregion
        /* --------------------------------------------------------------
         * Variable
         * ------------------------------------------------------------*/
        #region Variables
        /// <summary>
        /// Mouse position at last frame.
        /// </summary>
        Point m_oldPos = new Point(-1, -1);
        /// <summary>
        /// Mouse position at the beginning of an action.
        /// </summary>
        Point m_startPos = new Point(-1, -1);
        /// <summary>
        /// Indicates if an action has been aborted.
        /// </summary>
        bool m_actionAborted;
        /// <summary>
        /// Selected object.
        /// </summary>
        UeFGame.GameObjects.GameObject m_selectedObject;
        /// <summary>
        /// World used for the simulation of the game objects.
        /// </summary>
        World m_world;
        /// <summary>
        /// GameObject initializing data actually copied into the clipboard.
        /// </summary>
        UeFGame.GameObjects.GameObjectInit m_clipboardObject;
        #endregion
        /* --------------------------------------------------------------
         * Properties
         * ------------------------------------------------------------*/
        #region Properties
        /// <summary>
        /// Controler state
        /// </summary>
        public ControlerState State { get; set; }
        /// <summary>
        /// Macro recorder
        /// </summary>
        public MacroRecorder MacroRecorder { get; set; }
        /// <summary>
        /// Returns true if a selection action is in process.
        /// </summary>
        public bool SelectionStarted { get; set; }
        /// <summary>
        /// Returns true if an action with the left click started.
        /// </summary>
        public bool ActionStartedLeft { get; set; }
        /// <summary>
        /// The currently selected object.
        /// </summary>
        public UeFGame.GameObjects.GameObject SelectedObject
        {
            get { return m_selectedObject; }
            set
            {
                if (m_selectedObject == value)
                    return;
                m_selectedObject = value;
                if (GameObjectSelected != null)
                    GameObjectSelected(m_selectedObject);
            }
        }
        /// <summary>
        /// Gets the render option's zoom value.
        /// </summary>
        float Zoom
        {
            get { return UeFGlobals.MapView.GraphicsManager.RenderOptions.Zoom; }
        }
        /// <summary>
        /// Gets a value indicating if we can perform actions.
        /// </summary>
        bool IsOk
        {
            get
            {
                return UeFGlobals.MapDataWrapper.Map != null && UeFGlobals.MapDataWrapper.GameObjects != null;
            }
        }
        #endregion
        /* --------------------------------------------------------------
         * Setup events
         * ------------------------------------------------------------*/
        #region Setup Events
        /// <summary>
        /// Public controler constructor.
        /// </summary>
        public Controler()
        {
            State = new ControlerState();
            State.DrawMode = DrawMode.Pen;
            State.Mode = ControlerMode.Tile;
            MacroRecorder = new MacroRecorder(50);
            m_world = new World(Vector2.Zero);
            UeFGame.Globals.World = m_world;
        }
        /// <summary>
        /// This method will be called after the creation of the instances we need in order to
        /// suscribe to their events.
        /// </summary>
        public void SetupEvents()
        {
            UeFGlobals.MapTreeView.MapSelected += new Common.Tools.Controls.MapTreeView.MapSelectedDelegate(OnMapSelected);
            UeFGlobals.Project.OnProjectSaved += new UeFGeexProject.ProjectSaveDelegate(OnProjectSaved);
            UeFGlobals.TilePicker.AreaSelected += new TilePicker.AreaSelectedEventHandler(OnTileSelectorAreaSelected);
        }
        /// <summary>
        /// Called when the project is saved.
        /// </summary>
        void OnProjectSaved()
        {
            // The maps are all marked saved, we need to add the current editing map to the unsaved map list.
            if(UeFGlobals.MapView.MapDataWrapper.Map != null)
                UeFGlobals.Project.AddUnsavedMap(UeFGlobals.MapView.MapDataWrapper.MapInfo, UeFGlobals.MapView.MapDataWrapper.Map);
        }
        /// <summary>
        /// Occurs when a map is selected in the MapTreeView.
        /// </summary>
        /// <param name="mapInfo"></param>
        void OnMapSelected(MapInfo mapInfo)
        {
            Map map = UeFGlobals.Project.RetrieveUnsaved(mapInfo);
            if (map != null)
            {
                UeFGlobals.MapDataWrapper.LoadMap(map, mapInfo);
            }
            else
            {
                UeFGlobals.MapDataWrapper.LoadMap(mapInfo);
                UeFGlobals.Project.AddUnsavedMap(mapInfo, UeFGlobals.MapDataWrapper.Map);
            }
            Common.Globals.MapView.RefreshMap();
            MacroRecorder.Reset();
            Common.Globals.MapView.IsDirty = true;
        }
        /// <summary>
        /// Disposes the controler.
        /// </summary>
        public void Dispose()
        {
            foreach (Body body in m_world.BodyList)
                if(!body.IsDisposed)
                    body.Dispose();

            m_world.BodyList.Clear();
        }
        #endregion
        /* --------------------------------------------------------------
         * MapView calls
         * ------------------------------------------------------------*/
        #region MapView Calls
        /// <summary>
        /// Transforms a mouse position into a tile position.
        /// </summary>
        /// <param name="mousePos"></param>
        /// <returns></returns>
        Point ToTile(Point mousePos)
        {
            mousePos.X = mousePos.X < 0 ? 0 : mousePos.X;
            mousePos.Y = mousePos.Y < 0 ? 0 : mousePos.Y;
            return new Point((int)((mousePos.X + UeFGlobals.MapView.ScrollX) / UeFGlobals.MapView.GraphicsManager.RenderOptions.Zoom / Project.GameOptions.TileSize),
                (int)((mousePos.Y  +  UeFGlobals.MapView.ScrollY) / UeFGlobals.MapView.GraphicsManager.RenderOptions.Zoom / Project.GameOptions.TileSize));
        }

        /// <summary>
        /// This event occurs when an area is selected in the TileSelector.
        /// This will add to the selection matrix the ids (on the tileset)
        /// </summary>
        /// <param name="rect">The new selected rectangle</param>
        void OnTileSelectorAreaSelected(Rectangle rect)
        {
            // We assume that we have a correct RpgHandler object here
            TilePicker tileSelector = UeFGlobals.TilePicker;

            m_selectionMatrix = new int[rect.Width][];
            for (short x = 0; x < rect.Width; x++)
            {
                m_selectionMatrix[x] = new int[rect.Height];
                for (short y = 0; y < rect.Height; y++)
                {
                    m_selectionMatrix[x][y] =
                        tileSelector.GetMatrixId(
                        x + rect.X, y + rect.Y);
                }
            }
            m_selectedTiles = new Rectangle(0, 0, rect.Width, rect.Height);
            m_cursorRect.Width = rect.Width;
            m_cursorRect.Height = rect.Height;
        }
        /// <summary>
        /// This event occurs when a map area has been selected.
        /// </summary>
        /// <param name="rect"></param>
        void OnMapAreaSelected()
        {
            m_selectionMatrix = new int[m_selectedTiles.Width][];
            for (short x = 0; x < Math.Min(TileData[ActiveLayer].GetLength(0) - m_selectedTiles.X, m_selectedTiles.Width); x++)
            {
                m_selectionMatrix[x] = new int[m_selectedTiles.Height];
                for (short y = 0; y < Math.Min(TileData[ActiveLayer].GetLength(1) - m_selectedTiles.Y, m_selectedTiles.Height); y++)
                {
                    int id = TileData[ActiveLayer][m_selectedTiles.X + x,m_selectedTiles.Y + y];
                    /* AUTOTILE CODE
                     * if (UeFGlobals.MapDataWrapper.IsValidAutotile(id))
                    {
                        m_selectionMatrix[x][y] = -RpgMapDataWrapper.GetAutotileNumber(id);
                    } 
                    else
                    { */
                    m_selectionMatrix[x][y] = id;
                    
                }
            }
        }
        /// <summary>
        /// Starts an action such as drawing etc...
        /// </summary>
        /// <param name="mousePos">Tile where the action starts</param>
        public void OnStartAction(Point mousePos, bool controlDown)
        {
            // Return if the control is not OK
            if (!IsOk)
                return;
            m_actionAborted = false;
            m_startPos = mousePos;
            switch (this.State.Mode)
            {
                case ControlerMode.Tile:
                    OnStartActionDrawMode(ToTile(mousePos), false);
                    break;
                case ControlerMode.GameObjects:
                    ActionStartedLeft = StartGameObjectAction(mousePos);
                    break;
                case ControlerMode.GameObjectsEditMode:
                    ActionStartedLeft = StartGameObjectEditAction(mousePos, false);
                    break;
            }
            m_oldPos.X = mousePos.X;
            m_oldPos.Y = mousePos.Y;
        }
        /// <summary>
        /// Updates the current action
        /// </summary>
        /// <param name="mousePos">Tile where the action starts</param>
        public void OnUpdateAction(Point mousePos, bool shifted)
        {
            // Return if the control is not OK
            if (!IsOk || m_actionAborted)
                return;
            switch (this.State.Mode)
            {
                case ControlerMode.Tile:
                    OnUpdateActionDrawMode(ToTile(mousePos), false);
                    OnMouseMotion(mousePos);
                    break;
                case ControlerMode.GameObjects:
                    UpdateGameObjectAction(mousePos);
                    break;
                case ControlerMode.GameObjectsEditMode:
                    if(ActionStartedLeft)
                        UpdateGameObjectEditAction(mousePos);
                    break;
            }
        }
        /// <summary>
        /// Ends the current action.
        /// Each action must register a MacroUnit in the Recorder when it ends.
        /// It won't be called if ActionStartedLeft is set to false.
        /// </summary>
        /// <param name="mousePos">Tile where the action starts</param>
        public void OnEndAction(Point mousePos, bool shifted)
        {
            // Ensure action started left is true.
            if (!ActionStartedLeft)
                throw new InvalidOperationException("This method must not be called when no action is started");
            // Return if the control is not OK
            if (!IsOk | m_actionAborted)
                return;
            switch (this.State.Mode)
            {
                case ControlerMode.Tile:
                    OnEndActionDrawingMode(ToTile(mousePos), shifted);
                    break;
                case ControlerMode.GameObjects:
                    EndGameObjectAction(mousePos);
                    break;
                case ControlerMode.GameObjectsEditMode:
                    if(ActionStartedLeft)
                        EndGameObjectEditAction(mousePos);
                    break;
            }
            ActionStartedLeft = false;
        }
        /// <summary>
        /// Aborts the current action.
        /// If no action is currently processing, raises an exception.
        /// </summary>
        public void AbortAction()
        {
            // Return if the control is not OK
            if (!IsOk)
                return;

            // Ends the action to register the macro.
            OnEndAction(m_oldPos, false);
            // Now sets the variables to their correct values.
            m_actionAborted = true;     // will be set to false when another action starts.
            ActionStartedLeft = false;
            // Undoes the last operation.
            Undo();
            UeFGlobals.MapView.IsDirty = true;
        }
        /// <summary>
        /// This event occurs when the mouse is moving.
        /// It is called after Update Action if the left button
        /// of the mouse is clicked.
        /// </summary>
        /// <param name="mousept">Location of the mouse, in tiles.</param>
        public void OnMouseMotion(Point mousept)
        {
            m_oldPos.X = mousept.X;
            m_oldPos.Y = mousept.Y;

            // Tiles
            Point newTile = ToTile(mousept);
            if (newTile.X != m_oldTile.X || newTile.Y != m_oldTile.Y)
            {
                UpdateCursor(newTile);
            }
            m_oldTile = newTile;
        }
        /// <summary>
        /// Called when the mouse leaves the XnaWindow
        /// Will stop displaying the cursor.
        /// </summary>
        public void OnMouseLeave()
        {
        }
        /// <summary>
        /// Called when the mouse enters the XnaWindow
        /// Will resume displaying the cursor.
        /// </summary>
        public void OnMouseEnter()
        {

        }
        /// <summary>
        /// Called when a key is pressed.
        /// </summary>
        /// <param name="e"></param>
        public void OnKeyDown(PreviewKeyDownEventArgs e)
        {
            // Return if the control is not OK
            if (!IsOk)
                return;
            // If the control key is pressed, and we are in a GameObject mode
            // then changes the mode.
            if (e.KeyCode == Keys.Space && SelectedObject != null)
            {
                if (State.Mode == ControlerMode.GameObjects)
                {
                    State.Mode = ControlerMode.GameObjectsEditMode;
                }
                else if (State.Mode == ControlerMode.GameObjectsEditMode)
                {
                    State.Mode = ControlerMode.GameObjects;
                }
            }
            switch (this.State.Mode)
            {
                case ControlerMode.GameObjects:
                case ControlerMode.GameObjectsEditMode:
                    OnGameObjectKeyDown(e);
                    break;
            }
        }
        #endregion
        /* --------------------------------------------------------------
         * Game object mode
         * ------------------------------------------------------------*/
        #region GameObjectMode

        #region Variables
        /// <summary>
        /// The game object macro unit currently being edited.
        /// </summary>
        MacroUnit _currentGameObjectMacroUnit;
        /// <summary>
        /// Indicates if a multi-object selection started.
        /// </summary>
        bool _objectSelectionStart;
        /// <summary>
        /// Selected objects.
        /// </summary>
        List<GameObject> _selectedObjects = new List<GameObject>();
        /// <summary>
        /// The current selection rect.
        /// Do not use it outside of the property.
        /// </summary>
        Rectangle? _gameObjectSelectionRect;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the selected objects.
        /// </summary>
        public List<GameObject> SelectedObjects
        {
            get { return _selectedObjects; }
            protected set { _selectedObjects = value; }
        }
        /// <summary>
        /// Gets or sets the selection rectangle.
        /// If null : no selection rectangle has to be drawn.
        /// </summary>
        public Rectangle? GameObjectSelectionRect
        {
            get
            {
                return _gameObjectSelectionRect;
            }
            protected set
            {
                if (value != null)
                {
                    var val = value.Value;
                    if (val.Width < 0)
                    {
                        val.X += val.Width;
                        val.Width = -val.Width;
                    }
                    if (val.Height < 0)
                    {
                        val.Y += val.Height;
                        val.Height = -val.Height;
                    }
                    _gameObjectSelectionRect = val;
                }
                else
                    _gameObjectSelectionRect = value;
            }
        }
        #endregion

        #region Left
        /// <summary>
        /// Starts a action in GameObject mode.
        /// In GameObject mode we move objects when they are dragged.
        /// </summary>
        /// <returns>Returns true if an action is started.</returns>
        bool StartGameObjectAction(Point point)
        {
            UeFGlobals.MapView.IsDirty = true;
            // Start selecting multiple objects by drag'n'drop.
            SelectedObject = GetObjectAt(point);
            // If the previously selected objects contains the selected object,
            // we won't cancel the previous selection.
            if (SelectedObject != null)
                if (SelectedObjects.Contains(SelectedObject))
                {
                    _objectSelectionStart = false;
                }
                else
                {
                    // If the previously selected objects do not contains the selected object, then
                    // clears the selection and select the new one
                    SelectedObjects.Clear();
                    SelectedObjects.Add(SelectedObject);
                }
            else
            {
                SelectedObjects.Clear();
                // If no selected object, start a drag operation to select many.
                _objectSelectionStart = true;
                return true;
            }
            
            // Clears the multi-object selection values
            _objectSelectionStart = false;
            GameObjectSelectionRect = null;



            // Start moving the objects
            // Creates the undo delegates
            List<GameObject.EditorActionDelegate> undoDelegates = new List<GameObject.EditorActionDelegate>();
            List<Vector2> oldValues = new List<Vector2>();
            foreach(GameObject obj in SelectedObjects)
            {
                GameObject objRef = obj;
                Vector2 posref = obj.InitializingData.ModuleSet.Base.SimPosition;
                undoDelegates.Add(() => {
                    objRef.InitializingData.ModuleSet.Base.SimPosition = posref;
                });
            }

            // Multiple drag operation macro unit.
            _currentGameObjectMacroUnit = new MultipleGameObjectMacroUnit(SelectedObjects, undoDelegates, new List<GameObject.EditorActionDelegate>());
            

            UeFGlobals.MapView.IsDirty = true;
            return true;
        }
        /// <summary>
        /// Update an action in GameObject mode
        /// </summary>
        void UpdateGameObjectAction(Point point)
        {
            if (_objectSelectionStart)
            {
                // if we are selecting objects
                GameObjectSelectionRect = new Rectangle(m_startPos.X, m_startPos.Y,
                    point.X - m_startPos.X, point.Y - m_startPos.Y);
                // Gets the selected objects.
                SelectedObjects = GetObjectsIn(GameObjectSelectionRect);
                // Select one of them.
                if(SelectedObject == null && SelectedObjects.Count != 0)
                    SelectedObject = SelectedObjects.First();
            }
            else
            {
                foreach(GameObject obj in SelectedObjects)
                {
                    obj.Modules.Base.SimStartX += CU.ToSimUnits((point.X - m_oldPos.X)/Zoom);
                    obj.Modules.Base.SimStartY += CU.ToSimUnits((point.Y - m_oldPos.Y)/Zoom);
                }
            }
            UeFGlobals.MapView.IsDirty = true;
        }
        /// <summary>
        /// Ends an action in GameObject mode.
        /// It is also called when the action is aborted.
        /// It must register a MacroUnit in the MacroRecorder.
        /// </summary>
        void EndGameObjectAction(Point point)
        {
            UeFGlobals.MapView.IsDirty = true;
            // Erases the selection. 
            if(_objectSelectionStart)
            {
                GameObjectSelectionRect = null;
                _objectSelectionStart = false;
                return;
            }
            else// drag -> position
            {
                // And of the drag position operation.
                if (_currentGameObjectMacroUnit is MultipleGameObjectMacroUnit)
                {
                    MultipleGameObjectMacroUnit unit = (MultipleGameObjectMacroUnit)_currentGameObjectMacroUnit;
                    List<GameObject.EditorActionDelegate> redoDelegates = new List<GameObject.EditorActionDelegate>();
                    foreach (GameObject obj in SelectedObjects)
                    {
                        GameObject objRef = obj;
                        Vector2 posref = obj.InitializingData.ModuleSet.Base.SimPosition;
                        redoDelegates.Add(() =>
                        {
                            objRef.InitializingData.ModuleSet.Base.SimPosition = posref;
                        });
                    }
                    unit.RedoActions = redoDelegates;
                }
                else if (_currentGameObjectMacroUnit is GameObjectMacroUnit)
                {
                    GameObjectMacroUnit unit = (GameObjectMacroUnit)_currentGameObjectMacroUnit;
                    GameObject objRef = SelectedObject;
                    Vector2 posref = objRef.InitializingData.ModuleSet.Base.SimPosition;
                    unit.RedoAction = new GameObject.EditorActionDelegate(() =>
                    {
                        objRef.InitializingData.ModuleSet.Base.SimPosition = posref;
                    });
                }
            }

            // Appends the last action.
            MacroRecorder.Append(_currentGameObjectMacroUnit);

            // Nulls the current game object macro unit.
            _currentGameObjectMacroUnit = null;
            
            // Fires the "action ended" event :
            if(GameObjectActionEnded != null)
                GameObjectActionEnded(_currentGameObjectMacroUnit);
        }
        #endregion

        #region Right
        /// <summary>
        /// Start an action with the right click.
        /// Used to display menus.
        /// </summary>
        /// <param name="point"></param>
        void StartGameObjectRightAction(Point point)
        {
            List<UeFGame.GameObjects.GameObject> objects = GetObjectsAt(point);
            // Check if one of the object clicked is a selected object
            // If it's the case, extends the selection to all the selected objects.
            bool isSelected = false;
            foreach(GameObject obj in objects)
            {
                foreach(GameObject obj2 in SelectedObjects)
                {
                    if (obj == obj2)
                    {
                        isSelected = true;
                        break;
                    }
                }
            }
            // Extend the selection to all the selected objects.
            // But exclude those that are not selected objects and that could be under the click
            if (isSelected && SelectedObjects.Count > 1)
            {
                objects = SelectedObjects;
            }
            // ----------
            // Menu strip
            ContextMenuStrip strip = new ContextMenuStrip();
            // Proposes to add a new object :
            string typeName = UeFGlobals.UIHandler.GameObjectPicker.SelectionObjectType.Name;
            ToolStripMenuItem addNewBtn = new ToolStripMenuItem("Nouvel objet (" + typeName + ")");
            strip.Items.Add(addNewBtn);
            addNewBtn.Click += delegate(object o, EventArgs a)
            {
                AddObject(point);
            };

            // Pasting an object
            ToolStripMenuItem pasteBtn = new ToolStripMenuItem("Coller");
            pasteBtn.Click += delegate(object o, EventArgs e)
            {
                PasteClipboardObject(new Point((int)MouseToSim(point).X, (int)MouseToSim(point).Y));
            };
            pasteBtn.Enabled = m_clipboardObject != null;
            // Start position
            ToolStripMenuItem startPosBtn = new ToolStripMenuItem("Point de départ");
            startPosBtn.Click += delegate(object o, EventArgs e)
            {
                string filename = UeFGlobals.Project.ContentDirectory + "\\config.xml";
                UeFGame.Config cfg = UeFGame.Config.LoadFromFile(filename);
                cfg.StartMapId = UeFGlobals.MapDataWrapper.MapInfo.Id;
                cfg.PlayerStartPositionSim = MouseToSim(point);
                cfg.Save(filename);
            };
            
            // Selection / Deletion
            if (objects.Count >= 1)
            {
                SelectedObject = objects.First();
                ToolStripMenuItem select = new ToolStripMenuItem("Sélectioner"); // main strip for selection
                ToolStripMenuItem delete = new ToolStripMenuItem("Supprimer");
                ToolStripMenuItem properties = new ToolStripMenuItem("Propriétés");
                ToolStripMenuItem copy = new ToolStripMenuItem("Copier");
                ToolStripMenuItem paste = new ToolStripMenuItem("Paste");

                // Make the items work for the selected object.
                if (objects.Count == 1)
                {
                    GameObject obj = m_selectedObject;
                    // Selection
                    select.Click += delegate(object o, EventArgs e)
                    {
                        SelectedObject = obj;
                        UeFGlobals.MapView.IsDirty = true;
                    };
                    // Deletion
                    delete.Click += delegate(object o, EventArgs e)
                    {
                        UeFGlobals.MapDataWrapper.DeleteObject(obj);
                        UeFGlobals.MapView.IsDirty = true;
                    };
                    // Deletion
                    properties.Click += delegate(object o, EventArgs e)
                    {
                        OnViewProperties(obj);
                    };
                    // Copy
                    copy.Click += delegate(object o, EventArgs e)
                    {
                        CopyObject(obj);
                    };
                    
                }
                else
                {
                    // Delete all
                    ToolStripMenuItem btnDelAll = new ToolStripMenuItem("Tous");
                    delete.DropDownItems.Add(btnDelAll);
                    // Delete item :
                    btnDelAll.Click += delegate(object o, EventArgs e)
                    {
                        int count = objects.Count;
                        foreach (GameObject obj in objects)
                        {
                            UeFGlobals.MapDataWrapper.DeleteObject(obj);
                        }
                        UeFGlobals.MapView.IsDirty = true;
                    };

                    foreach (UeFGame.GameObjects.GameObject obj in objects)
                    {
                        UeFGame.GameObjects.GameObject objRef = obj;
                        // Selection
                        ToolStripMenuItem btn = new ToolStripMenuItem(obj.GetType().Name + " : " + obj.Modules.Base.Name);
                        select.DropDownItems.Add(btn);
                        btn.Click += delegate(object o, EventArgs e)
                        {
                            SelectedObject = objRef;
                            UeFGlobals.MapView.IsDirty = true;
                        };
                        // Deletion
                        ToolStripMenuItem btnDel = new ToolStripMenuItem(obj.GetType().Name + " : " + obj.Modules.Base.Name);
                        delete.DropDownItems.Add(btnDel);
                        btnDel.Click += delegate(object o, EventArgs e)
                        {
                            UeFGlobals.MapDataWrapper.DeleteObject(objRef);
                            UeFGlobals.MapView.IsDirty = true;
                        };
                        // Deletion
                        ToolStripMenuItem btnProperties = new ToolStripMenuItem(obj.GetType().Name + " : " + obj.Modules.Base.Name);
                        properties.DropDownItems.Add(btnProperties);
                        btnProperties.Click += delegate(object o, EventArgs e)
                        {
                            OnViewProperties(objRef);
                        };
                        // Copy
                        ToolStripMenuItem btnCopy = new ToolStripMenuItem(obj.GetType().Name + " : " + obj.Modules.Base.Name);
                        copy.DropDownItems.Add(btnCopy);
                        btnCopy.Click += delegate(object o, EventArgs e)
                        {
                            CopyObject(objRef);
                        };
                        
                    }
                }
                strip.Items.Add(select);
                strip.Items.Add(new ToolStripSeparator());
                strip.Items.Add(delete);
                strip.Items.Add(properties);
                strip.Items.Add(new ToolStripSeparator());
                strip.Items.Add(copy);
            }
            strip.Items.Add(pasteBtn);
            strip.Items.Add(new ToolStripSeparator());
            strip.Items.Add(startPosBtn);
            strip.Show(UeFGlobals.MapView, point);
            UeFGlobals.MapView.IsDirty = true;
        }
        #endregion

        #region Keys
        /// <summary>
        /// Called when a key is pressed.
        /// Calls the "OnKeyDownInEditor" method of the selected object.
        /// </summary>
        /// <param name="e"></param>
        void OnGameObjectKeyDown(PreviewKeyDownEventArgs e)
        {
            if (SelectedObject == null)
                return;
            // In game object edit mode, the selected object has the focus,
            // and he receives the input.
            if (State.Mode == ControlerMode.GameObjectsEditMode)
            {
                // Makes the object perform its action:
                GameObject.EditorActionDelegate undoAction, redoAction;
                bool captured = SelectedObject.OnKeyDownInEditor(MouseToSim(m_oldPos), out undoAction, out redoAction, e.KeyCode);
                if (captured)
                {
                    GameObjectMacroUnit unit = new GameObjectMacroUnit(undoAction, redoAction);
                }
            }

            UeFGlobals.MapView.IsDirty = true;
        }
        #endregion

        /// <summary>
        /// Displays the properties dialog for an object.
        /// </summary>
        /// <param name="obj"></param>
        void OnViewProperties(GameObject obj)
        {
            Views.GameObjectPropertiesView view = new Views.GameObjectPropertiesView(obj);
            view.ShowDialog();
        }

        /// <summary>
        /// Adds an object to the map, at the given location.
        /// </summary>
        void AddObject(Point mousePt)
        {
            Dictionary<string, Type> tInit = UeFGlobals.UIHandler.GameObjectPicker.SelectionModules;
            Type tObj = UeFGlobals.UIHandler.GameObjectPicker.SelectionObjectType;
            // Creates a game object instance from the type given in the init data.
            GameObject obj = (GameObject)Activator.CreateInstance(tObj, new object[] { });
            GameObjectInit init = new GameObjectInit();
            foreach (var kvp in tInit)
            {
                init.ModuleSet[kvp.Key] = (Module)Activator.CreateInstance(kvp.Value, new object[] { });
            }
            init.Type = tObj.FullName;
            init.ModuleSet.Base.SimPosition = MouseToSim(mousePt);
            init.ModuleSet.Base.BehaviorID = FindFirstAvailableId();
            obj.InitializingData = init;
            // Initializes the object
            obj.Initialize();
            UeFGlobals.MapDataWrapper.AddGameObject(obj);
            UeFGlobals.MapView.IsDirty = true;
        }

        /// <summary>
        /// Puts the given object into the clipboard.
        /// </summary>
        /// <param name="obj"></param>
        void CopyObject(GameObject obj)
        {
            m_clipboardObject = obj.InitializingData.DeepCopy();
        }

        /// <summary>
        /// Pastes the clipboard object into the map at the given location.
        /// </summary>
        /// <param name="simLocation"></param>
        void PasteClipboardObject(Point simLocation)
        {
            PasteObject(m_clipboardObject, simLocation);
        }

        /// <summary>
        /// Pastes the given object into the map at the given location.
        /// </summary>
        /// <param name="location"></param>
        void PasteObject(GameObjectInit gameObjectInit, Point simLocation)
        {
            if (m_clipboardObject == null)
                return;

            var init = gameObjectInit.DeepCopy();
            init.ModuleSet.Base.SimPosition = new Vector2(simLocation.X, simLocation.Y);
            init.ModuleSet.Base.BehaviorID = FindFirstAvailableId();

            // Gets the type to create an instance of the object
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetAssembly(typeof(GameObject));
            Type tObj = assembly.GetType(init.Type);

            // Creates the instance
            GameObject obj = (GameObject)Activator.CreateInstance(tObj, new object[] { });
            
            // Initializes the object
            obj.InitializingData = init;
            obj.Initialize();
            UeFGlobals.MapDataWrapper.AddGameObject(obj);
            UeFGlobals.MapView.IsDirty = true;
        }
        /// <summary>
        /// Find the first available Game Object id.
        /// </summary>
        /// <returns></returns>
        int FindFirstAvailableId()
        {

            int i = 0;
            int j = 0;
            while (i < UeFGlobals.MapDataWrapper.GameObjects.Count)
            {
                bool isFree = true;
                // On regarde si il y a une place pour i
                foreach (GameObject obj in UeFGlobals.MapDataWrapper.GameObjects)
                {
                    int id = obj.MBase.BehaviorID;
                    if(id == i)
                        isFree = false;
                }
                if(isFree)
                    break;
                i++;
            }
            return i;
        }
        #endregion
        /* --------------------------------------------------------------
         * Game object edit mode
         * ------------------------------------------------------------*/
        #region GameObjectEditMode

        #region Left
        /// <summary>
        /// Starts a action in GameObject mode.
        /// In GameObject mode we move objects when they are dragged.
        /// </summary>
        /// <returns>Returns true if an action is started.</returns>
        bool StartGameObjectEditAction(Point point, bool rightClick)
        {
            UeFGlobals.MapView.IsDirty = true;

            // Check is the selected object will start a drag operation
            GameObject.EditorActionDelegate undoAction, redoAction;
            GameObject.ActionType actionType = SelectedObject.OnMouseDownInEditor(MouseToSim(point), out undoAction, out redoAction);

            // If no action is performed, return false : the action will be aborted.
            if (actionType == GameObject.ActionType.None)
                return false;

            // Creates the macro unit if needed.
            _currentGameObjectMacroUnit = new GameObjectMacroUnit(undoAction, redoAction);

            // If no drag operation is launched, then record the macro now and end the editing.
            if (actionType == GameObject.ActionType.NoDrag)
            {
                MacroRecorder.Append(_currentGameObjectMacroUnit);
                _currentGameObjectMacroUnit = null;
                return false;
            }
            else
            {
                // We will start a new drag operation.
                UeFGlobals.MapView.IsDirty = true;
                return true;
            }
        }
        /// <summary>
        /// Update an action in GameObject mode
        /// </summary>
        void UpdateGameObjectEditAction(Point point)
        {
            UeFGlobals.MapView.IsDirty = true;
            SelectedObject.OnMouseMoveEditor(MouseToSim(point), MouseToSim(point) - MouseToSim(m_oldPos));
            //    SelectedObject.Initialize();
            
        }
        /// <summary>
        /// Ends an action in GameObject mode.
        /// It is also called when the action is aborted.
        /// It must register a MacroUnit in the MacroRecorder.
        /// </summary>
        void EndGameObjectEditAction(Point point)
        {
            UeFGlobals.MapView.IsDirty = true;

            // Ends the action
            GameObject.EditorActionDelegate redoAction;
            SelectedObject.OnMouseUpInEditor(MouseToSim(point), out redoAction);
            ((GameObjectMacroUnit)_currentGameObjectMacroUnit).RedoAction = redoAction;

            // Appends the last action.
            MacroRecorder.Append(_currentGameObjectMacroUnit);

            // Nulls the current game object macro unit.
            _currentGameObjectMacroUnit = null;

            // Fires the "action ended" event :
            if (GameObjectActionEnded != null)
                GameObjectActionEnded(_currentGameObjectMacroUnit);

            SelectedObject.Initialize();
        }
        #endregion
        #endregion
        /* ------------------------------------------------------------------------------------------
         * Drawing actions.
         * Gathers the action related to the Drawing mode. (when the state's edit mode is EditMode.Draw).
         * ----------------------------------------------------------------------------------------*/
        #region Drawing Actions
        /// <summary>
        /// Updates the cursor position and size only if there is a tile selection/action on the map.
        /// </summary>
        void UpdateCursor()
        {
            switch (State.Mode)
            {
                case ControlerMode.Tile:
                    if (ActionStartedLeft && State.DrawMode == DrawMode.Rectangle)
                    {
                        // Disables the drawing of the cursor while a rectangle is being drawn
                        UeFGlobals.MapView.GraphicsManager.RenderOptions.IsCursorVisible = false;
                        return;
                    }
                    if (SelectionStarted)
                    {
                        UeFGlobals.MapView.GraphicsManager.RenderOptions.IsCursorVisible = true;
                        m_cursorRect.X = m_selectedTiles.X;
                        m_cursorRect.Y = m_selectedTiles.Y;
                        m_cursorRect.Width = m_selectedTiles.Width;
                        m_cursorRect.Height = m_selectedTiles.Height;
                        UeFGlobals.MapView.IsDirty = true;
                    }
                    break;
                case ControlerMode.GameObjects:
                    UeFGlobals.MapView.GraphicsManager.RenderOptions.IsCursorVisible = false;
                    break;
            }
        }
        /// <summary>
        /// Updates the cursor position.
        /// This is called either when a new tile is pointed, either when an area is selected.
        /// </summary>
        /// <param name="pt">The pointed tile if not selecting tiles on the map.
        /// If the Tile selection has started, this argument is useless.</param>
        void UpdateCursor(Point pt)
        {
            switch (State.Mode)
            {
                case ControlerMode.Tile:
                    if (ActionStartedLeft && State.DrawMode == DrawMode.Rectangle)
                    {
                        // Disables the drawing of the cursor while a rectangle is being drawn
                        UeFGlobals.MapView.GraphicsManager.RenderOptions.IsCursorVisible = false;
                        return;
                    }
                    // Updates the cursor's position and 
                    if (SelectionStarted == false)
                    {
                        UeFGlobals.MapView.GraphicsManager.RenderOptions.IsCursorVisible = UeFGlobals.MapView.GraphicsManager.RenderOptions.Zoom <= 2;
                        m_cursorRect.X = pt.X;
                        m_cursorRect.Y = pt.Y;
                        m_cursorRect.Width = m_selectedTiles.Width;
                        m_cursorRect.Height = m_selectedTiles.Height;
                        if (UeFGlobals.MapView.GraphicsManager.RenderOptions.IsCursorVisible)
                            UeFGlobals.MapView.IsDirty = true;
                    }
                    else
                    {
                        UeFGlobals.MapView.GraphicsManager.RenderOptions.IsCursorVisible = true;
                        m_cursorRect.X = m_selectedTiles.X;
                        m_cursorRect.Y = m_selectedTiles.Y;
                        m_cursorRect.Width = m_selectedTiles.Width;
                        m_cursorRect.Height = m_selectedTiles.Height;
                        UeFGlobals.MapView.IsDirty = true;
                    }
                    break;
            }

        }
        /// <summary>
        /// Aborts the current action.
        /// If no action is currently processing, raises an exception.
        /// </summary>
        void OnAbortActionDrawMode()
        {
            if (ActionStartedLeft)
            {
                if (State.DrawMode == DrawMode.Pen)
                {
                    OnAbortDrawPen();
                }
                else if (State.DrawMode == DrawMode.Rectangle)
                {
                    OnAbortDrawRectangle();
                }
                m_currentRecording = null;
                ActionStartedLeft = false;
            }
            else
            {
                throw new Exception(@"An action must be started in order to abort it. 
This exception is thrown in order to avoid 'random' calls to this function.");
            }
            UpdateCursor();

            // Fires the event
            if (DrawingActionEnded != null)
                DrawingActionEnded(m_currentRecording);
        }
        /// <summary>
        /// Max out
        /// </summary>
        int MaxOut { get; set; }
        /// <summary>
        /// Starts a drawing action.
        /// </summary>
        /// <param name="tile">Tile where the action starts</param>
        void OnStartActionDrawMode(Point tile, bool shifted)
        {
            if (!ActionStartedLeft)
                ActionStartedLeft = true; // Notifies an action started
            else
                return;
            // Takes a snapshot of the data before modification.
            MaxOut = Math.Max(m_selectedTiles.Width, m_selectedTiles.Height); // max overflow of the screen / we can change it later if needed
            Rectangle rect = UeFGlobals.MapView.GetScreenTileRectangle();
            rect.X = Math.Max(rect.X - MaxOut, 0);
            rect.Y = Math.Max(rect.Y - MaxOut, 0);
            rect.Width = Math.Min(TileData[ActiveLayer].GetLength(0), Math.Min(rect.Width + MaxOut * 2, TileData[0].GetLength(0) - rect.X));
            rect.Height = Math.Min(TileData[ActiveLayer].GetLength(1),  Math.Min(rect.Height + MaxOut * 2, TileData[0].GetLength(1) - rect.Y));
            m_dataSnapshot = new TileDataSnapshot(rect, TileData, ActiveLayer);
            // Creates the start point
            m_startPoint = new Point(tile.X, tile.Y);
            switch (State.DrawMode)
            {
                case DrawMode.Pen:
                    OnStartDrawPen(tile);
                    break;

                case DrawMode.Rectangle:
                    OnStartDrawRectangle(tile);
                    break;

                case DrawMode.FloodFill:
                    OnStartDrawFloodFill(tile);
                    break;
            }
            // Updates old tile information
            m_oldTile.X = tile.X;
            m_oldTile.Y = tile.Y;
            UpdateCursor();
        }
        /// <summary>
        /// Updates the current drawing action.
        /// </summary>
        /// <param name="tile">Tile where the action starts</param>
        void OnUpdateActionDrawMode(Point tile, bool shifted)
        {
            IsAutotileCheckingEnabled = !shifted;
            // Returns if no action has been started
            if (!ActionStartedLeft)
                throw new Exception("An action must be started in order to call OnUpdateAction");

            if (tile.X != m_oldTile.X || tile.Y != m_oldTile.Y)
            {
                if (State.DrawMode == DrawMode.Pen)
                {
                    // Draws a line between the current position and the new position, instead of
                    // drawing only one tile.
                    int diffX = (tile.X - m_oldTile.X) / m_selectedTiles.Width;
                    int diffY = (tile.Y - m_oldTile.Y) / m_selectedTiles.Height;
                    int iterations = (int)Math.Sqrt(Math.Pow(diffX, 2.0) + Math.Pow(diffY, 2.0));
                    if (iterations == 0)
                        iterations = 1;
                    Point drawTile = new Point(m_oldTile.X, m_oldTile.Y);
                    for (int i = 0; i <= iterations; i++)
                    {
                        drawTile.X = m_oldTile.X + i * diffX / iterations * m_selectedTiles.Width;
                        drawTile.Y = m_oldTile.Y + i * diffY / iterations * m_selectedTiles.Height;
                        OnUpdateDrawPen(drawTile);
                    }

                }
                else if (State.DrawMode == DrawMode.Rectangle)
                {
                    OnUpdateDrawRectangle(tile);
                }
            }
            UeFGlobals.MapView.IsDirty = true;
            UpdateCursor();
        }
        /// <summary>
        /// Ends the current drawing action.
        /// </summary>
        /// <param name="tile">Tile where the action starts</param>
        void OnEndActionDrawingMode(Point tile, bool shifted)
        {
            if (ActionStartedLeft)
                ActionStartedLeft = false; // end of action
            else
                throw new Exception("An action must be started in order to call OnEndAction");

            // Mode
            if (State.DrawMode == DrawMode.Pen)
            {
                OnEndDrawPen(tile);
            }
            else if (State.DrawMode == DrawMode.Rectangle)
            {
                OnEndDrawRectangle(tile);
            }

            // Fires the event
            if(DrawingActionEnded != null)
                DrawingActionEnded(m_currentRecording);

            m_currentRecording = null;
            m_dataSnapshot = null;

            // Runs the garbage collection
            GC.Collect();
            UeFGlobals.MapView.IsDirty = true;
            UpdateCursor();
        }
        #endregion
        /* --------------------------------------------------------------
         * Utils
         * ------------------------------------------------------------*/
        #region Utils
        /// <summary>
        /// Gets the object located at the given position.
        /// Returns null if there is not.
        /// </summary>
        /// <param name="tile"></param>
        UeFGame.GameObjects.GameObject GetObjectAt(Point tile)
        {
            float zoom = UeFGlobals.MapView.GraphicsManager.RenderOptions.Zoom;

            // Priority to the selected object.
            if(m_selectedObject != null)
                if(IsInAABB(tile, m_selectedObject.BoundingBox))
                    return m_selectedObject;

            UeFGame.GameObjects.GameObject objTop = null;
            // Check the objects
            foreach (UeFGame.GameObjects.GameObject obj in UeFGlobals.MapDataWrapper.GameObjects)
            {
                if (IsInAABB(tile, obj.BoundingBox))
                {
                    if (objTop == null)
                        objTop = obj;
                    else if (obj.Modules.Base.Z > objTop.Modules.Base.Z)
                        objTop = obj;
                }
            }
            return objTop;
        }
        /// <summary>
        /// Returns the objects contained in the given rectangle.
        /// </summary>
        /// <param name="rect">Rectangle in pixels.</param>
        /// <returns></returns>
        List<GameObject> GetObjectsIn(Rectangle? container)
        {
            Rectangle rect;
            if (container.HasValue)
                rect = container.Value;
            else
                return new List<GameObject>();

            // Scrolled pos in display units.
            Vector2 pos = CU.ToDisplayUnits(MouseToSim(new Point(rect.X, rect.Y)));
            var rect1 = new Microsoft.Xna.Framework.Rectangle((int)pos.X, (int)pos.Y,
               (int)( rect.Width / Zoom), (int)(rect.Height / Zoom));

            List<GameObject> gameObjects = new List<GameObject>();
            foreach (UeFGame.GameObjects.GameObject obj in UeFGlobals.MapDataWrapper.GameObjects)
            {
                var aabb = obj.BoundingBox;
                Vector2 BottomRight = CU.ToDisplayUnits(aabb.UpperBound);
                Vector2 TopLeft = CU.ToDisplayUnits(aabb.LowerBound);
                Vector2 Size = BottomRight - TopLeft;
                var objRect = new Microsoft.Xna.Framework.Rectangle((int)TopLeft.X, (int)TopLeft.Y,
                                                                    (int)Size.X, (int)Size.Y);
                if (rect1.Intersects(objRect))
                    gameObjects.Add(obj);

            }
            return gameObjects;
        }
        /// <summary>
        /// Gets the objects located at the given position.
        /// </summary>
        /// <param name="tile"></param>
        List<UeFGame.GameObjects.GameObject> GetObjectsAt(Point tile)
        {
            List<UeFGame.GameObjects.GameObject> objects = new List<UeFGame.GameObjects.GameObject>();
            // Check the objects
            foreach (UeFGame.GameObjects.GameObject obj in UeFGlobals.MapDataWrapper.GameObjects)
            {
                if (IsInAABB(tile, obj.BoundingBox))
                {
                    objects.Add(obj);
                }
            }
            return objects;
        }
        /// <summary>
        /// Returns true if the given point is in the given AABB.
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="aabb"></param>
        /// <returns></returns>
        bool IsInAABB(Point pt, FarseerPhysics.Collision.AABB aabb)
        {
            // aabb = new FarseerPhysics.Collision.AABB(aabb.UpperBound, aabb.LowerBound);
            Vector2 vect = MouseToSim(pt);
            return aabb.Contains(ref vect);
        }
        /// <summary>
        /// Converts a point in mouse coordinates to a Simulation value.
        /// </summary>
        /// <returns></returns>
        Vector2 MouseToSim(Point pt)
        {
            return new Vector2(CU.ToSimUnits(pt.X - UeFGlobals.MapView.AutoScrollPosition.X) / UeFGlobals.MapView.GraphicsManager.RenderOptions.Zoom,
                CU.ToSimUnits(pt.Y - UeFGlobals.MapView.AutoScrollPosition.Y)/UeFGlobals.MapView.GraphicsManager.RenderOptions.Zoom);
        }
        #endregion
        /* --------------------------------------------------------------
         * Macros
         * ------------------------------------------------------------*/
        #region Macros
        /// <summary>
        /// Undoes the last operation.
        /// </summary>
        public void Undo()
        {
            if (!MacroRecorder.CanUndo())
                return;
            MacroUnit unit = MacroRecorder.Undo();
            if (unit is UeFTileMacroUnit)
                UndoMacroUnit((UeFTileMacroUnit)unit);
            else
                unit.Undo();
        }
        /// <summary>
        /// Redoes the previously canceled operation.
        /// </summary>
        public void Redo()
        {
            if (!MacroRecorder.CanRedo())
                return;
            MacroUnit unit = MacroRecorder.Redo();
            if (unit is UeFTileMacroUnit)
                RedoMacroUnit((UeFTileMacroUnit)unit);
            else
                unit.Redo();
        }
        #endregion
        /* --------------------------------------------------------------
         * Right click
         * ------------------------------------------------------------*/
        #region Right click
        /// <summary>
        /// Starts an action proceeded by right clicking.
        /// </summary>
        /// <param name="mousePos"></param>
        public void OnStartRightAction(Point mousePos)
        {
            // Return if the control is not OK
            if (!IsOk)
                return;

            if (ActionStartedLeft)
                AbortAction();

            switch (this.State.Mode)
            {
                case ControlerMode.Tile:
                    OnStartSelection(ToTile(mousePos));
                    break;
                case ControlerMode.GameObjects:
                    StartGameObjectRightAction(mousePos);
                    break;
                case ControlerMode.GameObjectsEditMode:
                    StartGameObjectEditAction(mousePos, true);
                    break;
            }
        }
        /// <summary>
        /// Update the selection process
        /// </summary>
        /// <param name="mousePos"></param>
        public void OnUpdateRightAction(Point mousePos)
        {
            // Return if the control is not OK
            if (!IsOk)
                return;

            switch (this.State.Mode)
            {
                case ControlerMode.Tile:
                    OnUpdateSelection(ToTile(mousePos));
                    break;
            }
        }
        /// <summary>
        /// Ends the selection process
        /// </summary>
        /// <param name="tile"></param>
        public void OnEndRightAction(Point mousePos)
        {
            // Return if the control is not OK
            if (!IsOk)
                return;

            switch (this.State.Mode)
            {
                case ControlerMode.Tile:
                    OnEndSelection(ToTile(mousePos));
                    break;
                case ControlerMode.GameObjectsEditMode:
                    EndGameObjectEditAction(ToTile(mousePos));
                    break;
            }
        }
        #endregion

        #region Drawing
        #region Routines
        /* ------------------------------------------------------------------------------------------
         * Variables
         * ----------------------------------------------------------------------------------------*/
        #region Variables
        /// <summary>
        /// Represents the tile processed at last frame
        /// </summary>
        Point m_oldTile;
        /// <summary>
        /// MacroUnit used for edition : registers what is modified, and what is not.
        /// </summary>
        UeFTileMacroUnit m_currentRecording;
        /// <summary>
        /// Matrix of selected tiles.
        /// This is used because we will need to have the ability to copy tiles, so
        /// we must create a selection matrix instead of just accessing the SelectedArea of the
        /// TileSelector.
        /// This matrix is updated each time the user selects an area in the TileSelector or in the Map 
        /// (for copy/paste of tiles).
        /// </summary>
        int[][] m_selectionMatrix = new int[1][] { new int[1] { 0 } };
        /// <summary>
        /// Data snapshot
        /// </summary>
        TileDataSnapshot m_dataSnapshot;
        /// <summary>
        /// Starting point of the selection.
        /// </summary>
        Point m_startPoint;
        /// <summary>
        /// Rect of the selected tiles.
        /// In the drawing only the Width and Height are used, but the X and Y coordinates are usefull
        /// to memorize the selected tiles from the map selection.
        /// </summary>
        Rectangle m_selectedTiles = new Rectangle(0, 0, 1, 1);
        /// <summary>
        /// Represents the rect of the cursor.
        /// </summary>
        Rectangle m_cursorRect = new Rectangle(0, 0, 1, 1);
        #endregion
        /* ------------------------------------------------------------------------------------------
         * Properties
         * ----------------------------------------------------------------------------------------*/
        #region Properties
        /// <summary>
        /// Gets the cursor rect in tiles.
        /// </summary>
        public Rectangle CursorRect
        {
            get { return m_cursorRect; }
        }
        /// <summary>
        /// Gets the selected tiles
        /// </summary>
        Rectangle GetSelectedTiles
        {
            get { return m_selectedTiles; }
        }
        /// <summary>
        /// Shortcut to the data
        /// </summary>
        int[][,] TileData
        {
            get { return UeFGlobals.MapDataWrapper.Map.TileIds; }
        }
        /// <summary>
        /// Gets the current active layer
        /// </summary>
        int ActiveLayer
        {
            get { return UeFGlobals.MapView.GraphicsManager.RenderOptions.ActiveLayer; }
        }
        /// <summary>
        /// True if the autotiles must be checked, false otherwise
        /// </summary>
        bool IsAutotileCheckingEnabled
        {
            get { return false; }
            set { }
        }
        /// <summary>
        /// See MapDataWrapper.IsSameTile
        /// </summary>
        bool IsSameTile(int id1, int id2)
        {
            return id1 == id2;
        }
        #endregion
        /* ------------------------------------------------------------------------------------------
         * Draw Pen
         * ----------------------------------------------------------------------------------------*/
        #region Draw Pen

        /// <summary>
        /// Starts drawing tiles with the point-by-point method
        /// </summary>
        /// <param name="tile">position of the mouse in tiles</param>
        void OnStartDrawPen(Point tile)
        {
            int oldId;
            m_currentRecording = new UeFTileMacroUnit();
            for (int x = 0; x < Math.Min(GetSelectedTiles.Width, TileData[0].GetLength(0) - tile.X); x++)
            {
                for (int y = 0; y < Math.Min(GetSelectedTiles.Height, TileData[0].GetLength(1) - tile.Y); y++)
                {
                    oldId = TileData[ActiveLayer][x + tile.X, y + tile.Y];
                    TileData[ActiveLayer][x + tile.X,y + tile.Y] = GetId(x, y);
                    int caca = GetId(x, y);
                    // Records this action in the current macro recording
                    m_currentRecording.Add(x + tile.X, y + tile.Y, ActiveLayer,
                        TileData[ActiveLayer][x + tile.X, y + tile.Y], oldId);
                }
            }
            UeFGlobals.MapView.IsDirty = true;
        }
        /// <summary>
        /// Updates drawing tiles with the point-by-point method
        /// It is made that way : If the user comes again on a drawed part, 
        /// the drawed part must not change. So the algorithme is a little bit
        /// more complex, as the good tile id must be selected.
        /// </summary>
        /// <param name="tile">position of the mouse in tiles</param>
        void OnUpdateDrawPen(Point tile)
        {
            UpdateDrawPenShifted(tile);
            Common.Globals.MapView.IsDirty = true;
            
        }
        /// <summary>
        /// Updates drawing with the pen. This function is called by OnUpdateDrawPen if the AutotileChecking is 
        /// disabled.
        /// </summary>
        /// <param name="tile"></param>
        void UpdateDrawPenShifted(Point tile)
        {
            int newId;
            int oldId;
            for (int x = 0; x < Math.Max(0, Math.Min(GetSelectedTiles.Width, TileData[0].GetLength(0) - tile.X)); x++)
            {
                for (int y = 0; y < Math.Max(0, Math.Min(GetSelectedTiles.Height, TileData[0].GetLength(1) - tile.Y)); y++)
                {
                    // Offsets the start points, to tile.X - m_startPoint.X will always be positive
                    if (tile.X < m_startPoint.X)
                        m_startPoint.X -= GetSelectedTiles.Width;
                    if (tile.Y < m_startPoint.Y)
                        m_startPoint.Y -= GetSelectedTiles.Height;

                    // Creating new tile id
                    newId = GetId(
                        (Math.Abs(tile.X - m_startPoint.X) + x) % GetSelectedTiles.Width,
                        (Math.Abs(tile.Y - m_startPoint.Y) + y) % GetSelectedTiles.Height);

                    // Takes the old id from the snapshot
                    oldId = m_dataSnapshot[x + tile.X, y + tile.Y];
                    
                    // Only if the new id is different 
                    if (oldId != newId)
                    {
                        TileData[ActiveLayer][x + tile.X,y + tile.Y] = newId;
                        // Records this action in the current macro recording
                        m_currentRecording.Add(x + tile.X, y + tile.Y, ActiveLayer, newId, oldId);
                    }
                }
            }
        }
        /// <summary>
        /// Ends drawing tiles with the point-by-point method and registers the macro.
        /// </summary>
        /// <param name="tile">position of the mouse in tiles</param>
        void OnEndDrawPen(Point tile)
        {
            // Finally records the macro.
            MacroRecorder.Append(m_currentRecording);
        }
        /// <summary>
        /// Aborts the draw pen action.
        /// </summary>
        void OnAbortDrawPen()
        {
            // Restores the snapshot
            RestoreSnapshot();
            ActionStartedLeft = false;
            Common.Globals.MapView.IsDirty = true;
        }
        #endregion
        /* ------------------------------------------------------------------------------------------
         * Draw Rectangle
         * ----------------------------------------------------------------------------------------*/
        #region Draw Rectangle
        /// <summary>
        /// Starts drawing tiles with the rectangle method
        /// </summary>
        /// <param name="tile">position of the mouse in tiles</param>
        void OnStartDrawRectangle(Point tile)
        {
            int oldId;
            // Creates a new recording unit
            m_currentRecording = new UeFTileMacroUnit();
            // Memorizes the old id for the macro
            oldId = TileData[ActiveLayer][tile.X, tile.Y];
            // Affects the new value to the tiledata
            TileData[ActiveLayer][tile.X, tile.Y]= (short)GetId(0, 0);
            // Records this action in the current macro recording
            m_currentRecording.Add(tile.X, tile.Y, ActiveLayer,
                TileData[ActiveLayer][tile.X, tile.Y], oldId);

            // Marks the screen as dirty
            Common.Globals.MapView.IsDirty = true;
        }
        /// <summary>
        /// Updates drawing tiles with the rectangle method.
        /// </summary>
        /// <param name="tile">position of the mouse in tiles</param>
        void OnUpdateDrawRectangle(Point tile)
        {

            Rectangle selRect = GetRect(tile, m_startPoint);
            // Clears what has previously be done last frame
            Rectangle oldSel = GetRect(m_startPoint, m_oldTile);
            // Enlarges the rectangle of 1 pixel at each edge.
            if (oldSel.X != 0)
            {
                oldSel.X -= 1;
                oldSel.Width += 1;
            }
            if (oldSel.Y != 0)
            {
                oldSel.Y -= 1;
                oldSel.Height += 1;
            }

            if (oldSel.X + oldSel.Width < TileData[0].GetLength(0) - 1)
                oldSel.Width += 1;
            else
                oldSel.Width = Math.Min(TileData[0].GetLength(0)-1, TileData[0].GetLength(0) - oldSel.X);

            if (oldSel.Y + oldSel.Height < TileData[0].GetLength(1) - 1)
                oldSel.Height += 1;
            else
                oldSel.Height = Math.Min(TileData[0].GetLength(1)-1, TileData[0].GetLength(1) - oldSel.Y);

            RestoreSnapshot(oldSel);
            // Clears the macro unit
            m_currentRecording.TileInfos.Clear();
            // Use a slightly different algorythm if the autotile checking is disabled
            if (!IsAutotileCheckingEnabled)
            {
                UpdateDrawRectangleShifted(selRect);
                return;
            }
            int newId;
            int oldId;
            //GC.Collect();
            int xMax = Math.Max(0, Math.Min(selRect.Width, TileData[0].GetLength(0) - selRect.X));
            int yMax = Math.Max(0, Math.Min(selRect.Height, TileData[0].GetLength(1) - selRect.Y));
            for (int x = 0; x < xMax; x++)
            {
                for (int y = 0; y < yMax; y++)
                {
                    // Creating new tile id
                    newId = GetId(
                        (x) % GetSelectedTiles.Width,
                        (y) % GetSelectedTiles.Height);
                    oldId = m_dataSnapshot[x + selRect.X, y + selRect.Y];//TileData[x + selRect.X][y + selRect.Y][ActiveLayer];
                    // Tile info of the current tile
                    UeFTileInfo thisTileInfo;
                    // Only if the new id is different 
                    if (!IsSameTile(oldId, newId))
                    {
                        // Records this action in the current macro recording
                        thisTileInfo = m_currentRecording.Add(x + selRect.X, y + selRect.Y, ActiveLayer, newId, oldId);
                        TileData[ActiveLayer][x + selRect.X, y + selRect.Y] = newId;
                        //CheckAutotileAndAround(x + selRect.X, y + selRect.Y, ActiveLayer, m_currentRecording, true);
                    }
                    else
                    {
                        thisTileInfo = m_currentRecording.Add(x + selRect.X, y + selRect.Y, ActiveLayer,
                            m_dataSnapshot[x + selRect.X, y + selRect.Y],
                            m_dataSnapshot[x + selRect.X, y + selRect.Y]);
                    }
                    // Here, we record the sides if you are to the bounds of the rectangle, 
                    // in order to memorize them for the macro, and restore them as they
                    // will be modified if they are autotiles.
                    bool isOnLeft = x == 0 && selRect.X > 0;
                    bool isOnRight = x == (xMax - 1) && xMax + selRect.X < TileData[0].GetLength(0);
                    bool isOnTop = y == 0 && selRect.Y > 0;
                    bool isOnBottom = y == yMax - 1 && yMax + selRect.Y < TileData[0].GetLength(1);
                    if (isOnLeft)
                        m_currentRecording.Add(x + selRect.X - 1, y + selRect.Y, ActiveLayer,
                            m_dataSnapshot[x + selRect.X - 1, y + selRect.Y],
                            m_dataSnapshot[x + selRect.X - 1, y + selRect.Y]);
                    if (isOnRight)
                        m_currentRecording.Add(x + selRect.X + 1, y + selRect.Y, ActiveLayer,
                            m_dataSnapshot[x + selRect.X + 1, y + selRect.Y],
                            m_dataSnapshot[x + selRect.X + 1, y + selRect.Y]);
                    if (isOnTop)
                    {
                        m_currentRecording.Add(x + selRect.X, y + selRect.Y - 1, ActiveLayer,
                            m_dataSnapshot[x + selRect.X, y + selRect.Y - 1],
                            m_dataSnapshot[x + selRect.X, y + selRect.Y - 1]);
                        if (isOnLeft)
                            m_currentRecording.Add(x + selRect.X - 1, y + selRect.Y - 1, ActiveLayer,
                             m_dataSnapshot[x + selRect.X - 1, y + selRect.Y - 1],
                             m_dataSnapshot[x + selRect.X - 1, y + selRect.Y - 1]);
                        if (isOnRight)
                            m_currentRecording.Add(x + selRect.X + 1, y + selRect.Y - 1, ActiveLayer,
                             m_dataSnapshot[x + selRect.X + 1, y + selRect.Y - 1],
                             m_dataSnapshot[x + selRect.X + 1, y + selRect.Y - 1]);
                    }
                    if (isOnBottom)
                    {
                        m_currentRecording.Add(x + selRect.X, y + selRect.Y + 1, ActiveLayer,
                            m_dataSnapshot[x + selRect.X, y + selRect.Y + 1],
                            m_dataSnapshot[x + selRect.X, y + selRect.Y + 1]);
                        if (isOnLeft)
                            m_currentRecording.Add(x + selRect.X - 1, y + selRect.Y + 1, ActiveLayer,
                             m_dataSnapshot[x + selRect.X - 1, y + selRect.Y + 1],
                             m_dataSnapshot[x + selRect.X - 1, y + selRect.Y + 1]);
                        if (isOnRight)
                            m_currentRecording.Add(x + selRect.X + 1, y + selRect.Y + 1, ActiveLayer,
                             m_dataSnapshot[x + selRect.X + 1, y + selRect.Y + 1],
                             m_dataSnapshot[x + selRect.X + 1, y + selRect.Y + 1]);
                    }
                    // We are at the center, and the current tile is an autotile
                    /* AUTOTILE CODE
                     * if ((UeFGlobals.MapDataWrapper.IsValidAutotile(newId) || newId < 0))
                        if (!(isOnBottom | isOnLeft | isOnRight | isOnTop))
                        {
                            int id = RpgMapDataWrapper.GetAutotileNumber(newId) * Project.RpgGameOptions.NumberOfTilePerAutotile;

                            TileData[x + selRect.X][y + selRect.Y][ActiveLayer] = (short)id;
                            thisTileInfo.NewTilesetId = (short)id;
                        }
                    */
                }
            }
            /* AUTOTILE CODE
            // Me check only the bounds :
            for (int x = selRect.X; x < xMax + selRect.X; x++)
            {
                CheckAutotileAndAround(x, selRect.Y + yMax - 1, ActiveLayer, m_currentRecording);
                CheckAutotileAndAround(x, selRect.Y, ActiveLayer, m_currentRecording);
            }
            for (int y = selRect.Y; y < yMax + selRect.Y; y++)
            {
                CheckAutotileAndAround(selRect.X + xMax - 1, y, ActiveLayer, m_currentRecording);
                CheckAutotileAndAround(selRect.X, y, ActiveLayer, m_currentRecording);
            }
            // CheckMacroUnitAutotileAndAround(m_currentRecording, true);
             * */
            Common.Globals.MapView.IsDirty = true;
        }
        /// <summary>
        /// Updates the drawing of a rectangle, if the autotile checking is disabled.
        /// </summary>
        void UpdateDrawRectangleShifted(Rectangle selRect)
        {
            int oldId;
            int newId;
            for (int x = 0; x < Math.Max(0, Math.Min(selRect.Width, TileData[0].GetLength(0) - selRect.X)); x++)
            {
                for (int y = 0; y < Math.Max(0, Math.Min(selRect.Height, TileData[0].GetLength(1) - selRect.Y)); y++)
                {
                    // Creating new tile id
                    newId = GetId(
                        (x) % GetSelectedTiles.Width,
                        (y) % GetSelectedTiles.Height);

                    oldId = m_dataSnapshot[x + selRect.X, y + selRect.Y];//TileData[x + selRect.X][y + selRect.Y][ActiveLayer];
                    // Only if the new id is different 
                    if (oldId != newId)
                    {
                        // Records this action in the current macro recording
                        m_currentRecording.Add(x + selRect.X, y + selRect.Y, ActiveLayer, newId, oldId);
                        TileData[ActiveLayer][x + selRect.X, y + selRect.Y] = newId;
                    }
                }
            }
            Common.Globals.MapView.IsDirty = true;
        }
        /// <summary>
        /// Ends drawing tiles with the rectangle method and registers the macro.
        /// </summary>
        /// <param name="tile">position of the mouse in tiles</param>
        void OnEndDrawRectangle(Point tile)
        {
            // Records the macro
            MacroRecorder.Append(m_currentRecording);
            //CheckMacroUnitAutotileAndAround(m_currentRecording);
            if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                foreach (UeFTileInfo u in m_currentRecording.TileInfos)
                {
                    if (u.TileX == 2)
                    {
                        int ab = 0;
                        int a = ab + 5;
                    }
                }
            }
        }
        /// <summary>
        /// Aborts the DrawRectangle action.
        /// </summary>
        void OnAbortDrawRectangle()
        {
            RestoreSnapshot();
            ActionStartedLeft = false;
            Common.Globals.MapView.IsDirty = true;
        }
        #endregion
        /* ------------------------------------------------------------------------------------------
         * Draw FloodFill
         * ----------------------------------------------------------------------------------------*/
        #region Draw FloodFill
        /// <summary>
        /// Starts drawing tiles with the rectangle method
        /// </summary>
        /// <param name="tile">position of the mouse in tiles</param>
        void OnStartDrawFloodFill(Point tile)
        {
            // Creates a new recording unit
            m_currentRecording = new UeFTileMacroUnit();
            // Starts the filling
            m_dataSnapshot = null;
            FloodFillTile(tile.X, tile.Y);
            // Marks the screen as dirty
            Common.Globals.MapView.IsDirty = true;
            MacroRecorder.Append(m_currentRecording);
            ActionStartedLeft = false;
            // We call that as we end the action with m_action_started
            // => the event won't be fired from OnEndAction
            if(DrawingActionEnded != null)
                DrawingActionEnded(m_currentRecording);
        }
        /// <summary>
        /// Starts the floodfilling operation.
        /// </summary>
        void FloodFillTile(int x, int y)
        {
            // Old Id : all the tiles with the same id, next to each other will be replaced.
            int oldId = TileData[ActiveLayer][x,y];
            /*
             * if (!RpgMapDataWrapper.IsValidAutotile(oldId) && oldId > 0 && oldId < Project.RpgGameOptions.NumberOfTilePerAutotile)
                oldId = RpgMapDataWrapper.GetAutotileNumber(oldId);
             * */
            // Cancel operation if the tile used is the same as the tile clicked.
            if (IsSameTile(FloodFillMakeNewId(x, y), oldId))
                return;

            // Queue of points
            List<Point> queue = new List<Point>(256);
            bool[,] isChecked = new bool[TileData[0].GetLength(0), TileData[0].GetLength(1)];
            Point p = new Point(x, y);
            isChecked[x, y] = true;
            queue.Add(p);
            int loops = 0;
            while (queue.Count() != 0)
            {
                p = queue[0];
                if (IsSameTile(TileData[ActiveLayer][p.X,p.Y], oldId))
                {
                    loops += 1;
                    TileData[ActiveLayer][p.X,p.Y] = FloodFillMakeNewId(p.X, p.Y);
                    m_currentRecording.Add(p.X, p.Y, ActiveLayer, TileData[ActiveLayer][p.X,p.Y], oldId);
                    if (p.X < TileData[0].GetLength(0) - 1)
                        if (IsSameTile(TileData[ActiveLayer][p.X + 1,p.Y], oldId)
                            && !IsSameTile(TileData[ActiveLayer][p.X + 1,p.Y], FloodFillMakeNewId(p.X + 1, p.Y)))
                        {
                            if (!isChecked[p.X + 1, p.Y])
                            {
                                queue.Add(new Point(p.X + 1, p.Y));
                                isChecked[p.X + 1, p.Y] = true;
                            }
                        }
                    if (p.X > 0)
                        if (IsSameTile(TileData[ActiveLayer][p.X - 1,p.Y], oldId)
                            && !IsSameTile(TileData[ActiveLayer][p.X - 1,p.Y], FloodFillMakeNewId(p.X - 1, p.Y)))
                        {
                            if (!isChecked[p.X - 1, p.Y])
                            {
                                queue.Add(new Point(p.X - 1, p.Y));
                                isChecked[p.X - 1, p.Y] = true;
                            }
                        }
                    if (p.Y < TileData[0].GetLength(1) - 1)
                        if (IsSameTile(TileData[ActiveLayer][p.X,p.Y + 1], oldId)
                            && !IsSameTile(TileData[ActiveLayer][p.X,p.Y + 1], FloodFillMakeNewId(p.X, p.Y + 1)))
                        {
                            if (!isChecked[p.X, p.Y + 1])
                            {
                                queue.Add(new Point(p.X, p.Y + 1));
                                isChecked[p.X, p.Y + 1] = true;
                            }
                        }
                    if (p.Y > 0)
                        if (IsSameTile(TileData[ActiveLayer][p.X,p.Y - 1], oldId)
                            && !IsSameTile(TileData[ActiveLayer][p.X,p.Y - 1], FloodFillMakeNewId(p.X, p.Y - 1)))
                        {
                            if (!isChecked[p.X, p.Y - 1])
                            {
                                queue.Add(new Point(p.X, p.Y - 1));
                                isChecked[p.X, p.Y - 1] = true;
                            }
                        }
                }
                queue.RemoveAt(0);
            }
            /*CheckMacroUnitAutotileAndAround(m_currentRecording);*/
            return;
        }
        /// <summary>
        /// Returns the correct id for the given tile during the floodfilling operation.
        /// </summary>
        short FloodFillMakeNewId(int x, int y)
        {
            // Offset
            if (x < m_startPoint.X)
                m_startPoint.X -= GetSelectedTiles.Width;
            if (y < m_startPoint.Y)
                m_startPoint.Y -= GetSelectedTiles.Height;
            // Creating new tile id
            return (short)GetId(
                Math.Abs((x - m_startPoint.X) % GetSelectedTiles.Width),
                Math.Abs((y - m_startPoint.Y) % GetSelectedTiles.Height));
        }
        #endregion
        /* ------------------------------------------------------------------------------------------
         * Selection (Right button action)
         * ----------------------------------------------------------------------------------------*/
        #region Selection
        /// <summary>
        /// Starts the selection process.
        /// </summary>
        /// <param name="tile"></param>
        void OnStartSelection(Point tile)
        {
            SelectionStarted = true;
            m_startPoint = new Point(tile.X, tile.Y);
            m_selectedTiles = new Rectangle(tile.X, tile.Y, 1, 1);
            UpdateCursor();
        }
        /// <summary>
        /// Updates the selection process.
        /// </summary>
        /// <param name="tile"></param>
        void OnUpdateSelection(Point tile)
        {
            m_selectedTiles = new Rectangle(Math.Min(m_startPoint.X, tile.X),
                Math.Min(m_startPoint.Y, tile.Y), Math.Abs(m_startPoint.X - tile.X) + 1,
                Math.Abs(m_startPoint.Y - tile.Y) + 1);
            UpdateCursor();
        }
        /// <summary>
        /// Ends the selection process.
        /// </summary>
        /// <param name="tile"></param>
        void OnEndSelection(Point tile)
        {
            UpdateCursor();
            OnMapAreaSelected();
            SelectionStarted = false;
        }
        #endregion
        /* ------------------------------------------------------------------------------------------
         * Autotile Checking
         * ----------------------------------------------------------------------------------------*/
        #region Autotile Checking
        /* AUTOTILE CODE
        /// <summary>
        /// Checks the autotiles and gives them the right autotile id.
        /// For each autotile checked, checks if there are some changes to make on the autotiles around.
        /// </summary>
        /// <param name="macroUnit"></param>
        void CheckMacroUnitAutotileAndAround(UeFTileMacroUnit macroUnit, bool record)
        {
            for (int i = 0; i < macroUnit.TileInfos.Count; i++)
            {
                UeFTileInfo info = macroUnit.TileInfos[i];
                // The tile as not been assigned yet.
                if (info.NewTilesetId <= 0 || info.NewTilesetId >= Project.GameOptions.NumberOfAutotileID)
                {
                    CheckAutotileAndAround(info.TileX, info.TileY, info.Layer, macroUnit, record);

                }
            }
        }
        /// <summary>
        /// Checks the autotiles and gives them the right autotile id.
        /// </summary>
        /// <param name="macroUnit"></param>
        void CheckMacroUnitAutotile(UeFTileMacroUnit macroUnit, bool record)
        {
            for (int i = 0; i < macroUnit.TileInfos.Count; i++)
            {
                UeFTileInfo info = macroUnit.TileInfos[i];
                // The tile as not been assigned yet.
                if (info.NewTilesetId < 0)
                {
                    CheckAutotile(info.TileX, info.TileY, info.Layer, macroUnit, record);
                }
            }
        }
        void CheckMacroUnitAutotile(UeFTileMacroUnit macroUnit)
        {
            CheckMacroUnitAutotile(macroUnit, true);
        }
        void CheckMacroUnitAutotileAndAround(UeFTileMacroUnit macroUnit)
        {
            CheckMacroUnitAutotileAndAround(macroUnit, true);
        }
        */
        /* AUTOTILE CODE
        /// <summary>
        /// Checks an autotile and the others around it.
        /// </summary>
        void CheckAutotileAndAround(int x, int y, int z, UeFTileMacroUnit macroUnit, bool record)
        {
            // CheckAutotile(info.TileX, info.TileY, info.Layer, macroUnit);
            for (int xi = Math.Max(0, x - 1); xi <= Math.Min(TileData[0].GetLength(0), x + 1); xi++)
            {
                for (int yi = Math.Max(0, y - 1); yi <= Math.Min(TileData[0].GetLength(1), y + 1); yi++)
                {
                    CheckAutotile(xi, yi, z, macroUnit, record);
                }
            }
        }
        void CheckAutotileAndAround(int x, int y, int z, UeFTileMacroUnit macroUnit)
        {
            CheckAutotileAndAround(x, y, z, macroUnit, true);
        }
        /// <summary>
        /// Checks a single autotile at the given position
        /// </summary>
        void CheckAutotile(int x, int y, int z, UeFTileMacroUnit macroUnit, bool record)
        {
            if (x < 0 || y < 0 || x >= TileData[0].GetLength(0) || y >= TileData[0].GetLength(1))
                return;
            if (!(UeFGlobals.MapDataWrapper.IsValidAutotile(TileData[x][y][z]) | TileData[x][y][z] < 0))
                return;
            UeFTileInfo? inf = macroUnit.GetTileInfoAt(x, y);
            if (inf.HasValue)
            {
                UeFTileInfo info = inf.Value;
                TileData[info.TileX][info.TileY][info.Layer] = RpgMapDataWrapper.GenerateAutotileId(info.TileX, info.TileY, info.Layer);
                if (record)
                {
                    info.NewTilesetId = RpgMapDataWrapper.GenerateAutotileId(info.TileX, info.TileY, info.Layer);
                    if (m_dataSnapshot != null)
                        info.OldTilesetId = m_dataSnapshot[info.TileX, info.TileY];
                }
            }
            else
            {
                short oldId = TileData[x][y][z];
                TileData[x][y][z] = RpgMapDataWrapper.GenerateAutotileId(x, y, z);
                if (record)
                {
                    if (m_dataSnapshot != null)
                        oldId = m_dataSnapshot[x, y];
                    macroUnit.Add(x, y, z, TileData[x][y][z], oldId);
                }
            }
        }
        void CheckAutotile(int x, int y, int z, UeFTileMacroUnit macroUnit)
        {
            CheckAutotile(x, y, z, macroUnit, true);
        } */
        #endregion
        /* ------------------------------------------------------------------------------------------
         * Macros and Data handling
         * ----------------------------------------------------------------------------------------*/
        #region Macros and Data handling
        /// <summary>
        /// Undoes a whole macro unit
        /// </summary>
        /// <param name="unit"></param>
        void UndoMacroUnit(UeFTileMacroUnit unit)
        {
            foreach (UeFTileInfo info in unit.TileInfos)
            {
                TileData[info.Layer][info.TileX,info.TileY] = info.OldTilesetId;
            }
        }
        /// <summary>
        /// Redoes a whole macro unit
        /// </summary>
        /// <param name="unit"></param>
        void RedoMacroUnit(UeFTileMacroUnit unit)
        {
            foreach (UeFTileInfo info in unit.TileInfos)
            {
                TileData[info.Layer][info.TileX,info.TileY] = info.NewTilesetId;
            }
            /* CheckMacroUnitAutotileAndAround(unit, false); */
        }
        /// <summary>
        /// Restores the tiles that were on the macro unit.
        /// Only the tiles recorded in the MacroUnit are restored.
        /// </summary>
        /// <param name="unit"></param>
        void RestoreFromMacroUnit(UeFTileMacroUnit unit)
        {
            if (unit == null)
                return;
            foreach (UeFTileInfo info in unit.TileInfos)
            {
                TileData[info.Layer][info.TileX,info.TileY] = info.OldTilesetId;//m_dataSnapshot[info.TileX][info.TileY][info.Layer];
            }
        }
        /// <summary>
        /// Restore the tiles as they were on the snapshot.
        /// </summary>
        void RestoreSnapshot(Rectangle rect)
        {
            if (m_dataSnapshot == null)
                return;
            for (int x = rect.X; x < rect.Right; x++)
            {
                for (int y = rect.Y; y < rect.Bottom; y++)
                {
                    TileData[ActiveLayer][x,y] = m_dataSnapshot[x, y];
                }
            }
        }
        /// <summary>
        /// Restore the tiles as they were on the snapshot.
        /// </summary>
        void RestoreSnapshot()
        {
            if (m_dataSnapshot == null)
                return;

            for (int x = m_dataSnapshot.Sx; x < m_dataSnapshot.Width + m_dataSnapshot.Sx; x++)
            {
                for (int y = m_dataSnapshot.Sy; y < m_dataSnapshot.Height + m_dataSnapshot.Sy; y++)
                {
                    TileData[ActiveLayer][x,y] = m_dataSnapshot[x, y];
                }
            }
        }
        /* ------------------------------------------------------------------------------------------
         * Data cloning
         * ----------------------------------------------------------------------------------------*/
        #region Data Cloning
        /// <summary>
        /// Clones and return an array data
        /// </summary>
        /// <param name="data">The data to clone</param>
        /// <returns></returns>
        public short[][][] CloneData(short[][][] data)
        {
            short[][][] newData = new short[data.Count()][][];
            CloneData(data, newData);
            return newData;
        }
        /// <summary>
        /// Clones the data from src to dst.
        /// </summary>
        public void CloneData(short[][][] src, short[][][] dst)
        {
            for (int x = 0; x < src.Count(); x++)
            {
                dst[x] = new short[src[x].Count()][];
                for (int y = 0; y < src[x].Count(); y++)
                {
                    dst[x][y] = new short[src[x][y].Count()];
                    for (int layer = 0; layer < src[x][y].Count(); layer++)
                    {
                        dst[x][y][layer] = src[x][y][layer];
                    }
                }
            }
        }
        #endregion
        #endregion
        /* ------------------------------------------------------------------------------------------
         * Tools
         * ----------------------------------------------------------------------------------------*/
        #region Tools
        /// <summary>
        /// Returns a rectangle created using 2 points
        /// </summary>
        Rectangle GetRect(Point p1, Point p2)
        {
            return Common.Tools.Geometry.GetRect(p1, p2); // for backward compatibility
        }
        /// <summary>
        /// Gets the id to use.
        /// x and y are the coordinates of the SELECTED tile to use.
        /// </summary>
        int GetId(int x, int y)
        {
            return m_selectionMatrix[x][y];
        }
        #endregion
        #endregion

        #endregion
    }
}