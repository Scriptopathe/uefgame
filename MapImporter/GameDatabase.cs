using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UeFGame.GameComponents;
namespace MapImporter
{
    /// <summary>
    /// Base de données du jeu.
    /// Contient des assets qui sont chargés par listes, et qui doivent être mémorisés tout le long du jeu.
    /// Normalement, seul FileRessourceLoader utilise cette instance.
    /// </summary>
    public class GameDatabase
    {
        static List<Tileset> Tilesets;
        
        /// <summary>
        /// Returns the tileset whose id is given as argument.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Tileset GetTileset(int id)
        {
            // Anti-crash safety : 
            // If the Database is deleted, we create the first default element.
            if (id == 0 && Tilesets.Count == 0)
                Tilesets.Add(new Tileset());

            return Tilesets[id];

        }

        /// <summary>
        /// Loads the Database
        /// </summary>
        public static void Load()
        {
            Tilesets = FileRessourceProvider.LoadTilesets();
        }
    }
}
