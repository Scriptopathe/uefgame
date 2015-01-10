using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
namespace Geex.Edit.UeF.MainForm
{
    /// <summary>
    /// Control that enables the user to select game objects.
    /// </summary>
    public partial class GameObjectPicker : UserControl
    {
        /* ----------------------------------------------------------------
         * Event / Delegates
         * --------------------------------------------------------------*/
        #region Events / Delegates
        public delegate void GameObjectPickedEventHandler(Type type);
        /// <summary>
        /// Fired when a Game object type is picked through the list
        /// </summary>
        public event GameObjectPickedEventHandler GameObjectPicked;
        #endregion
        /* ----------------------------------------------------------------
         * Variables
         * --------------------------------------------------------------*/
        #region Variables
        /// <summary>
        /// List of the types (1: GameObject, 2: (string, module))
        /// </summary>
        List<Tuple<Type, Dictionary<string, Type>>> m_typesList;

        #endregion
        /* ----------------------------------------------------------------
         * Properties
         * --------------------------------------------------------------*/
        #region Properties
        /// <summary>
        /// Gets the selected GameObject type of the GameObjectPicker
        /// </summary>
        public Type SelectionObjectType
        {
            get { return m_typesList[m_objectsList.SelectedIndex].Item1; }
        }
        /// <summary>
        /// Gets the selected modules.
        /// </summary>
        public Dictionary<string, Type> SelectionModules
        {
            get { return m_typesList[m_objectsList.SelectedIndex].Item2; }
        }
        #endregion
        /* ----------------------------------------------------------------
         * Methods
         * --------------------------------------------------------------*/
        #region Methods
        /// <summary>
        /// Creates a new GameObjectPicker instance.
        /// </summary>
        public GameObjectPicker()
        {
            InitializeComponent();
            if(UeFGame.Globals.ExecuteInEditor != false) // <=>DesignMode
                SetupGameObjectsList();
            InitializeEvents();
        }
        /// <summary>
        /// Subscribe to the events needed.
        /// </summary>
        void InitializeEvents()
        {

        }
        /// <summary>
        /// Sets up the game object list.
        /// </summary>
        void SetupGameObjectsList()
        {
            Assembly uef = Assembly.GetAssembly(typeof(UeFGame.GameObjects.GameObject));
            m_typesList = new List<Tuple<Type, Dictionary<string, Type>>>();
            foreach (Type type in uef.GetTypes())
            {
                UeFGame.Editor.EditableGameObjectAttribute[] attributes = (UeFGame.Editor.EditableGameObjectAttribute[])type.GetCustomAttributes(typeof(UeFGame.Editor.EditableGameObjectAttribute), false);
                // If there is one EditableGameObjectAttribute in the list.
                if (attributes.Count() != 0)
                {
                    m_objectsList.Items.Add(type.Name);
                    m_typesList.Add(new Tuple<Type, Dictionary<string, Type>>(type, attributes.First().Modules));
                }
            }
            m_objectsList.SelectedIndex = 0;
        }
        #endregion

    }
}
