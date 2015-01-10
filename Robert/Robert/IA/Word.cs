using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Robert.IA
{
    /// <summary>
    /// Représente un mot.
    /// </summary>
    public class Word
    {
        /// <summary>
        /// Liste des mots connectés à ce mot.
        /// </summary>
        public List<Word> ConnectedWords = new List<Word>();
        /// <summary>
        /// Liste des nombre de connexions pour chaque mot de ConnectedWord.
        /// </summary>
        public List<int> ConnexionCount = new List<int>();
        /// <summary>
        /// Liste des fonctions de mots.
        /// </summary>
        public WordFunction Function = new WordFunction();
        /// <summary>
        /// Retourne la chaine représentée par ce mot.
        /// </summary>
        public string Value;
    }
}
