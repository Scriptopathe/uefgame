using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Geex.Edit.Common.Extensibility;
namespace Geex.Edit
{
    public partial class ModulesChoiceForm : Form
    {
        #region Variables
        /// <summary>
        /// Modules given as argument.
        /// </summary>
        List<IModuleDeclaration> m_modules;
        /// <summary>
        /// The module choosen by the user
        /// </summary>
        IModuleDeclaration m_choosenModule;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the module choosen by the user.
        /// </summary>
        public IModuleDeclaration ChoosenModule
        {
            get { return m_choosenModule; }
        }
        /// <summary>
        /// Indicates if the choice must be remembered.
        /// </summary>
        public bool Remember
        {
            get { return m_rememberCheckbox.Checked; }
        }
        #endregion
        /// <summary>
        /// Constructor used by the winforms designer.
        /// </summary>
        public ModulesChoiceForm()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Creates a new instance of modules choice form.
        /// </summary>
        /// <param name="modules">The modules list to display.</param>
        public ModulesChoiceForm(List<IModuleDeclaration> modules)
        {
            m_modules = modules;
            InitializeComponent();
            InitLang();
            InitControls();
            this.CenterToParent();
        }


        #region Initialization
        /// <summary>
        /// Initializes the controls.s
        /// </summary>
        void InitControls()
        {
            // Set up the list
            foreach (IModuleDeclaration module in m_modules)
            {
                m_modulesListBox.Items.Add(module.GetModuleName() + " - " + module.GetModuleKind() + " v" + module.GetModuleVersion());
            }
            m_modulesListBox.SelectedIndex = 0;
            m_modulesListBox.SelectionMode = SelectionMode.One;
            m_modulesListBox.SelectedIndexChanged += new EventHandler(OnSelectedModuleChanged);
            m_choosenModule = m_modules.First();

            // Event for this window
            this.m_okButton.Click += new EventHandler(OnCheckRemember);
        }
        /// <summary>
        /// Checks if the remember checkbox is checked, and applys the necessary settings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnCheckRemember(object sender, EventArgs e)
        {
            if (m_rememberCheckbox.Checked)
            {

            }
        }

        /// <summary>
        /// Inits the controls text using the current language.
        /// </summary>
        void InitLang()
        {
            this.Text = Common.Lang.I["ModuleChoiceForm_Caption"];
            this.m_okButton.Text = Common.Lang.I["Global_OK"];
            this.m_label.Text = Common.Lang.I["ModuleChoiceForm_LabelText"];
            this.m_rememberCheckbox.Text = Common.Lang.I["ModuleChoiceForm_RememberMyChoice"];
        }
        #endregion

        #region Event calls
        /// <summary>
        /// Called when the module selection has changed.
        /// It will asign the choosen module to its new value.
        /// </summary>
        void OnSelectedModuleChanged(object sender, EventArgs e)
        {
            m_choosenModule = m_modules[m_modulesListBox.SelectedIndex];
        }
        #endregion
    }
}
