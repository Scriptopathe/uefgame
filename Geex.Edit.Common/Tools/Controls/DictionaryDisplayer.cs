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
    public partial class DictionaryDisplayer : UserControl
    {
        /// <summary>
        /// Delegate used to get a custom string from an object.
        /// </summary>
        /// <returns></returns>
        public delegate string ToStringDelegate(object o);
        /* ----------------------------------------------------------
         * Variables.
         * --------------------------------------------------------*/
        #region Variables
        /// <summary>
        /// The dictionary whose keys and values will be displayed.
        /// </summary>
        Dictionary<object, object> m_dictionary;
        /// <summary>
        /// The font used to draw the keys.
        /// </summary>
        Font m_keyfont;
        /// <summary>
        /// The font used to draw the values.
        /// </summary>
        Font m_valuefont;
        /// <summary>
        /// The color used to draw the keys.
        /// </summary>
        Color m_keysColor;
        /// <summary>
        /// The color used to draw the values.
        /// </summary>
        Color m_valuesColor;
        /// <summary>
        /// Width of the keys columns.
        /// </summary>
        int m_keysColumnsWidth;
        /// <summary>
        /// Width of the values column.
        /// </summary>
        int m_valuesColumnsWidth;
        /// <summary>
        /// Height of the lines.
        /// </summary>
        int m_linesHeight;
        #endregion
        /* ----------------------------------------------------------
         * Properties.
         * --------------------------------------------------------*/
        #region Properties
        /// <summary>
        /// The color used to draw the keys.
        /// </summary>
        public Color KeysColor
        {
            get { return m_keysColor; }
            set { m_keysColor = value; Invalidate(); }
        }
        /// <summary>
        /// The color used to draw the values.
        /// </summary>
        public Color ValuesColor
        {
            get { return m_valuesColor; }
            set { m_valuesColor = value; Invalidate(); }
        }
        /// <summary>
        /// The dictionary whose keys and values will be displayed.
        /// </summary>
        public Dictionary<object, object> Dictionary
        {
            get { return m_dictionary; }
            set { m_dictionary = value; Invalidate(); }
        }
        /// <summary>
        /// The font used to draw the keys.
        /// </summary>
        public Font Keyfont
        {
            get { return m_keyfont; }
            set { m_keyfont = value; Invalidate(); }
        }
        /// <summary>
        /// The font used to draw the values.
        /// </summary>
        public Font Valuefont
        {
            get { return m_valuefont; }
            set { m_valuefont = value; Invalidate(); }
        }
        /// <summary>
        /// Width of the key columns.
        /// </summary>
        public int KeysColumnsWidth
        {
            get { return m_keysColumnsWidth; }
            set { m_keysColumnsWidth = value; Invalidate(); }
        }
        /// <summary>
        /// Width of the values column.
        /// </summary>
        public int ValuesColumnsWidth
        {
            get { return m_valuesColumnsWidth; }
            set { m_valuesColumnsWidth = value; Invalidate(); }
        }
        /// <summary>
        /// Height of the lines.
        /// </summary>
        public int LinesHeight
        {
            get { return m_linesHeight; }
            set { m_linesHeight = value; Invalidate(); }
        }
        /// <summary>
        /// Gets or sets the delegate which returns a string from
        /// a key.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ToStringDelegate KeyToString
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the delegate which returns a string from
        /// a value.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ToStringDelegate ValueToString
        {
            get;
            set;
        }
        #endregion
        /* ----------------------------------------------------------
         * Methods.
         * --------------------------------------------------------*/
        #region Methods
        /// <summary>
        /// Creates a new instance of the DictionaryDisplayer class.
        /// </summary>
        public DictionaryDisplayer()
        {
            InitializeComponent();
            // Default values
            DoubleBuffered = true;
            KeysColor = Color.Black;
            ValuesColor = Color.Red;
            Keyfont = new Font(
                new FontFamily(System.Drawing.Text.GenericFontFamilies.Monospace),
                8.0f);
            Valuefont = Keyfont;
            KeysColumnsWidth = 20;
            ValuesColumnsWidth = 60;
            LinesHeight = 25;
        }
        /// <summary>
        /// Paints the control.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Draw(e.Graphics);
        }
        /// <summary>
        /// Draws the component.
        /// </summary>
        public void Draw(System.Drawing.Graphics g)
        {
            g.Clear(Color.White);
            if (DesignMode && m_dictionary == null)
            {
                m_dictionary = new Dictionary<object, object>();
                for (int i = 0; i < 100; i++)
                {
                    m_dictionary.Add(i, (int)Math.Exp(i));
                }
            }
            if (m_dictionary != null)
            {
                // Sets up the autoscroll.
                int lines = ClientSize.Height / m_linesHeight;
                int totalWidth = (m_dictionary.Count / lines) * (ValuesColumnsWidth + KeysColumnsWidth);
                this.AutoScrollMinSize = new Size(totalWidth, this.ClientSize.Height);

                int column = 0;
                int line = 0;
                Brush keyBrush = new SolidBrush(KeysColor);
                Brush valueBrush = new SolidBrush(ValuesColor);
                Pen separator = new Pen(Color.Gray);
                separator.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                foreach (KeyValuePair<object, object> kvp in m_dictionary)
                {
                    string key;
                    string value;
                    if (KeyToString == null)
                        key = kvp.Key.ToString();
                    else
                        key = KeyToString(kvp.Key);
                    
                    if (ValueToString == null)
                        value = ":" + kvp.Value.ToString();
                    else
                        value = ":" + ValueToString(kvp.Value);

                    // Gets the position where to draw the strings.
                    int x = AutoScrollPosition.X + column * (KeysColumnsWidth + ValuesColumnsWidth);
                    int y = AutoScrollPosition.Y + line * LinesHeight;

                    // Draws the key
                    g.DrawString(key, Keyfont, keyBrush, x, y);
                    // Draws the value aligned to the right
                    int textW = (int)g.MeasureString(value, Valuefont).Width;
                    if (textW > ValuesColumnsWidth)
                    {
                        g.DrawString(value, Valuefont, valueBrush,
                            new RectangleF(x + KeysColumnsWidth, y, ValuesColumnsWidth, LinesHeight));
                    }
                    else
                    {
                        g.DrawString(value, Valuefont, valueBrush,
                            x + KeysColumnsWidth + ValuesColumnsWidth - textW, y);
                    }
                    // Draws a separator line.
                    g.DrawLine(separator, new Point(x + KeysColumnsWidth + ValuesColumnsWidth, 0),
                        new Point(x + KeysColumnsWidth + ValuesColumnsWidth, ClientSize.Height));
                    // Changes the position.
                    line++;
                    if (line == lines)
                    {
                        line = 0;
                        column++;
                    }
                }
            }

            g.Flush();
        }
        #endregion
    }
}
