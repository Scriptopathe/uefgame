using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UeFGame.Tools;
namespace MaskGenerator
{
    public partial class MainForm : Form
    {
        #region Variables
        List<string> m_filenames = new List<string>();
        string m_contentDirectory = "C:\\Users\\Scriptopathe\\Documents\\Josue\\[Projets]\\Projets de jeu\\Usine en Folie\\UsineEnFolieContent\\";
        #endregion
        /// <summary>
        /// Crée la fenêtre principale.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            InitializeEvents();
        }
        /// <summary>
        /// Initialise les évènements.
        /// </summary>
        void InitializeEvents()
        {
            m_srcFilesButton.Click += new EventHandler(OnBrowseButtonClicked);
            m_generateButton.Click += new EventHandler(OnGenerateButtonClicked);
        }
        /// <summary>
        /// Appelé quand le bouton "Générer" est cliqué.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnGenerateButtonClicked(object sender, EventArgs e)
        {
            string outputFolder = "DesignTimeAssets\\Data\\TilesetsMasks\\";

            bool sampleGenerated = false;
            foreach (string filename in m_filenames)
            {
                System.Drawing.Bitmap bmp = new Bitmap(filename);
                UeFGame.GameComponents.TilesetMask mask = PolygonGenerator.GenerateMaskFromTexture(bmp);
                Serializer.Serialize<UeFGame.GameComponents.TilesetMask>(mask, outputFolder + System.IO.Path.GetFileNameWithoutExtension(filename) + ".xml");
                if (!sampleGenerated)
                {
                    sampleGenerated = true;
                    List<FarseerPhysics.Common.Vertices> vertices = PolygonGenerator.ConvertToSimUnits(PolygonGenerator.GenerateFromMask(mask.Raw));
                    m_verticesDisplay.Vertices = vertices;
                    m_verticesDisplay.Zoom = 100f;
                    

                    bmp = new Bitmap(bmp.Width, bmp.Height);
                    for (int x = 0; x < bmp.Width; x++)
                    {
                        for (int y = 0; y < bmp.Height; y++)
                        {
                            bmp.SetPixel(x, y, mask.Raw[x, y] ? Color.Black : Color.White);
                        }
                    }
                    m_verticesDisplay.Image = bmp;
                    m_verticesDisplay.Draw();

                }
            }
        }
        /// <summary>
        /// Appelé quand le bouton "parcourir" est cliqué.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnBrowseButtonClicked(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = true;
            dlg.InitialDirectory = m_contentDirectory + "RunTimeAssets\\Graphics\\Tilesets";
            m_filenames.Clear();
            m_srcFile.Text = "";
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                foreach (string filename in dlg.FileNames)
                {
                    m_filenames.Add(filename);
                    m_srcFile.Text += filename + "; ";
                }
            }
        }
    }
}
