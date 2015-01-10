using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UeFGame.GameObjects.Scripting
{
    /// <summary>
    /// Classe représentant un ensemble de variables repérées par des clefs.
    /// </summary>
    public class ScriptVariables : Dictionary<string, object>
    {
        public new object this[string str]
        {
            get
            {
                return base[str];
            }
            set
            {
                if (!this.ContainsKey(str))
                    Add(str, value);
                else
                    base[str] = value;
            }
        }
        /// <summary>
        /// Initialise une variable si elle n'a jamais été initialisée.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Initialize(string key, object value)
        {
            if (this.ContainsKey(key))
                return;
            else
                Add(key, value);
        }
    }
    /// <summary>
    /// Représente une contexte de CommandSet.
    /// Contient des variables et autres.
    /// </summary>
    public class CommandSetContext
    {
        public ScriptVariables TempVars { get; set; }
        public CommandSetContext()
        {
            TempVars = new ScriptVariables();
        }

        /// <summary>
        /// Réinitialise ce Contexte à son état initial.
        /// </summary>
        public void Reset()
        {
            TempVars.Clear();
        }
    }
}
