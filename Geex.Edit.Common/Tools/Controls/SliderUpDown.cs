using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Geex.Edit.Common.Tools.Controls
{
    /// <summary>
    /// Control combining a Slider and a NumericUpDown.
    /// </summary>
    public partial class SliderUpDown : UserControl
    {
        /* ----------------------------------------------------------
         * Variables
         * --------------------------------------------------------*/
        #region Variables
        /// <summary>
        /// Used to avoid infinite loops.
        /// </summary>
        bool ignoreEvent = false;
        /// <summary>
        /// Minimum value.
        /// </summary>
        int m_min;
        /// <summary>
        /// Maximum value.
        /// </summary>
        int m_max;
        /// <summary>
        /// Number of graduations of the trackbar.
        /// </summary>
        int m_numberOfGraduations = 10;
        #endregion
        /* ----------------------------------------------------------
         * Properties
         * --------------------------------------------------------*/
        #region Properties
        /// <summary>
        /// Gets or sets the number of graduations of the trackbar.
        /// </summary>
        public int NumberOfGraduations
        {
            get { return m_numberOfGraduations; }
            set { m_numberOfGraduations = value; UpdateTickFrequency(); }
        }
        /// <summary>
        /// Minimum value.
        /// </summary>
        public int Min
        {
            get { return m_min; }
            set
            {
                m_min = value;
                m_trackbar.Minimum = value;
                m_updown.Minimum = value;
                UpdateTickFrequency();
            }
        }
        /// <summary>
        /// Maximum value.
        /// </summary>
        public int Max
        {
            get { return m_max; }

            set
            {
                m_max = value;
                m_trackbar.Maximum = value;
                m_updown.Maximum = value;
                UpdateTickFrequency();
            }
        }
        /// <summary>
        /// Gets or sets the value of the control.
        /// </summary>
        public int Value
        {
            get { return (int)m_updown.Value; }
            set
            {
                m_updown.Value = value;
            }
        }
        /// <summary>
        /// Fired when the value changed.
        /// </summary>
        public event EventHandler ValueChanged;
        #endregion
        /* ----------------------------------------------------------
         * Methods
         * --------------------------------------------------------*/
        #region Methods
        /// <summary>
        /// Creates a new instance of the Slider Up Down component.
        /// </summary>
        public SliderUpDown()
        {
            InitializeComponent();
            m_trackbar.ValueChanged += new EventHandler(OnTrackbarValueChanged);
            m_updown.ValueChanged += new EventHandler(OnUpdownValueChanged);
        }
        /// <summary>
        /// Updates the tick frequency of the trackbar.
        /// </summary>
        void UpdateTickFrequency()
        {
            int elapsed = Max - Min;
            m_trackbar.TickFrequency = elapsed / NumberOfGraduations;
        }
        /// <summary>
        /// Called the the trackbar value changed.
        /// </summary>
        void OnTrackbarValueChanged(object o, EventArgs a)
        {
            if (ignoreEvent)
                return;
            ignoreEvent = true;
            m_updown.Value = m_trackbar.Value;
            ignoreEvent = false;
            if (ValueChanged != null)
                ValueChanged(this, EventArgs.Empty);
        }
        /// <summary>
        /// Called when the updown value changed.
        /// </summary>
        void OnUpdownValueChanged(object o, EventArgs a)
        {
            if (ignoreEvent)
                return;
            ignoreEvent = true;
            m_trackbar.Value = (int)m_updown.Value;
            ignoreEvent = false;
            if (ValueChanged != null)
                ValueChanged(this, EventArgs.Empty);
        }
        #endregion
    }
}
