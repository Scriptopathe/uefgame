using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UeFGame
{
    /// <summary>
    /// Permet le stockage de paramètres de configuration.
    /// </summary>
    public class Config
    {
        /// <summary>
        /// Id de la map où commence le jeu.
        /// </summary>
        public int StartMapId { get; set; }
        public System.Drawing.Size Resolution { get; set; }
        /// <summary>
        /// Position de départ du joueur en unités de simulation.
        /// </summary>
        public Microsoft.Xna.Framework.Vector2 PlayerStartPositionSim { get; set; }
        public Config()
        {
            StartMapId = 2;
            Resolution = new System.Drawing.Size(1366, 700);
            PlayerStartPositionSim = new Microsoft.Xna.Framework.Vector2(1, 1);
        }
        /// <summary>
        /// Sauvegarde la configuration dans le fichier donné.
        /// </summary>
        /// <param name="filename"></param>
        public void Save(string filename)
        {
            Tools.Serializer.Serialize<Config>(this, filename);
        }
        /// <summary>
        /// Charge la configuration depuis un fichier.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static Config LoadFromFile(string filename)
        {
            Config cfg = Tools.Serializer.Deserialize<Config>(filename, true);
            return cfg;
        }
    }
}
