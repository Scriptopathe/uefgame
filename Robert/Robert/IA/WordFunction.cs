using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Robert.IA
{
    /// <summary>
    /// Représente une fonction du mot dans une phrase :
    /// - Sujet
    /// - Verbe
    /// - COD
    /// - COI
    /// - Adj
    /// - Adv
    /// etc
    /// </summary>
    public class WordFunction
    {
        /// <summary>
        /// Retourne vrai si la fonction peut être représentée par un mot de la classe grammaticale donnée.
        /// </summary>
        /// <param name="gram"></param>
        /// <returns></returns>
        public bool CanBe(GrammaticalClass gram);
        /// <summary>
        /// Retourne une des possible fonctions grammaticales du mot suivant.
        /// </summary>
        /// <returns></returns>
        public WordFunction GetNextFunction();
        public bool IsPlural;
        public bool IsMasculin;
    }
}
