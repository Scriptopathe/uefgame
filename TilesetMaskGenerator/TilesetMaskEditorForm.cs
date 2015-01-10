using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
namespace TilesetMaskGenerator
{
    /// <summary>
    /// Fenêtre principale du programme.
    /// </summary>
    public partial class MaskGeneratorForm : Form
    {
        /// <summary>
        /// Générateur de masque.
        /// </summary>
        public MaskGeneratorForm()
        {
            InitializeComponent();
            this.m_loadButton.Click += new EventHandler(OnLoadButtonClicked);
            this.m_saveButton.Click += new EventHandler(OnSaveButtonClicked);
        }

        /// <summary>
        /// Sauvegarde le masque à l'emplacement sélectionné par l'utilisateur.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnSaveButtonClicked(object sender, EventArgs e)
        {
            if (m_tilesetEdit.EditedBitmap == null)
            {
                MessageBox.Show(this, "Aucun fichier à exporter !", "Erreur");
                return;
            }
            SaveFileDialog dlg = new SaveFileDialog();
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                UeFGame.GameComponents.TilesetMask mask = m_tilesetEdit.Mask;
                XmlSerializer ser = new XmlSerializer(typeof(UeFGame.GameComponents.TilesetMask));
                System.IO.Stream stream = System.IO.File.Open(dlg.FileName, System.IO.FileMode.Create);
                ser.Serialize(stream, mask);
                stream.Close();
                mask.ToMonochromaticBitmap().Save(dlg.FileName + "-preview.png", System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        /// <summary>
        /// Demande à l'utilisateur une image à charger.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnLoadButtonClicked(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    Bitmap bmp = new Bitmap(dlg.FileName);
                    m_tilesetEdit.EditedBitmap = bmp;
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Une erreur est survenue lors du chargement du tileset : " + ex.Message);
                }
            }
        }
        
    }
}
