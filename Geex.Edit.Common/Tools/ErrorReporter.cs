using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace Geex.Edit.Common.Tools
{
    /// <summary>
    /// Error reporter used to display information about errors to the user.
    /// </summary>
    class ErrorReporter : Form
    {
        #region Variables
        StringBuilder m_message;
        TextBox m_textBox;
        bool hasErrors;
        #endregion
        /// <summary>
        /// Constructor.
        /// </summary>
        public ErrorReporter(Form parent)
        {
            m_message = new StringBuilder();
            hasErrors = false;
            // Settings
            this.Hide();
            this.Size = new System.Drawing.Size(640, 480);

            this.CenterToParent();
            this.Text = Lang.I["ErrorReporter_Caption"];
            // Layout
            Label label = new Label();
            label.Dock = DockStyle.Top;
            label.Text = Lang.I["ErrorReporter_Label"];

            m_textBox = new TextBox();
            m_textBox.Dock = DockStyle.Fill;

            this.Controls.Add(label);
            this.Controls.Add(m_textBox);
        }
        /// <summary>
        /// Adds a new error message in the queue.
        /// </summary>
        /// <param name="str"></param>
        public void Add(string str)
        {
            m_message.AppendLine(str);
            hasErrors = true;
        }
        /// <summary>
        /// Shows the error report.
        /// </summary>
        public void ShowErrors()
        {
            if (hasErrors)
            {
                m_textBox.AppendText(m_message.ToString());
                this.Show();
                this.CenterToParent();
            }
        }
    }
}
