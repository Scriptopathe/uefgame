using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Robert.IA
{
    /// <summary>
    /// Génère une phrase aléatoire.
    /// </summary>
    public class SentenceGenerator
    {
        #region Variables
        Language m_language;

        #endregion 
        /// <summary>
        /// Crée une nouvelle instance de SentenceGenerator à partir du langage spécifié.
        /// </summary>
        /// <param name="language"></param>
        public SentenceGenerator(Language language)
        {
            m_language = language;
        }


        /// <summary>
        /// Génère une phrase à partir d'un mot.
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public string GenerateFromWord(Word word)
        {
            StringBuilder sentence = new StringBuilder();
            sentence.Append(word.Value + " ");
            Word currentWord = word;
            while (word.Function == (m_language.Functions["stop"]))
            {
                WordFunction func = currentWord.Function.GetNextFunction();
                Dictionary<int, Word> matches = new Dictionary<int,Word>();
                foreach (Word w in currentWord.ConnectedWords)
                {
                    if (w.Function == func)
                    {
                        matches.Add(currentWord.ConnexionCount[currentWord.ConnectedWords.IndexOf(w)], w);
                    }
                }

                Word match = matches.First().Value;
                int bestMatchCount = int.MinValue;
                foreach (var kvp in matches)
                {
                    if (kvp.Key > bestMatchCount)
                    {
                        bestMatchCount = kvp.Key;
                        match = kvp.Value;
                    }
                }

                sentence.Append(match.Value + " ");
            }
            return sentence.ToString();
        }

        /// <summary>
        /// Génère une phrase à partir du contexte.
        /// </summary>
        /// <returns></returns>
        public string GenerateFromContext()
        {
            throw new NotImplementedException();
        }
    }
}
