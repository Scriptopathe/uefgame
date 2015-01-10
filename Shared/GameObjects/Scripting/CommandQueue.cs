using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UeFGame.GameObjects.Scripting
{
    /// <summary>
    /// Représente une Queue de commandes destinées à être exécutées les unes après les autres.
    /// </summary>
    public class CommandQueue
    {
        /// <summary>
        /// Obtient ou défini le set de commande de cette Queue.
        /// </summary>
        public List<CommandSet> Commands { get; set; }
        /// <summary>
        /// Représente le contexte de la queue.
        /// </summary>
        public CommandSetContext Context { get; set; }
        /// <summary>
        /// Mets à jour la queue.
        /// </summary>
        /// <param name="evt"></param>
        public void Update(GameEvent evt)
        {
            foreach (CommandSet command in Commands)
            {
                command.Prepare();
                command.Update(evt, Context);
            }
            
        }

        /// <summary>
        /// Crée une nouvelle instance de CommandQueue.
        /// </summary>
        public CommandQueue()
        {
            Commands = new List<CommandSet>();
            Context = new CommandSetContext();
        }

        /// <summary>
        /// Ajoute le commandset donné à la queue.
        /// </summary>
        /// <param name="commandSet"></param>
        public void Add(CommandSet commandSet)
        {
            Commands.Add(commandSet);
        }

        /// <summary>
        /// Réinitialise la CommandQueue à son état initial.
        /// </summary>
        public void Reset()
        {
            Commands.Clear();
            Context.Reset();
        }
    }
}
