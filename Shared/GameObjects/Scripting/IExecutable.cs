using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UeFGame.GameObjects.Scripting
{
    /// <summary>
    /// Représente une action exécutable.
    /// </summary>
    public interface IExecutable
    {
        void Update(GameEvent evt, CommandSetContext set);
        bool IsTerminated { get; }
        void Prepare();
    }
}
