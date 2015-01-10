using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.CodeDom.Compiler;
namespace Geex.Edit.UeF.Views
{
    public enum ErrorDisplay
    {
        Errors = 0x01,
        Warning = 0x02,
        Both = Errors | Warning
    }
    /// <summary>
    /// Contrôle permettant la visualisation d'une liste d'erreurs.
    /// </summary>
    public partial class ErrorsListView : ListView
    {
        #region Variables
        private CompilerErrorCollection m_errors;
        /// <summary>
        /// Définit ou obtient la collection d'erreurs à afficher.
        /// </summary>
        public CompilerErrorCollection Errors
        {
            get { return m_errors; }
            set { m_errors = value; SetupList(); }
        }

        public ErrorDisplay Display { get; set; }
        #endregion
        
        #region Methods
        /// <summary>
        /// Constructeur.
        /// </summary>
        public ErrorsListView()
        {
            Display = ErrorDisplay.Errors;
            InitializeComponent();

        }
        /// <summary>
        /// Mets à jour la liste pour accueillir les erreurs.
        /// </summary>
        public void SetupList()
        { 
            Items.Clear();
            if (Errors == null)
                return;

            foreach (CompilerError error in Errors)
            {
                if (error.IsWarning && ((Display & ErrorDisplay.Warning) == 0))
                    continue;
                if (!error.IsWarning && ((Display & ErrorDisplay.Errors) == 0))
                    continue;
                ListViewItem item = new ListViewItem(new string[] { error.Line.ToString(), error.ErrorText.ToString() });
                Items.Add(item);
            }
        }
        #endregion
    }
}
