using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Robert.IA
{
    /// <summary>
    /// Représente une collection de mots et de fonctions.
    /// </summary>
    class Language
    {
        /// <summary>
        /// Représente les WordFunctions disponibles pour ce langage;
        /// </summary>
        public Dictionary<string, WordFunction> Functions = new Dictionary<string,WordFunction>();
        /// <summary>
        /// Représente les mots disponibles pour ce langage.
        /// </summary>
        public List<Word> Words = new List<Word>();

    }
}
