using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UeFGame.GameObjects.Scripting
{
    public partial class CommandSet
    {
        /// <summary>
        /// Permet la création rapide de conditions.
        /// </summary>
        public static class ConditionFactory
        {
            /// <summary>
            /// Retourne vrai si l'objet de la condition est en collision avec le joueur.
            /// </summary>
            /// <returns></returns>
            public static Condition CollideWithPlayer = new Condition(delegate(GameEvent evt, CommandSetContext context)
            {
                return Globals.GameMap.GetPlayer().IsCollidingWith(evt);
            });

            /// <summary>
            /// Retourne vrai si l'objet de la donné est en collision avec l'objet courant.
            /// </summary>
            /// <param name="extEvt"></param>
            /// <returns></returns>
            public static Condition CollideWith(GameEvent extEvt)
            {
                return new Condition(delegate(GameEvent evt, CommandSetContext context)
                {
                    if (evt.IsCollidingWith(extEvt))
                        return true;
                    else
                        return false;
                    //return evt.IsCollidingWith(extEvt);
                });
            }
        }
    }
}
