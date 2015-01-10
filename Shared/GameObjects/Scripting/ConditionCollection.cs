using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UeFGame.GameObjects.Scripting
{
    /// <summary>
    /// Indique le critère permettant d'affirmer que la liste de conditions est remplie :
    /// </summary>
    public enum ConditionType
    {
        All, // toutes les conditions remplies
        One, // juste une
    }
    /// <summary>
    /// Représente une collection de Conditions.
    /// </summary>
    public class ConditionCollection : List<Condition>
    {
        /// <summary>
        /// Critère permettant d'affirmer que la liste de conditions est remplie.
        /// </summary>
        public ConditionType Type { get; set; }
        /// <summary>
        /// Retourne vrai la condition représentée par cette collection est remplie.
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public bool IsValid(GameEvent evt, CommandSetContext cmd)
        {
            if (this.Count == 0)
                return true;
            bool oneTrue = false;
            bool allTrue = true;
            foreach (Condition cond in this)
            {
                bool value = cond(evt, cmd);
                if (value)
                    oneTrue = true;
                if (!value)
                    allTrue = false;
            }
            return Type == ConditionType.All ? allTrue : oneTrue;
        }
        /// <summary>
        /// Crée une nouvelle collection de conditions.
        /// </summary>
        public ConditionCollection()
            : base()
        {
            Type = ConditionType.All;
        }
    }
}
