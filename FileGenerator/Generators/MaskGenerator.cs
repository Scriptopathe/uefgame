using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UeFGame.GameComponents;
using UeFGame.Ressources;
namespace FileGenerator.Generators
{
    public class MaskGenerator
    {
        public TilesetMask Mask;
        public int TilesetId;

        public MaskGenerator()
        {
            Mask = new TilesetMask();
        }

        public Tileset GetAssociatedTileset()
        {
            return FileRessourceProvider.LoadTileset(TilesetId);
        }
        
        public void Generate()
        {
            FileRessourceProvider.SaveTilesetMask(TilesetId, Mask);
        }
    }
}
