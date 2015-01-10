using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UeFGame.GameObjects;
using System.CodeDom.Compiler;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
namespace Geex.Edit.UeF.Views
{
    /// <summary>
    /// TODO : modifier cette classe pour qu'elle prenne en compte tous les modules.
    /// </summary>
    public partial class GameObjectPropertiesView : Form
    {
        #region Variables
        /// <summary>
        /// Gameobject currently being modified.
        /// </summary>
        GameObject m_gameObject;
        List<ToolStripButton> m_modulesButtons;
        Timer m_compilationTimer;
        #endregion

        #region Methods
        public GameObjectPropertiesView()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Constructor.
        /// </summary>
        public GameObjectPropertiesView(GameObject obj)
        {
            InitializeComponent();
            m_modulesButtons = new List<ToolStripButton>();
            m_gameObject = obj;
            m_editControl.InitializeControl(obj.Modules.Base);
            m_compileButton.Click += new EventHandler(OnCompile);
            m_errorsListView.DoubleClick += new EventHandler(OnErrorDoubleClick);
            m_codeEditor.TextChanged += new EventHandler(OnLaunchDeferredCompilation);
            m_compilationTimer = new Timer();
            m_compilationTimer.Tick += new EventHandler(OnCompile);
            m_okButton.Click += new EventHandler(OnOkClicked);
            m_codeEditor.Text = obj.MBase.Script;
            LoadModuleButtons(m_gameObject);
        }

        /// <summary>
        /// Se produit lorsque le bouton ok est cliqué.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnOkClicked(object sender, EventArgs e)
        {
            m_gameObject.MBase.Script = m_codeEditor.Text;
        }

        /// <summary>
        /// Lance une compilation différée pour la vérification des erreurs.
        /// Si après un certain timer, aucun texte n'est modifié, la vérification des erreur se lance
        /// toute seule.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnLaunchDeferredCompilation(object sender, EventArgs e)
        {
            m_compilationTimer.Interval = 1000;
            m_compilationTimer.Stop();
            m_compilationTimer.Start();
        }



        /// <summary>
        /// Compile la source afin de vérifier les erreurs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnCompile(object sender, EventArgs e)
        {
            m_compilationTimer.Stop();

            MapImporter.UeFMapCompiler compiler = new MapImporter.UeFMapCompiler();

            var errors = compiler.CheckForErrors(m_codeEditor.Text);
            if (errors.HasErrors || errors.HasWarnings)
            {
                m_errorsListView.Errors = errors;
                m_warningListView.Errors = errors;
                m_codeEditor.Document.MarkerStrategy.RemoveAll((TextMarker mark) => { return true; });
                foreach (CompilerError error in errors)
                {
                    HighLightError(error);
                }
                m_codeEditor.Refresh();
            }
            else
            {
                m_errorsListView.Errors = null;
                m_codeEditor.Document.MarkerStrategy.RemoveAll((TextMarker mark) => { return true; });
                m_codeEditor.Refresh();
            }
        }
        /// <summary>
        /// Appelé lors d'un double click sur une erreur.
        /// Donne le focus à l'éditeur de texte et le place à la ligne donnée.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnErrorDoubleClick(object sender, EventArgs e)
        {
            if (m_errorsListView.SelectedIndices.Count == 0)
                return;

            int selection = m_errorsListView.SelectedIndices[0];
            int lineNumber = m_errorsListView.Errors[selection].Line - 1;
            int col = m_errorsListView.Errors[selection].Column - 1;
            m_codeEditor.Focus();
            m_codeEditor.ActiveTextAreaControl.Caret.Line = lineNumber;
            m_codeEditor.ActiveTextAreaControl.Caret.Column = col;

            TextLocation start = new TextLocation(col, lineNumber);
            TextLocation end = new TextLocation(0, lineNumber+1);
            m_codeEditor.ActiveTextAreaControl.SelectionManager.SetSelection(start, end);
        }
        /// <summary>
        /// Marque une erreur dans l'éditeur de texte.
        /// </summary>
        /// <param name="error"></param>
        void HighLightError(CompilerError error)
        {
            try
            {

                int offset = m_codeEditor.Document.GetLineSegment(error.Line - 1).Offset + error.Column - 1;
                int length = m_codeEditor.Document.GetLineSegment(error.Line - 1).TotalLength - error.Column - 1;
                if (error.Column > m_codeEditor.Document.GetLineSegment(error.Line - 1).Length)
                {
                    offset = m_codeEditor.Document.GetLineSegment(error.Line - 1).Offset;
                    length = m_codeEditor.Document.GetLineSegment(error.Line - 1).Length;
                }
                if (!error.IsWarning)
                {
                    TextMarker marker = new TextMarker(offset, length, TextMarkerType.WaveLine, Color.OrangeRed);
                    m_codeEditor.Document.MarkerStrategy.AddMarker(marker);
                }
                else
                {
                    TextMarker marker = new TextMarker(offset, length, TextMarkerType.WaveLine, Color.GreenYellow);
                    m_codeEditor.Document.MarkerStrategy.AddMarker(marker);
                }
            }
            catch { }
        }
        /// <summary>
        /// Fired when a GameObject is selected.
        /// </summary>
        /// <param name="obj"></param>
        void LoadModuleButtons(GameObject obj)
        {
            if (obj != null)
            {
                m_editControl.LoadObject(obj.Modules.Base);

                DeleteButtons();
                foreach (string moduleName in obj.Modules.GetModules().Keys)
                {
                    string moduleNameRef = moduleName;
                    ToolStripButton btn = new ToolStripButton(moduleName);
                    btn.Click += delegate(object sender, EventArgs e)
                    {
                        m_editControl.LoadObject(obj.Modules[moduleNameRef]);
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
        #endregion
    }
}
