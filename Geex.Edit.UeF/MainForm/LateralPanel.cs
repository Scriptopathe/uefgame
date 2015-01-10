using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Geex.Edit.UeF.MainForm
{
    public partial class LateralPanel : UserControl
    {
        #region Variables
        List<ToolStripButton> m_modulesButtons = new List<ToolStripButton>();

        #endregion

        #region Properties
        /// <summary>
        /// Gets the GameObjectPicker.
        /// </summary>
        public GameObjectPicker GameObjectPicker
        {
            get { return m_gameObjectPicker; }
        }

        #endregion

        /// <summary>
        /// Lateral panel class.
        /// </summary>
        public LateralPanel(Control container)
        {
            base.Parent = container;
            InitializeComponent();
            InitializeStuff();
            InitializeEvents();
        }
        /// <summary>
        /// Initialize the events.
        /// </summary>
        public void InitializeEvents()
        {
            UeFGlobals.MapView.Controler.GameObjectSelected += new MapView.GameObjectSelectedEventHandler(Controler_GameObjectSelected);
            UeFGlobals.MapView.Controler.GameObjectActionEnded += new MapView.GameObjectActionEndedEventHandler(Controler_GameObjectActionEnded);
            m_objectEditControl.PropertyValueChanged += new Views.ObjectEditControl.PropertyValueChangedEventHandler(m_objectEditControl_PropertyValueChanged);
        }


        /// <summary>
        /// Initialize the other stuff.
        /// </summary>
        void InitializeStuff()
        {
            // Creating the map tree view
            MapTreeView treeView = new MainForm.MapTreeView();
            Common.Globals.MapTreeView = treeView;

            treeView.Dock = DockStyle.Fill;
            treeView.Scrollable = true;
            m_splitter.Panel2.Controls.Add(treeView);

            // Setup of dock etc...
            this.Dock = DockStyle.Left;
            this.MinimumSize = new Size(300, -1);
            m_splitter.PerformLayout();
            this.PerformLayout();
            // Initialize ObjectEditor
            m_objectEditControl.InitializeControl();
        }

        #region Events
        /// <summary>
        /// Fired when a GameObject is selected.
        /// </summary>
        /// <param name="obj"></param>
        void Controler_GameObjectSelected(UeFGame.GameObjects.GameObject obj)
        {
            if (obj != null)
            {
                m_objectEditControl.LoadObject(obj.Modules.Base);

                DeleteButtons();
                foreach (string moduleName in obj.Modules.GetModules().Keys)
                {
                    string moduleNameRef = moduleName;
                    ToolStripButton btn = new ToolStripButton(moduleName);
                    btn.Click += delegate(object sender, EventArgs e)
                    {
                        m_objectEditControl.LoadObject(obj.Modules[moduleNameRef]);
                    };
                    m_moduleStrip.Items.Add(btn);
                    m_modulesButtons.Add(btn);
                }
            }
        }

        /// <summary>
        /// Delete the buttons.
        /// </summary>
        void DeleteButtons()
        {
            foreach (ToolStripButton btn in m_modulesButtons)
            {
                btn.Dispose();
            }
            m_modulesButtons.Clear();
            m_moduleStrip.Items.Clear();
        }
        /// <summary>
        /// Fired when a game object action ended in the MapView.
        /// Updates the ObjectEditControl.
        /// </summary>
        /// <param name="unit"></param>
        void Controler_GameObjectActionEnded(MapView.MacroUnit unit)
        {
            m_objectEditControl.ReloadObjectValues();
        }
        /// <summary>
        /// This event is called when a property changed in the object control.
        /// </summary>
        void m_objectEditControl_PropertyValueChanged(object obj, string propertyName, object newValue)
        {
            UeFGlobals.MapView.Controler.SelectedObject.Initialize();
            UeFGlobals.MapView.IsDirty = true;
        }

        #endregion
    }
}
