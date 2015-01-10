using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FarseerPhysics.Common;
namespace MaskGenerator
{
    public partial class VerticesDisplay : UserControl
    {
        #region Properties
        /// <summary>
        /// Liste de polygones.
        /// </summary>
        public List<Vertices> Vertices
        {
            get;
            set;

        }
        /// <summary>
        /// Zoom du contrôle.
        /// </summary>
        public float Zoom
        {
            get;
            set;
        }
        public System.Drawing.Bitmap Image
        {
            get;
            set;
        }
        #endregion

        #region Methods
        public VerticesDisplay()
        {
            InitializeComponent();
            this.Paint += delegate(object sender, PaintEventArgs e)
            {
                Draw();
            };
            this.MouseDown += new MouseEventHandler(OnMouseDown);
            this.MouseUp += new MouseEventHandler(OnMouseUp);
            this.MouseMove += new MouseEventHandler(OnMouseMove);
        }

        #region Deplacement
        bool isDown = false;
        Point last = Point.Empty;
        Point center = Point.Empty;
        void OnMouseDown(object sender, MouseEventArgs e)
        {
            isDown = true;
            last = e.Location;
        }

        void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (isDown)
            {
                center.X += e.Location.X - last.X;
                center.Y += e.Location.Y - last.Y;
                last = e.Location;
                Draw();
            }
        }

        void OnMouseUp(object sender, MouseEventArgs e)
        {
            isDown = false;
        }
        #endregion
        /// <summary>
        /// Dessine le contrôle.
        /// </summary>
        public void Draw()
        {
            Graphics g = CreateGraphics();
            g.Clear(Color.Blue);
            Pen p, p2;
            int i = 0;
            if (Image != null)
            {
                g.DrawImage(Image, center);
            }
            if (Vertices != null)
            {
                //Point center = new Point(0, 0);//new Point((int)(g.ClipBounds.X + g.ClipBounds.Width / 2), (int)(g.ClipBounds.Y + g.ClipBounds.Height / 2));
                foreach (Vertices polygon in Vertices)
                {
                    //p = new Pen(System.Drawing.Color.FromArgb(i * 20 % 255, Math.Max(0, (255 - i * 20) % 255), 0), 3);
                    p = new Pen(System.Drawing.Color.FromArgb(255, 0, 0), 1);
                    p2 = new Pen(System.Drawing.Color.FromArgb(0, 255, 255), 1);
                    Microsoft.Xna.Framework.Vector2 old = polygon.First();
                    // Dessine lignes puis les points
                    foreach (Microsoft.Xna.Framework.Vector2 vect in polygon)
                    {
                        g.DrawLine(p2, new Point((int)(center.X + vect.X * Zoom), (int)(center.Y + vect.Y * Zoom)),
                                        new Point((int)(center.X + old.X * Zoom), (int)(center.Y + old.Y * Zoom)));
                        old = vect;
                    }
                    g.DrawLine(p2, new Point((int)(center.X + polygon.First().X * Zoom), (int)(center.Y + polygon.First().Y * Zoom)),
                        new Point((int)(center.X + old.X * Zoom), (int)(center.Y + old.Y * Zoom)));

                    foreach (Microsoft.Xna.Framework.Vector2 vect in polygon)
                    {
                        g.DrawEllipse(p, new Rectangle((int)(center.X + vect.X * Zoom) - 1, (int)(center.Y + vect.Y * Zoom) - 1, 2, 2));
                    }

                            
                    i++;
                }
            }
        }
        #endregion

    }
}
