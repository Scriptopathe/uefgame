using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Matrix = System.Drawing.Imaging.ColorMatrix;
using ImageAttributes = System.Drawing.Imaging.ImageAttributes;
namespace Geex.Edit.Common.Tools.Controls
{
    /// <summary>
    /// An advanced Picture box object.
    /// It features :
    ///     - Zoom buttons
    ///     - Saturation (0-255)
    ///     - Drawing of a selection rectangle.
    /// </summary>
    public class AdvancedPictureBox : ScrollableControl
    {
        #region Variables
        Image m_image;
        ImageAttributes m_imageAttributes;
        int m_zoom = 1;
        float[][] m_saturationMatrix;
        float m_saturation = 1.0f;
        Pen _rectanglePen;
        Rectangle m_selectionRect;
        #endregion

        #region Properties
        /// <summary>
        /// Rectangle (whose dimensions are in pixels) that can be drawn on the image to show
        /// the user selection within the picture box, if the image shown by the PictureBox
        /// represents more than one entity (frame).
        /// </summary>
        public Rectangle SelectionRect
        {
            get { return m_selectionRect; }
            set { m_selectionRect = value; Invalidate(); }
        }
        /// <summary>
        /// Gets or sets the image of this AdvancedPictureBox.
        /// </summary>
        public Image Image
        {
            get { return m_image; }
            set
            {
                m_image = value;
                if (m_image != null)
                {
                    this.AutoScrollMinSize = new Size(m_image.Width / m_zoom,
                        m_image.Height / m_zoom);
                }
                System.Drawing.Graphics g = this.CreateGraphics();
                PaintControl(g);
            }
        }
        /// <summary>
        /// Gets or sets the zoom of the control.
        /// </summary>
        public int Zoom
        {
            get { return m_zoom; }
            set
            {
                m_zoom = value;
                if (m_image != null)
                    this.AutoScrollMinSize = new Size(m_image.Width / m_zoom,
                        m_image.Height / m_zoom);
                PaintControl();
            }
        }
        /// <summary>
        /// Gets or sets the saturation of the image
        /// Range : 0 (gray) -> 255 (full colors).
        /// </summary>
        public int Saturation
        {
            get { return (int)m_saturation*255; }
            set 
            {
                if (value < 0)
                    value = 0;
                else if (value > 255)
                    value = 255;
                m_saturation = value/255.0f;
                m_saturationMatrix = SaturateMatrix(m_saturation);
                PaintControl();
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Creates a new instance of Advanced Picture box.
        /// </summary>
        public AdvancedPictureBox()
            : base()
        {
            this.AutoScroll = true;
            this.Paint += new PaintEventHandler(OnPaint);
            m_imageAttributes = new ImageAttributes();
            this.DoubleBuffered = true;

            // Misc
            _rectanglePen = new Pen(Color.Black, 2);
        }
        /// <summary>
        /// Paints the control.
        /// </summary>
        void OnPaint(object sender, PaintEventArgs e)
        {
            PaintControl(e.Graphics);
        }
        /// <summary>
        /// Paints the control on a double buffered graphics.
        /// </summary>
        void PaintControl()
        {
            System.Drawing.Graphics g = CreateGraphics();
            BufferedGraphics bg = BufferedGraphicsManager.Current.Allocate(g, this.ClientRectangle);
            PaintControl(bg.Graphics);
            PaintSelectionRect(bg.Graphics);
            bg.Render();
            bg.Dispose();
        }
        /// <summary>
        /// Paints the selection rectangle.
        /// </summary>
        /// <param name="g"></param>
        void PaintSelectionRect(System.Drawing.Graphics g)
        {
            if(SelectionRect != null)
                g.DrawRectangle(_rectanglePen, SelectionRect);
        }
        /// <summary>
        /// Paints the control, with as only argument a graphics object.
        /// </summary>
        /// <param name="g"></param>
        void PaintControl(System.Drawing.Graphics g)
        {
            g.Clear(Color.LightGray);

            if (Image != null)
            {
                Rectangle dstRect = new Rectangle(this.AutoScrollPosition.X,
                                    this.AutoScrollPosition.Y,
                                    m_image.Width / m_zoom,
                                    m_image.Height / m_zoom);
                // If no hue change is needed
                if (m_saturationMatrix == null)
                {
                    g.DrawImage(Image, dstRect, 0, 0, Image.Width, Image.Height, GraphicsUnit.Pixel);
                }
                else // use a matrix to change the hue.
                {
                    // Create a matrix to change hue.
                    // float[][] mat = RotateHueMatrix(50);
                    Matrix matrix = new Matrix(m_saturationMatrix);
                    m_imageAttributes.SetColorMatrix(matrix);
                    try
                    {
                        g.DrawImage(Image, dstRect, 0, 0, Image.Width, Image.Height, GraphicsUnit.Pixel, m_imageAttributes);
                    }
                    catch (OutOfMemoryException)
                    {
                        // FIXME
                        g.DrawImage(Image, dstRect, 0, 0, Image.Width, Image.Height, GraphicsUnit.Pixel);
                    }
                }
            }
            else
                g.DrawRectangle(Pens.Black,
                    new Rectangle(this.ClientRectangle.X, this.ClientRectangle.Y, this.ClientRectangle.Width - 1, this.ClientRectangle.Height - 1));
        }

        #endregion

        #region Matrix Utils
        /// <summary>
        /// Multitplies two matrixes and returns the final result.
        /// </summary>
        void MatrixMult(float[][] a, float[][] b, ref float[][] result)
        {
            int x, y;
            for (y = 0; y < 4; y++)
            {
                if(result[y] == null)
                    result[y] = new float[4];
                for (x = 0; x < 4; x++)
                {
                    result[y][x] = b[y][0] * a[0][x]
                              + b[y][1] * a[1][x]
                              + b[y][2] * a[2][x]
                              + b[y][3] * a[3][x];

                }
            }
          
        }
        /// <summary>
        /// Returns a rotated matrix around the x axis.
        /// </summary>
        /// <param name="rs"></param>
        /// <param name="rc"></param>
        /// <returns></returns>
        void XRotateMatrix(ref float[][] m, float rs, float rc)
        {
            float[][] rotateMatrix = new float[4][];
            rotateMatrix[0] = new float[4];
            rotateMatrix[0][0] = 1.0f;
            rotateMatrix[0][1] = 0.0f;
            rotateMatrix[0][2] = 0.0f;
            rotateMatrix[0][3] = 0.0f;
            rotateMatrix[1] = new float[4];
            rotateMatrix[1][0] = 0.0f;
            rotateMatrix[1][1] = rc;
            rotateMatrix[1][2] = rs;
            rotateMatrix[1][3] = 0.0f;
            rotateMatrix[2] = new float[4];
            rotateMatrix[2][0] = 0.0f;
            rotateMatrix[2][1] = -rs;
            rotateMatrix[2][2] = rc;
            rotateMatrix[2][3] = 0.0f;
            rotateMatrix[3] = new float[4];
            rotateMatrix[3][0] = 0.0f;
            rotateMatrix[3][1] = 0.0f;
            rotateMatrix[3][2] = 0.0f;
            rotateMatrix[3][3] = 1.0f;
            MatrixMult(rotateMatrix, m, ref m);
        }
        /// <summary>
        /// Returns a rotated matrix around the y axis.
        /// </summary>
        /// <param name="rs"></param>
        /// <param name="rc"></param>
        /// <returns></returns>
        void YRotateMatrix(ref float[][] m, float rs, float rc)
        {
            float[][] rotateMatrix = new float[4][];
            rotateMatrix[0] = new float[4];
            rotateMatrix[1] = new float[4];
            rotateMatrix[2] = new float[4];
            rotateMatrix[3] = new float[4];

            // --
            rotateMatrix[0][0] = rc;
            rotateMatrix[0][1] = 0.0f;
            rotateMatrix[0][2] = -rs;
            rotateMatrix[0][3] = 0.0f;

            rotateMatrix[1][0] = 0.0f;
            rotateMatrix[1][1] = 1.0f;
            rotateMatrix[1][2] = 0.0f;
            rotateMatrix[1][3] = 0.0f;

            rotateMatrix[2][0] = rs;
            rotateMatrix[2][1] = 0.0f;
            rotateMatrix[2][2] = rc;
            rotateMatrix[2][3] = 0.0f;

            rotateMatrix[3][0] = 0.0f;
            rotateMatrix[3][1] = 0.0f;
            rotateMatrix[3][2] = 0.0f;
            rotateMatrix[3][3] = 1.0f;
            // --
            MatrixMult(rotateMatrix, m, ref m);
        }
        /// <summary>
        /// Returns a rotated matrix around the z axis.
        /// </summary>
        /// <param name="rs"></param>
        /// <param name="rc"></param>
        /// <returns></returns>
        void ZRotateMatrix(ref float[][] m, float rs, float rc)
        {
            float[][] rotateMatrix = new float[4][];
            rotateMatrix[0] = new float[4];
            rotateMatrix[1] = new float[4];
            rotateMatrix[2] = new float[4];
            rotateMatrix[3] = new float[4];

            // --
            rotateMatrix[0][0] = rc;
            rotateMatrix[0][1] = rs;
            rotateMatrix[0][2] = 0.0f;
            rotateMatrix[0][3] = 0.0f;

            rotateMatrix[1][0] = -rs;
            rotateMatrix[1][1] = rc;
            rotateMatrix[1][2] = 0.0f;
            rotateMatrix[1][3] = 0.0f;

            rotateMatrix[2][0] = 0.0f;
            rotateMatrix[2][1] = 0.0f;
            rotateMatrix[2][2] = 1.0f;
            rotateMatrix[2][3] = 0.0f;

            rotateMatrix[3][0] = 0.0f;
            rotateMatrix[3][1] = 0.0f;
            rotateMatrix[3][2] = 0.0f;
            rotateMatrix[3][3] = 1.0f;
            // --
            MatrixMult(rotateMatrix, m, ref m);

        }
        /// <summary>
        /// Returns a rotate hue matrix
        /// </summary>
        /// <param name="rot"></param>
        /// <returns></returns>
        float[][] RotateHueMatrix(float rot)
        {
            float[][] matrix = new float[5][];
            for (int i = 0; i < 5; i++)
            {
                matrix[i] = new float[5];
                matrix[i][i] = 1.0f;
            }
            float mag;
            float xrs, xrc;
            float yrs, yrc;
            float zrs, zrc;

            // rotate the grey vector
            mag = (float)Math.Sqrt(2);
            xrs = 1.0f / mag;
            xrc = 1.0f / mag;
            XRotateMatrix(ref matrix, xrs, xrc);

            mag = (float)Math.Sqrt(3);
            yrs = -1.0f / mag;
            yrc = (float)Math.Sqrt(2.0) / mag;
            YRotateMatrix(ref matrix, yrs, yrc);

            // Rotates the hue.
            zrs = (float)Math.Sin(rot * Math.PI / 180.0f);
            zrc = (float)Math.Cos(rot * Math.PI / 180.0f);
            ZRotateMatrix(ref matrix, zrs, zrc);

            // Rotates the grey vector back
            XRotateMatrix(ref matrix, -xrs, xrc);
            YRotateMatrix(ref matrix, -yrs, yrc);

            return matrix;
        }

        const float RLUM = 0.3086f;
        const float GLUM = 0.6094f;
        const float BLUM = 0.0820f;
        /// <summary>
        /// Returns a saturate matrix
        /// </summary>
        /// <param name="sat"></param>
        /// <returns></returns>
        float[][] SaturateMatrix(float sat)
        {
            float[][] matrix = new float[5][];
            for (int k = 0; k < 5; k++)
            {
                matrix[k] = new float[5];
                matrix[k][k] = 1.0f;
            }
            float a, b, c, d, e, f, g, h, i;
            float rwgt, gwgt, bwgt;

            rwgt = RLUM;
            gwgt = GLUM;
            bwgt = BLUM;

            a = (1.0f - sat) * rwgt + sat;
            b = (1.0f - sat) * rwgt;
            c = (1.0f - sat) * rwgt;
            d = (1.0f - sat) * gwgt;
            e = (1.0f - sat) * gwgt + sat;
            f = (1.0f - sat) * gwgt;
            g = (1.0f - sat) * bwgt;
            h = (1.0f - sat) * bwgt;
            i = (1.0f - sat) * bwgt + sat;

            matrix[0][0] = a;
            matrix[0][1] = b;
            matrix[0][2] = c;
            matrix[0][3] = 0.0f;

            matrix[1][0] = d;
            matrix[1][1] = e;
            matrix[1][2] = f;
            matrix[1][3] = 0.0f;

            matrix[2][0] = g;
            matrix[2][1] = h;
            matrix[2][2] = i;
            matrix[2][3] = 0.0f;

            matrix[3][0] = 0.0f;
            matrix[3][1] = 0.0f;
            matrix[3][2] = 0.0f;
            matrix[3][3] = 1.0f;

            return matrix;
        }
        #endregion
    }


}
