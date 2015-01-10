using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.TextEditor;
using System.IO;
using ICSharpCode.TextEditor.Util;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor.Actions;
namespace Geex.Edit.UeF.Controls
{
    public partial class TextEditor : ICSharpCode.TextEditor.TextEditorControl
    {
        public TextEditor() : base()
        {
            InitializeComponent();
            LoadSyntax();
        }

        public void LoadSyntax()
        {
            // Allez on charge tout ce bordel et c'est bon.
            string dir = Geex.Edit.Common.AppRessources.RessourceDir() + "\\texteditor\\";
            FileSyntaxModeProvider fsmProvider;
            if (Directory.Exists(dir))
            {
                fsmProvider = new FileSyntaxModeProvider(dir);
                HighlightingManager.Manager.AddSyntaxModeFileProvider(fsmProvider);
                SetHighlighting("C#");
            }
        }
    }
}
