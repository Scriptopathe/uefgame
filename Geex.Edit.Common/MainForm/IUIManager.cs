using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace Geex.Edit.Common.MainForm
{
    public interface IUIManager : IDisposable
    {
        /// <summary>
        /// Initializes the events that depends on the other components
        /// </summary>
        void InitEvents();
        /// <summary>
        /// Initializes the controls in the given parent control.
        /// </summary>
        void InitControls(Control parent);
    }
}
