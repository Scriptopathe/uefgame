using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UeFGame.GameObjects.Scripting
{
    /// <summary>
    /// Représente une méthode définissant une action.
    /// </summary>
    /// <param name="evt"></param>
    /// <param name="cmd"></param>
    /// <returns>True si l'action continue, false sinon.</returns>
    public delegate bool ActionDelegate(GameEvent evt, CommandSetContext cmd);
    /// <summary>
    /// Représente une action de commande.
    /// Doit retourner true si l'action continue, false sinon.
    /// </summary>
    public class Action : IExecutable
    {
        /// <summary>
        /// Représente la fonction appelée par cette action.
        /// </summary>
        private ActionDelegate m_function;
        private bool m_isTerminated;
        /// <summary>
        /// Appelle la fonction de l'action.
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public void Update(GameEvent evt, CommandSetContext cmd)
        {
            m_isTerminated = !m_function(evt, cmd);
        }
        /// <summary>
        /// Retourne true si l'action est terminée.
        /// </summary>
        /// <returns></returns>
        public bool IsTerminated
        {
            get { return m_isTerminated; }
        }
        /// <summary>
        /// Crée une nouvelle action avec la fonction donnée.
        /// </summary>
        /// <param name="action"></param>
        public Action(ActionDelegate fonction)
        {
            m_function = fonction;
        }

        /// <summary>
        /// Prepare cette action à son exécution.
        /// </summary>
        public void Prepare()
        {
            m_isTerminated = false;
        }
    }
}
