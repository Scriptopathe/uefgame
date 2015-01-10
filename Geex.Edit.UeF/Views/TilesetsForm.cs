using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UeFGame.GameComponents;

namespace Geex.Edit.UeF.Views
{
    public partial class TilesetsForm : Form
    {
        #region Properties
        /// <summary>
        /// Copy of the Tilesets.
        /// </summary>
        public List<UeFGame.GameComponents.Tileset> Tilesets { get { return UeFGlobals.Project.Database.Tilesets; } }
        /// <summary>
        /// Reference to the selected Tileset.
        /// </summary>
        Tileset SelectedTileset
        {
            get { return Tilesets[m_tilesetsListbox.SelectedIndex]; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Tileset form.
        /// </summary>
        public TilesetsForm()
        {
            InitializeComponent();

        }
        /// <summary>
        /// Initialize the controls of the form.
        /// </summary>
        public void InitializeControls()
        {
            // Tileset Name
            string dir = UeFGlobals.Project.RunTimeTilesetTexturesDirectory;

            // Tileset list
            m_tilesetsListbox.Items.Clear();
            int id = 0;
            foreach(Tileset tileset in Tilesets)
            {
                m_tilesetsListbox.Items.Add(id.ToString().PadLeft(4, '0') + " : " + tileset.Name);
                id++;
            }
            m_tilesetsListbox.SelectedIndexChanged += new EventHandler(OnTilesetSelected);
            m_tilesetsListbox.SelectedIndex = 0;

            m_picturePicker.GP_Directory = UeFGlobals.Project.RunTimeTilesetTexturesDirectory.Replace(
                UeFGlobals.Project.ContentDirectory + "\\", "");

            if (Tilesets.Count != 0)
            {
                m_picturePicker.GP_Filename = Tilesets[0].TextureName + ".png";

            }
            m_nameTextbox.TextChanged += new EventHandler(OnNameChanged);
            m_picturePicker.GP_GraphicsParserType = typeof(GraphicsParser);
            m_picturePicker.GP_ValidityPredicate = Project.GameRessources.GetGraphicsFileValidity;
            m_picturePicker.GP_Options = new Views.GraphicsParserOptionsRpg(GraphicsParserOptionsRpg.Preset.NoOptions);
            m_picturePicker.SelectionChanged += new Common.Tools.Controls.PicturePicker.SelectionChangedEventHandler(OnTextureSelection);

            // Ajouter / Supprimer
            m_addButton.Click += new EventHandler(OnAddTileset);
            m_deleteButton.Click += new EventHandler(OnDeleteTileset);
            m_okButton.Click += new EventHandler(OnOk);
        }

        /// <summary>
        /// Rend effectif les changements dans le reste de l'éditeur.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnOk(object sender, EventArgs e)
        {
            UeFGlobals.MapView.RefreshMap();
            UeFGlobals.TilePicker.OnTilesetChanged();
        }
        /// <summary>
        /// Ajoute un tileset dans la BDD.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnAddTileset(object sender, EventArgs e)
        {
            Tileset t = new Tileset();
            t.Name = "Nouveau";
            t.TextureName = "default";
            Tilesets.Add(t);
            m_tilesetsListbox.Items.Add((Tilesets.Count-1).ToString().PadLeft(4, '0') + ": " + t.Name);
        }
        
        /// <summary>
        /// Supprime un tileset de la BDD.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnDeleteTileset(object sender, EventArgs e)
        {
            int s = m_tilesetsListbox.SelectedIndex;
            m_tilesetsListbox.Items.RemoveAt(s);
            Tilesets.RemoveAt(s);
        }

        bool _noraise;
        /// <summary>
        /// Se produit lorsque le nom du tileset charge.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnNameChanged(object sender, EventArgs e)
        {
            if (_noraise)
                return;
            _noraise = true;
            m_tilesetsListbox.Items[m_tilesetsListbox.SelectedIndex] = m_nameTextbox.Text;
            _noraise = false;
            Tilesets[m_tilesetsListbox.SelectedIndex].Name = m_nameTextbox.Text;
        }

        /// <summary>
        /// Se produit lorsqu'une nouvelle 
        /// </summary>
        /// <param name="newSelection"></param>
        void OnTextureSelection(Common.Tools.Controls.GraphicsParserOutput newSelection)
        {
            int id = m_tilesetsListbox.SelectedIndex;
            Tilesets[id].TextureName = newSelection.Filename.Replace(".png", "");
            m_picturePicker.Invalidate();
        }

        /// <summary>
        /// Called when a new tileset is selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnTilesetSelected(object sender, EventArgs e)
        {
            if (_noraise || m_tilesetsListbox.SelectedIndex == -1)
                return;
            _noraise = true;
            m_nameTextbox.Text = Tilesets[m_tilesetsListbox.SelectedIndex].Name;
            _noraise = false;
            m_picturePicker.GP_Filename = Tilesets[m_tilesetsListbox.SelectedIndex].TextureName + ".png";
            m_picturePicker.Invalidate();
        }
        #endregion
    }
}
