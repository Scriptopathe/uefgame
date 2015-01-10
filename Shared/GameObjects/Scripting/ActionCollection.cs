using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UeFGame.GameObjects.Scripting
{
    /// <summary>
    /// Représente une liste d'actions.
    /// </summary>
    public class ActionCollection : List<IExecutable>, IExecutable
    {
        #region Variabkes / Properties
        int m_currentAction = 0;
        bool m_isTerminated = false;
        /// <summary>
        /// Obtient une valeur indiquant si la liste des actions est terminée.
        /// </summary>
        public bool IsTerminated { get { return m_isTerminated; } }
        #endregion
        /// <summary>
        /// Ajoute une nouvelle action à la liste d'actions.
        /// </summary>
        /// <param name="action"></param>
        public void Add(ActionDelegate action)
        {
            Add(new Action(action));
        }
        /// <summary>
        /// Update the actions.
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="set"></param>
        public void Update(GameEvent evt, CommandSetContext set)
        {
            if (m_isTerminated)
                return;
            this[m_currentAction].Prepare();
            this[m_currentAction].Update(evt, set);
            if (this[m_currentAction].IsTerminated)
            {
                m_currentAction++;
                if (m_currentAction == Count)
                {
                    m_isTerminated = true;
                    m_currentAction = 0;
                }
            }
        }

        /// <summary>
        /// Resets the action.
        /// </summary>
        public void Prepare()
        {
            m_isTerminated = false;
        }
    }
}
