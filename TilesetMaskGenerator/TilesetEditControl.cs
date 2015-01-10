using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
namespace TilesetMaskGenerator
{
    /// <summary>
    /// Permet l'édition du masque d'un tileset importé.
    /// </summary>
    public class TilesetEditControl : UserControl
    {
        #region Variable
        /// <summary>
        /// Bitmap servant de tampon pour l'affichage.
        /// </summary>
        private Bitmap m_bufferBitmap;
        /// <summary>
        /// Bitmap source.
        /// </summary>
        private Bitmap m_srcBitmap;
        /// <summary>
        /// Tiles actifs => ne seront pas passables.
        /// </summary>
        public bool[,] m_activeTiles;
        #endregion

        #region Properties
        /// <summary>
        /// Obtient ou définit la valeur du Bitmap en cours d'édition.
        /// </summary>
        public Bitmap EditedBitmap
        {
            get { return m_srcBitmap; }
            set { m_srcBitmap = value; LoadBitmap(m_srcBitmap); }
        }
        /// <summary>
        /// Obtient le masque généré.
        /// </summary>
        public UeFGame.GameComponents.TilesetMask Mask
        {
            get { return CreateMask();  }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Crée une instance de TilesetEditControl.
        /// </summary>
        public TilesetEditControl()
        {
            this.AutoScroll = true;
            this.Paint += new PaintEventHandler(PaintControl);
            this.MouseClick += new MouseEventHandler(OnToogleState);
            this.MouseMove += new MouseEventHandler(OnToogleState);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnToogleState(object sender, MouseEventArgs e)
        {

            if (e.Button == System.Windows.Forms.MouseButtons.Left || e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                // Left => remove
                // Right => add
                bool val = e.Button == System.Windows.Forms.MouseButtons.Left ? false : true;
                int tileX = (-AutoScrollPosition.X + e.X) / 32;
                int tileY = (-AutoScrollPosition.Y + e.Y) / 32;

                // Verif
                if (tileX < 0 || tileY < 0 || tileX >= m_activeTiles.GetLength(0) || tileY >= m_activeTiles.GetLength(1))
                    return;

                // On fait la modif si besoin.
                if (m_activeTiles[tileX, tileY] != val)
                {
                    m_activeTiles[tileX, tileY] = val;
                    RedrawTile(tileX, tileY);
                }
            }

            
        }
        /// <summary>
        /// Redessine le tile donné en argument.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        void RedrawTile(int x, int y)
        {
            bool drawBack = m_activeTiles[x, y];
            Rectangle rect = new Rectangle(x * 32, y * 32, 32, 32);
            Rectangle scrolled = new Rectangle(x * 32, y * 32, 32, 32);
            Color col = drawBack ? Color.PaleVioletRed : Color.White;
            Graphics g = Graphics.FromImage(m_bufferBitmap);
            g.FillRectangle(new SolidBrush(col), scrolled);
            g.DrawImage(m_srcBitmap, scrolled, rect, GraphicsUnit.Pixel);

            // Si non utilisé, on blanchit.
            if (!drawBack)
            {
                g.FillRectangle(new SolidBrush(Color.FromArgb(125, Color.White)), rect);
            }
            g.Flush();
            g.Dispose();
            Invalidate();
        }
        /// <summary>
        /// Dessine le contrôle.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PaintControl(object sender, PaintEventArgs e)
        {
            Graphics g = CreateGraphics();
            if (m_bufferBitmap != null)
                g.DrawImage(m_bufferBitmap, new Point(AutoScrollPosition.X, AutoScrollPosition.Y));
        }
        /// <summary>
        /// Charge le bitmap source.
        /// </summary>
        /// <param name="src"></param>
        void LoadBitmap(Bitmap src)
        {
            if (src != null)
            {
                m_bufferBitmap = new Bitmap(src.Width, src.Height);
                /*Graphics g = Graphics.FromImage(m_bufferBitmap);
                g.Clear(Color.PaleVioletRed);
                g.DrawImage(m_srcBitmap, new Point(0, 0));
                g.Flush();
                g.Dispose();*/

                ClientSize = new Size(m_bufferBitmap.Width, m_bufferBitmap.Height);
                AutoScrollMinSize = new Size(ClientSize.Width+1, ClientSize.Height);
                AdjustFormScrollbars(true);

                // Rend tous les tiles actifs.
                m_activeTiles = new bool[m_bufferBitmap.Width / 32, m_bufferBitmap.Height / 32];
                for(int x = 0; x < m_activeTiles.GetLength(0); x++)
                {
                    for(int y = 0; y < m_activeTiles.GetLength(1); y++)
                    {
                        m_activeTiles[x, y] = true;
                        RedrawTile(x, y);
                    }
                }
            }
        }

        /// <summary>
        /// Génère un masque à partir du modèle.
        /// </summary>
        /// <returns></returns>
        UeFGame.GameComponents.TilesetMask CreateMask()
        {
            var mask = new UeFGame.GameComponents.TilesetMask();
            mask.Raw = new bool[m_activeTiles.GetLength(0) * 32, m_activeTiles.GetLength(1) * 32];
            for (int x = 0; x < m_activeTiles.GetLength(0); x++)
            {
                for (int y = 0; y < m_activeTiles.GetLength(1); y++)
                {
                    // On dessine sur le masque seulement les active tiles.
                    if (m_activeTiles[x, y])
                    {
                        for (int sx = x * 32; sx < (x + 1) * 32; sx++)
                        {
                            for (int sy = y * 32; sy < (y + 1) * 32; sy++)
                            {
                                Color px = m_srcBitmap.GetPixel(sx, sy);
                                mask.Raw[sx, sy] = (px.A != 0);
                            }
                        }
                    }
                }
            }

            return mask;
        }
        #endregion
    }
}
