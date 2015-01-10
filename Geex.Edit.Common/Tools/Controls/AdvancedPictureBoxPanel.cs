using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
namespace Geex.Edit.Common.Tools.Controls
{
    /// <summary>
    /// This is the combination of an advanced picture box and some controls.
    /// </summary>
    public class AdvancedPictureBoxPanel : UserControl
    {

        public event EventHandler PictureBoxClicked;
        /* ----------------------------------------------------------------------------
         * Variables
         * --------------------------------------------------------------------------*/
        #region Variables
        private AdvancedPictureBox m_pictureBox;
        private Panel m_picturePanel;
        private FlowLayoutPanel m_pan;
        private TrackBar m_saturationTrack;
        private GroupBox m_saturationGroupbox;
        #endregion
        /* ----------------------------------------------------------------------------
         * Properties
         * --------------------------------------------------------------------------*/
        #region Properties
        /// <summary>
        /// Image of the picture box.
        /// </summary>
        public Image Image
        {
            get { return m_pictureBox.Image; }
            set { m_pictureBox.Image = value; }
        }
        /// <summary>
        /// Gets or sets the visibility of the hue change bar.
        /// </summary>
        public bool ShowSaturation
        {
            get { return m_saturationGroupbox.Visible; }
            set { m_saturationGroupbox.Visible = value; }
        }
        /// <summary>
        /// Gets or sets the saturation of the image in the picture box.
        /// See AdvancedPictureBox.Saturation for further details.
        /// </summary>
        public int Saturation
        {
            get { return m_pictureBox.Saturation; }
            set { m_pictureBox.Saturation = value; m_saturationTrack.Value = value; }
        }
        /// <summary>
        /// Gets or sets the frame cut.
        /// The Frame cut is a Size whose width indicates the number of frames in the width of the
        /// given bitmap (=columns) as well as the number of frames in the height of the given bitmap (=lines).
        /// I.E. if a charset is containing 16 frames (4 on each row and 4 on each line), this function
        /// shall return a Size whose width and height are 4.
        /// Each of these frames is represented in other functions by an id, created like this :
        /// id = width*y + x
        /// </summary>
        public Size FrameCut
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the selected frame Id.
        /// </summary>
        public int SelectedFrameId
        {
            get;
            set;
        }
        #endregion
        /* ----------------------------------------------------------------------------
         * Methods
         * --------------------------------------------------------------------------*/
        #region Methods
        /// <summary>
        /// Creates a new instance of AdvancedPictureBoxPanel !
        /// </summary>
        public AdvancedPictureBoxPanel()
        {
            InitializeComponent();
            InitializeZoomButtons();
            InitializeLang();
            this.m_pictureBox.MouseClick += new MouseEventHandler(OnPictureBoxClicked);
            this.m_saturationTrack.ValueChanged += new EventHandler(OnSaturationChanged);
        }
        
