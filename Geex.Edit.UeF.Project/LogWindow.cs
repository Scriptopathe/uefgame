using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Geex.Edit.UeF.Project
{
    public partial class LogWindow : Form
    {
        public void Append(string text)
        {
            m_logTextbox.Text += text.Replace("\n", "\r\n");
        }
        public LogWindow()
        {
            InitializeComponent();
        }
    }
}
