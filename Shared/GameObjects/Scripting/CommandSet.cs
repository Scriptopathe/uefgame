using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UeFGame.GameObjects.Scripting
{
    /// <summary>
    /// Représente un ensemble de commandes dont l'exécution est liée à des conditions.
    /// </summary>
    public partial class CommandSet : IExecutable
    {
        #region Variables
        private bool m_executionStarted = false;
        private bool m_alreadyExecuted = false;
        /// <summary>
        /// Obtient une valeur indiquant si le CommandSet actuel est en cours d'exécution.
        /// </summary>
        public bool IsExecuting
        {
            get { return m_executionStarted; }
        }

        public bool IsTerminated
        {
            get { return !m_executionStarted; }
        }
        #endregion
        /// <summary>
        /// Représente une liste de conditions nécessaires à l'éxécution des commandes
        /// contenues dans ce CommandSet.
        /// </summary>
        public ConditionCollection Conditions { get; set; }
        /// <summary>
        /// Représente la liste d'actions à effectuer par ce command set.
        /// </summary>
        public ActionCollection Actions { get; set; }
        /// <summary>
        /// Obtient ou définit une valeur indiquant si le CommandSet recommence son exécution une fois terminé.
        /// </summary>
        public bool CanLoop { get; set; }
        /// <summary>
        /// Continue l'exécution du command set.
        /// </summary>
        /// <returns></returns>
        public void Update(GameEvent evt, CommandSetContext context)
        {
            // On vérifie la condition de lancement.
            if (!m_executionStarted && (CanLoop || !m_alreadyExecuted))
            {
                if (Conditions.IsValid(evt, context))
                {
                    m_executionStarted = true;
                    m_alreadyExecuted = true;
                }
            }

            if (m_executionStarted)
            {
                Actions.Prepare();
                Actions.Update(evt, context);
                if (Actions.IsTerminated)
                {
                    m_executionStarted = false;
                }
            }
        }
        /// <summary>
        /// Initialise le commandset avec les Actions données.
        /// </summary>
        /// <param name="exes"></param>
        public CommandSet(params IExecutable[] exes)
        {
            Actions = new ActionCollection();
            Conditions = new ConditionCollection();
            Actions.AddRange(exes);
        }
        /// <summary>
        /// Constructeur sans paramètres.
        /// </summary>
        public CommandSet()
        {
            Actions = new ActionCollection();
            Conditions = new ConditionCollection();
        }
        /// <summary>
        /// Ajoute une action dans la liste d'action.
        /// </summary>
        /// <param name="exe"></param>
        public void Add(IExecutable exe)
        {
            Actions.Add(exe);
        }

        /// <summary>
        /// Réinitialise le CommandSet.
        /// </summary>
        public void Reset()
        {
            Conditions.Clear();
            Actions.Clear();
            CanLoop = false;
            m_executionStarted = false;
            m_alreadyExecuted = false;
        }
        /// <summary>
        /// Prépare le CommandSet.
        /// </summary>
        public void Prepare()
        {
        }
    }
}