        /// <summary>
        /// Called when the picture box is clicked. Raises the PictureBoxClicked event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnPictureBoxClicked(object sender, MouseEventArgs e)
        {
            if (m_pictureBox.Image == null)
                return;
            if(PictureBoxClicked != null)
                PictureBoxClicked(sender, e);
            // Asks the advanced picture box to draw a selection rectangle uppon the image.
            int frameW, frameH, frameX, frameY, id;
            frameW = m_pictureBox.Image.Width / FrameCut.Width;
            frameH = m_pictureBox.Image.Height / FrameCut.Height;
            frameX = e.X / frameW;
            frameY = e.Y / frameH;
            id = frameY * FrameCut.Width + frameY;
            m_pictureBox.SelectionRect = new Rectangle(frameX*frameW, frameY*frameH, frameW, frameH);
        }
        /// <summary>
        /// Initialize langage.
        /// </summary>
        void InitializeLang()
        {
            if (Lang.I == null)
                return;
            this.m_saturationGroupbox.Text = Lang.I["AdvancedPictureBoxPanel_SatGroupbox"];
        }
        /// <summary>
        /// Initializes the zoom buttons.
        /// </summary>
        void InitializeZoomButtons()
        {
            // Initializes the zoom buttons
            for (int i = 0; i < 4; i++)
            {
                Button btn = new Button();
                int zoomAmount = ((int)Math.Pow(2, i));
                btn.Text = "X" + zoomAmount.ToString();
                btn.Size = new Size(30, 30);
                btn.Click += delegate(object sender, EventArgs e)
                {
                    m_pictureBox.Zoom = zoomAmount;
                };
                // The buttons are added to the flow layout panel.
                m_pan.Controls.Add(btn);
            }
        }
        /// <summary>
        /// Changes the hue of the preview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnSaturationChanged(object sender, EventArgs e)
        {
            this.m_pictureBox.Saturation = m_saturationTrack.Value;
        }
        #endregion
        /* ------------------------------------------------------------------------
         * DESIGN
         * ----------------------------------------------------------------------*/
        #region DESIGN
        /// <summary>
        /// Initializes the components.
        /// </summary>
        private void InitializeComponent()
        {
            this.m_picturePanel = new System.Windows.Forms.Panel();
            this.m_pictureBox = new Geex.Edit.Common.Tools.Controls.AdvancedPictureBox();
            this.m_pan = new System.Windows.Forms.FlowLayoutPanel();
            this.m_saturationTrack = new System.Windows.Forms.TrackBar();
            this.m_saturationGroupbox = new System.Windows.Forms.GroupBox();
            this.m_picturePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_saturationTrack)).BeginInit();
            this.m_saturationGroupbox.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_picturePanel
            // 
            this.m_picturePanel.Controls.Add(this.m_pictureBox);
            this.m_picturePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_picturePanel.Location = new System.Drawing.Point(0, 40);
            this.m_picturePanel.Name = "m_picturePanel";
            this.m_picturePanel.Size = new System.Drawing.Size(453, 203);
            this.m_picturePanel.TabIndex = 0;
            // 
            // m_pictureBox
            // 
            this.m_pictureBox.AutoScroll = true;
            this.m_pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_pictureBox.Image = null;
            this.m_pictureBox.Location = new System.Drawing.Point(0, 0);
            this.m_pictureBox.Name = "m_pictureBox";
            this.m_pictureBox.Saturation = 255;
            this.m_pictureBox.Size = new System.Drawing.Size(453, 203);
            this.m_pictureBox.TabIndex = 0;
            this.m_pictureBox.Zoom = 1;
            // 
            // m_pan
            // 
            this.m_pan.Dock = System.Windows.Forms.DockStyle.Top;
            this.m_pan.Location = new System.Drawing.Point(0, 0);
            this.m_pan.MaximumSize = new System.Drawing.Size(-1, 40);
            this.m_pan.Name = "m_pan";
            this.m_pan.Size = new System.Drawing.Size(0, 40);
            this.m_pan.TabIndex = 1;
            // 
            // m_saturationTrack
            // 
            this.m_saturationTrack.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_saturationTrack.LargeChange = 20;
            this.m_saturationTrack.Location = new System.Drawing.Point(3, 16);
            this.m_saturationTrack.Maximum = 255;
            this.m_saturationTrack.MinimumSize = new System.Drawing.Size(300, 45);
            this.m_saturationTrack.Name = "m_saturationTrack";
            this.m_saturationTrack.Size = new System.Drawing.Size(447, 45);
            this.m_saturationTrack.SmallChange = 10;
            this.m_saturationTrack.TabIndex = 0;
            this.m_saturationTrack.TickFrequency = 20;
            this.m_saturationTrack.Value = 255;
            // 
            // m_saturationGroupbox
            // 
            this.m_saturationGroupbox.Controls.Add(this.m_saturationTrack);
            this.m_saturationGroupbox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.m_saturationGroupbox.Location = new System.Drawing.Point(0, 243);
            this.m_saturationGroupbox.Name = "m_saturationGroupbox";
            this.m_saturationGroupbox.Size = new System.Drawing.Size(453, 58);
            this.m_saturationGroupbox.TabIndex = 2;
            this.m_saturationGroupbox.TabStop = false;
            this.m_saturationGroupbox.Text = "Saturation";
            // 
            // AdvancedPictureBoxPanel
            // 
            this.Controls.Add(this.m_picturePanel);
            this.Controls.Add(this.m_pan);
            this.Controls.Add(this.m_saturationGroupbox);
            this.Name = "AdvancedPictureBoxPanel";
            this.Size = new System.Drawing.Size(453, 301);
            this.m_picturePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.m_saturationTrack)).EndInit();
            this.m_saturationGroupbox.ResumeLayout(false);
            this.m_saturationGroupbox.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion
    }
}