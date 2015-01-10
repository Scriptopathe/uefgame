using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
namespace UeFGame.GameObjects.Player.Combos
{
    /// <summary>
    /// Représente un noeud de l'arbre de combos.
    /// </summary>
    public class ChainComboNode
    {
        /// <summary>
        /// Délai avant la fin du combo.
        /// </summary>
        public int Delay { get; set;}
        /// <summary>
        /// Id de l'action à exécuter.
        /// </summary>
        public int Action { get; set;}
        /// <summary>
        /// Permet l'accès au enfants de ce noeud.
        /// </summary>
        public Dictionary<Keys, ChainComboNode> Children;

        public ChainComboNode(int delay, int action, Dictionary<Keys, ChainComboNode> children)
        {
            Delay = delay;
            Action = action;
            Children = children;
        }

        public ChainComboNode(int delay, int action)
        {
            Delay = delay;
            Action = action;
            Children = new Dictionary<Keys, ChainComboNode>();
        }
    }
}
