using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using UeFGame.GameComponents;
namespace Geex.Edit.UeF.Views
{
    /// <summary>
    /// Control used to view and edit a Tileset mask.
    /// </summary>
    public class TilesetMaskView : UserControl
    {
        #region Properties
        /// <summary>
        /// Gets or sets the selected tileset.
        /// </summary>
        public Tileset SelectedTileset
        {
            get;
            set;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Creates a new TilesetMaskView control.
        /// </summary>
        public TilesetMaskView()
            : base()
        {
            this.ClientSize = new System.Drawing.Size(Tileset.MaxTilesetSize, Tileset.MaxTilesetSize);
            this.AutoScroll = true;
        }
        /// <summary>
        /// Paints the control.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            //Bitmap tex = new Bitmap(UeFGame.Cache.QuickRessource.TilesetTexturesDir + SelectedTileset.TextureName + ".png");
            base.OnPaint(e);
        }
        #endregion
    }
}
