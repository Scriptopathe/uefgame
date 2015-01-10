using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UeFGame.GameComponents;
namespace FileGenerator.Generators
{
    /// <summary>
    /// Classe permettant la génération d'un fichier tileset.
    /// </summary>
    public class TilesetGenerator
    {
        public List<Tileset> Tilesets;


        /// <summary>
        /// Crée une instance de FileGenerators.Generators.TilesetGenerator.
        /// </summary>
        public TilesetGenerator()
        {
            Tilesets = UeFGame.Ressources.FileRessourceProvider.LoadTilesets();
        }
        public void Generate()
        {
            Program.ContentWork.AddElement("RunTimeAssets\\Data\\Tilesets.ueftilesets", "UeFTilesetImporter", "UeFTilesetProcessor");
            UeFGame.Ressources.FileRessourceProvider.SaveTilesets(Tilesets);
        }
    }
}
