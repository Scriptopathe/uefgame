using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UeFGame.GameObjects.Scripting
{
    /// <summary>
    /// Représente une structure de contrôle "if".
    /// </summary>
    public class IfAction : IExecutable
    {
        #region Properties
        public bool IsTerminated { get; set; }
        public Condition Cond { get; set; }
        public IExecutable TrueAction { get; set; }
        public IExecutable FalseAction { get; set; }
        bool m_trueCondition;
        bool m_started;
        #endregion
        /// <summary>
        /// Mets à jour l'action.
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="set"></param>
        public void Update(GameEvent evt, CommandSetContext set)
        {
            // On vérifie la condition si on n'a pas encore démarré.
            if (!m_started)
            {
                m_trueCondition = Cond(evt, set);
                m_started = true;
                IsTerminated = false;
            }
            
            IExecutable current = m_trueCondition ? TrueAction : FalseAction;

            if (current == null)
            {
                IsTerminated = true;
                m_started = false;
                return;
            }

            current.Prepare();
            current.Update(evt, set);
            // Si l'action est terminée, on se marque terminé.
            if (current.IsTerminated)
            {
                m_started = false;
                IsTerminated = true;
            }
        }
        /// <summary>
        /// Préparation à l'exécution.
        /// </summary>
        public void Prepare()
        {
            IsTerminated = false;
        }
        /// <summary>
        /// Crée une nouvelle branche "if" avec les tru
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="trueActions"></param>
        /// <param name="falseActions"></param>
        public IfAction(Condition condition, IExecutable trueActions, IExecutable falseActions) 
        {
            Cond = condition;
            TrueAction = trueActions;
            FalseAction = falseActions;
            m_started = false;
        }
        /// <summary>
        /// Crée une nouvelle branche vide.
        /// </summary>
        public IfAction() { }
    }
}
