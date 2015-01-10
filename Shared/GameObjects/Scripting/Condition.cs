using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UeFGame.GameObjects.Scripting
{
    /// <summary>
    /// Fonction représentant une prédicat pour la validation d'une condition.
    /// </summary>
    /// <returns>True si la condition est remplie, false sinon.</returns>
    public delegate bool Condition(GameEvent evt, CommandSetContext cmd);
}
