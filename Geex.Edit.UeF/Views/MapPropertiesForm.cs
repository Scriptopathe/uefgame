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
    public partial class MapPropertiesForm : Form
    {
        #region Variables

        #endregion


        #region Properties
        /// <summary>
        /// This dialog's map.
        /// </summary>
        public MapInitializingData Map
        {
            get;
            set;
        }
        /// <summary>
        /// This dialog's map info.
        /// </summary>
        public Common.Project.MapInfo MapInfo
        {
            get;
            set;
        }
        public int NewWidth
        {
            get { return (int)m_widthUpdown.Value; }
        }
        public int NewHeight
        {
            get { return (int)m_heightUpdown.Value; }
        }
        public int NewTilesetId
        {
            get { return (int)m_tilesetCombo.SelectedIndex; }
        }
        public string NewName
        {
            get { return m_mapNameTextbox.Text; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Constructeur.
        /// </summary>
        public MapPropertiesForm()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Initialise la form.
        /// </summary>
        public void Initialize()
        {
            m_heightUpdown.Value = (int)Map.SizeInTiles.Y;
            m_widthUpdown.Value = (int)Map.SizeInTiles.X;
            m_mapNameTextbox.Text = MapInfo.Name;
            int i = 0;
            foreach (Tileset tileset in UeFGlobals.Project.Database.Tilesets)
            {
                m_tilesetCombo.Items.Add(i.ToString().PadLeft(4, '0') + ": " + tileset.Name);
                i++;
            }
            m_tilesetCombo.SelectedIndex = Map.TilesetId;
        }
        #endregion
    }
}
